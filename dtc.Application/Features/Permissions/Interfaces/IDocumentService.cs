using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using dtc.Application.Features.Permissions.Interfaces;
using dtc.Application.Features.Permissions.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Permissions.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentResponseDto> CreateDocumentAsync(Guid userId, CreateDocumentRequestDto request);
        Task<DocumentResponseDto> UpdateDocumentAsync(Guid userId, Guid documentId, UpdateDocumentRequestDto request);
        Task<bool> DeleteDocumentAsync(Guid userId, Guid documentId);
        Task<IEnumerable<DocumentResponseDto>> GetMyDocumentsAsync(Guid userId);
        Task<DocumentResponseDto> GetDocumentByIdAsync(Guid documentId);
        Task<bool> VerifyDocumentAsync(Guid documentId);
    }
}
