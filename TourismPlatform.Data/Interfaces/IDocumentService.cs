using System;
using TourismPlatform.Core.DTOs.Document;

namespace TourismPlatform.Data.Interfaces;

public interface IDocumentService
{
    Task<DocumentResponseDto> UploadDocumentAsync(UploadDocumentDto uploadDto, Guid tenantId, string uploadedBy);
    Task<DocumentResponseDto?> GetDocumentByIdAsync(Guid id, Guid tenantId);
    Task<List<DocumentResponseDto>> GetDocumentsAsync(DocumentSearchDto searchDto, Guid tenantId);
    Task<(Stream stream, string fileName, string contentType)> DownloadDocumentAsync(Guid id, Guid tenantId);
    Task<bool> DeleteDocumentAsync(Guid id, Guid tenantId);
    Task<List<string>> GetDocumentTypesAsync(Guid tenantId);
}
