using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TourismPlatform.Core.DTOs.Document;
using TourismPlatform.Core.Entities;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.Data.Services;

public class DocumentService : IDocumentService
{
    private readonly TourismDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadPath;

    public DocumentService(TourismDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        _uploadPath = Path.Combine(_environment.ContentRootPath, "uploads", "documents");
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    public async Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto uploadDto, string uploadedBy)
    {
        // Validate file
        if (uploadDto.File.Length == 0)
            throw new ArgumentException("File cannot be empty");

        if (uploadDto.File.Length > 10 * 1024 * 1024) // 10MB limit
            throw new ArgumentException("File size cannot exceed 10MB");

        List<string> allowedExtensions = new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        string fileExtension = Path.GetExtension(uploadDto.File.FileName).ToLower();

        if (!allowedExtensions.Contains(fileExtension))
            throw new ArgumentException($"File type {fileExtension} is not allowed");

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadPath, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.File.CopyToAsync(stream);
        }

        // Create document record
        var document = new Document
        {
            Id = Guid.NewGuid(),
            DocumentType = uploadDto.DocumentType,
            FileName = uniqueFileName,
            FilePath = filePath,
            FileExtension = fileExtension,
            FileSizeBytes = (int)uploadDto.File.Length,
            BookingId = uploadDto.BookingId,
            CustomerId = uploadDto.CustomerId,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = uploadedBy
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return await GetDocumentByIdAsync(document.Id) 
            ?? throw new InvalidOperationException("Failed to retrieve uploaded document");
    }

    public async Task<DocumentResponseDto?> GetDocumentByIdAsync(Guid id)
    {
        var document = await _context.Documents
            .Include(d => d.Booking)
            .Include(d => d.Customer)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null) return null;

        return MapToResponseDto(document);
    }

    public async Task<List<DocumentResponseDto>> GetDocumentsAsync(DocumentSearchDto searchDto)
    {
        var query = _context.Documents
            .Include(d => d.Booking)
            .Include(d => d.Customer)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchDto.DocumentType))
        {
            query = query.Where(d => d.DocumentType == searchDto.DocumentType);
        }

        if (searchDto.BookingId.HasValue)
        {
            query = query.Where(d => d.BookingId == searchDto.BookingId);
        }

        if (searchDto.CustomerId.HasValue)
        {
            query = query.Where(d => d.CustomerId == searchDto.CustomerId);
        }

        if (searchDto.FromDate.HasValue)
        {
            query = query.Where(d => d.UploadedAt >= searchDto.FromDate);
        }

        if (searchDto.ToDate.HasValue)
        {
            query = query.Where(d => d.UploadedAt <= searchDto.ToDate);
        }

        var documents = await query
            .OrderByDescending(d => d.UploadedAt)
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        return documents.Select(MapToResponseDto).ToList();
    }

    public async Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(Guid id)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null)
            throw new FileNotFoundException("Document not found");

        if (!File.Exists(document.FilePath))
            throw new FileNotFoundException("Physical file not found");

        var stream = new FileStream(document.FilePath, FileMode.Open, FileAccess.Read);
        var contentType = GetContentType(document.FileExtension);
        
        return (stream, document.FileName, contentType);
    }

    public async Task<bool> DeleteDocumentAsync(Guid id)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id);

        if (document == null) return false;

        // Delete physical file
        if (File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
        }

        // Delete database record
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<string>> GetDocumentTypesAsync()
    {
        return await _context.Documents
            .Select(d => d.DocumentType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }

    private DocumentResponseDto MapToResponseDto(Document document)
    {
        return new DocumentResponseDto
        {
            Id = document.Id,
            DocumentType = document.DocumentType,
            FileName = document.FileName,
            OriginalFileName = document.FileName,
            FileExtension = document.FileExtension,
            FileSizeBytes = document.FileSizeBytes,
            FileSizeFormatted = FormatFileSize(document.FileSizeBytes),
            BookingId = document.BookingId,
            BookingNumber = document.Booking?.BookingNumber,
            CustomerId = document.CustomerId,
            CustomerName = document.Customer != null 
                ? $"{document.Customer.FirstName} {document.Customer.LastName}" 
                : null,
            UploadedAt = document.UploadedAt,
            UploadedBy = document.UploadedBy,
            DownloadUrl = $"/api/documents/{document.Id}/download"
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    private string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}