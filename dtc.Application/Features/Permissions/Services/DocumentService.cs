using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Interfaces;
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
        private readonly ICloudinaryService _cloudinaryService;

        public DocumentService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        // DEV-132: Add personal's document
        public async Task<DocumentResponseDto> CreateDocumentAsync(Guid userId, CreateDocumentRequestDto request)
        {
            var document = new Document(
                userId: userId,
                providerPublicId: request.ProviderPublicId,
                version: request.Version,
                resourceType: request.ResourceType,
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
                throw new UnauthorizedAccessException("Document not found or access denied.");

            document.ChangeFile(
                request.ProviderPublicId, 
                request.Version, 
                request.ResourceType, 
                request.Extension, 
                request.Size);

            await _unitOfWork.Documents.UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(document);
        }

        // DEV-134: Delete personal's document
        public async Task<bool> DeleteDocumentAsync(Guid userId, Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null || document.UserId != userId)
                throw new UnauthorizedAccessException("Document not found or access denied.");

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
            if (document == null) throw new KeyNotFoundException("Document not found.");

            return MapToDto(document);
        }

        // DEV-136: Verified personal's document
        public async Task<bool> VerifyDocumentAsync(Guid documentId)
        {
            var document = await _unitOfWork.Documents.GetByIdAsync(documentId);
            if (document == null) throw new KeyNotFoundException("Document not found.");

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
                ResourceType = doc.ResourceType,
                ProviderPublicId = doc.ProviderPublicId,
                Version = doc.Version,
                FileName = doc.FileName,
                Extension = doc.Extension,
                Size = doc.Size,
                IsVerified = doc.IsVerified,
                FileUrl = _cloudinaryService.GetUrl(doc.ProviderPublicId, doc.Version, doc.ResourceType, isSecure: true)
            };
        }
    }
}
