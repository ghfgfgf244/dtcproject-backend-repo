using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dtc.API.Controllers
{
    [Authorize]
    public class DocumentController : BaseApiController
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
                return Created(response, "Document added successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
                return Ok(response, "Document updated successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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
                return NoContent("Document deleted successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(Guid id)
        {
            try
            {
                var response = await _documentService.GetDocumentByIdAsync(id);
                return Ok(response);
            }
            catch
            {
                return NotFound("Document");
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
                return NoContent("Document verified successfully.");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
