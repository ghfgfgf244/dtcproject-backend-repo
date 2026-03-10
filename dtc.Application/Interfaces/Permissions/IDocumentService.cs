using dtc.Application.DTOs.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Interfaces.Permissions
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
