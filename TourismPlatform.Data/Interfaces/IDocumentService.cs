using System;
using TourismPlatform.Core.DTOs.Document;

namespace TourismPlatform.Data.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto uploadDto, string uploadedBy);
    Task<DocumentResponseDto?> GetDocumentByIdAsync(Guid id);
    Task<List<DocumentResponseDto>> GetDocumentsAsync(DocumentSearchDto searchDto);
    Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(Guid id);
    Task<bool> DeleteDocumentAsync(Guid id);
    Task<List<string>> GetDocumentTypesAsync();
}
