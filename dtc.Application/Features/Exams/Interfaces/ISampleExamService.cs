using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Interfaces
{
    public interface ISampleExamService
    {
        Task<SampleExamResponseDto> CreateSampleExamAsync(CreateSampleExamRequestDto request);
        Task<SampleExamResponseDto> UpdateSampleExamQuestionsAsync(Guid id, UpdateSampleExamQuestionsRequestDto request);
        Task<bool> DeleteSampleExamAsync(Guid id);
        Task<SampleExamDetailResponseDto> GetSampleExamDetailAsync(Guid id);
        Task<IEnumerable<SampleExamResponseDto>> GetAllSampleExamsAsync();
        
        Task<SampleTestResultResponseDto> DoSampleTestAsync(Guid sampleExamId, Guid studentId, SubmitSampleTestRequestDto request);
        Task<IEnumerable<SampleTestResultResponseDto>> GetSampleTestResultsForStudentAsync(Guid studentId);
    }
}
