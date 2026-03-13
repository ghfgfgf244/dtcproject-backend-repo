using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Entities;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Permissions.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // DEV-132: Add personal's document
        public async Task<DocumentResponseDto> CreateDocumentAsync(Guid userId, CreateDocumentRequestDto request)
        {
            var document = new Document(
                userId: userId,
                resourceType: (ResourceType)request.ResourceType,
                fileUrl: request.FileUrl,
                fileName: request.FileName,
                extension: request.Extension,
                size: request.Size
            );

            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(document);
        }

        // DEV-133: Update personal's document
        public async Task<DocumentResponseDto> UpdateDocumentAsync(Guid userId, Guid documentId, UpdateDocumentRequestDto request)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null || document.UserId != userId)
                throw new Exception("Document not found or access denied.");

            document.ChangeFile(request.FileUrl, request.Extension, request.Size);

            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(document);
        }

        // DEV-134: Delete personal's document
        public async Task<bool> DeleteDocumentAsync(Guid userId, Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null || document.UserId != userId)
                throw new Exception("Document not found or access denied.");

            await _unitOfWork.Documents.RemoveAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // DEV-135: View personal's document
        public async Task<IEnumerable<DocumentResponseDto>> GetMyDocumentsAsync(Guid userId)
        {
            var documents = await _unitOfWork.Documents.FindAsync(d => d.UserId == userId);
            var dtos = new List<DocumentResponseDto>();
            foreach (var doc in documents)
            {
                dtos.Add(MapToDto(doc));
            }
            return dtos;
        }

        public async Task<DocumentResponseDto> GetDocumentByIdAsync(Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null) throw new Exception("Document not found.");

            return MapToDto(document);
        }

        // DEV-136: Verified personal's document
        public async Task<bool> VerifyDocumentAsync(Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null) throw new Exception("Document not found.");

            document.Verify();
            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private DocumentResponseDto MapToDto(Document doc)
        {
            return new DocumentResponseDto
            {
                Id = doc.Id,
                UserId = doc.UserId,
                ResourceType = doc.ResourceType.ToString(),
                FileUrl = doc.FileUrl,
                FileName = doc.FileName,
                Extension = doc.Extension,
                Size = doc.Size,
                IsVerified = doc.IsVerified
            };
        }
    }
}
