using dtc.Application.DTOs.Permissions;
using dtc.Application.Interfaces.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        // DEV-132: Add personal's document
        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _documentService.CreateDocumentAsync(userId, request);
                return CreatedAtAction(nameof(GetDocumentById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-133: Update personal's document
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var response = await _documentService.UpdateDocumentAsync(userId, id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-134: Delete personal's document
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _documentService.DeleteDocumentAsync(userId, id);
                return Ok(new { Message = "Document deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DEV-135: View personal's document (All)
        [HttpGet]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var response = await _documentService.GetMyDocumentsAsync(userId);
            return Ok(response);
        }

        // DEV-135 (Specific Document)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            try
            {
                var response = await _documentService.GetDocumentByIdAsync(id);
                // Security check logic per user could be enforced in service layer
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // DEV-136: Verified personal's document
        [HttpPost("{id}/verify")]
        [Authorize(Roles = "Admin,TrainingManager,EnrollmentManager")]
        public async Task<IActionResult> VerifyDocument(Guid id)
        {
            try
            {
                await _documentService.VerifyDocumentAsync(id);
                return Ok(new { Message = "Document verified successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
