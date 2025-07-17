using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismPlatform.Core.DTOs.Document;
using TourismPlatform.Data.Interfaces;

namespace TourismPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : BaseController
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DocumentResponseDto>>> GetDocuments(
            [FromQuery] string? documentType = null,
            [FromQuery] Guid? bookingId = null,
            [FromQuery] Guid? customerId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenantId = GetTenantId();
            var searchDto = new DocumentSearchDto
            {
                DocumentType = documentType,
                BookingId = bookingId,
                CustomerId = customerId,
                FromDate = fromDate,
                ToDate = toDate,
                Page = page,
                PageSize = pageSize
            };

            var documents = await _documentService.GetDocumentsAsync(searchDto, tenantId);
            return Ok(documents);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DocumentResponseDto>> GetDocument(Guid id)
        {
            var tenantId = GetTenantId();
            var document = await _documentService.GetDocumentByIdAsync(id, tenantId);
            
            if (document == null)
                return NotFound($"Document with ID {id} not found");

            return Ok(document);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<DocumentResponseDto>> UploadDocument([FromForm] UploadDocumentDto uploadDto)
        {
            try
            {
                var tenantId = GetTenantId();
                var uploadedBy = GetUserId();
                
                var document = await _documentService.UploadDocumentAsync(uploadDto, tenantId, uploadedBy);
                
                return CreatedAtAction(
                    nameof(GetDocument), 
                    new { id = document.Id }, 
                    document);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id:guid}/download")]
        public async Task<ActionResult> DownloadDocument(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var (stream, fileName, contentType) = await _documentService.DownloadDocumentAsync(id, tenantId);
                
                return File(stream, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteDocument(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deleted = await _documentService.DeleteDocumentAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound($"Document with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<string>>> GetDocumentTypes()
        {
            var tenantId = GetTenantId();
            var types = await _documentService.GetDocumentTypesAsync(tenantId);
            return Ok(types);
        }
    }
}