using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;

namespace dtc.Application.Features.Training.Services
{
    public class StudentDrivingDistanceService : IStudentDrivingDistanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentDrivingDistanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentDrivingDistanceResponseDto> CreateDistanceRecordAsync(CreateStudentDrivingDistanceRequestDto request)
        {
            var existing = await _unitOfWork.StudentDrivingDistances.FindAsync(d => d.StudentId == request.StudentId);
            if (existing.Any())
            {
                throw new Exception("Student already has a driving distance record.");
            }

            var record = new StudentDrivingDistance(request.StudentId, request.MaxMorningDistanceKm, request.MaxEveningDistanceKm);
            await _unitOfWork.StudentDrivingDistances.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(record);
        }

        public async Task<StudentDrivingDistanceResponseDto> RecordActualDistanceAsync(Guid id, UpdateStudentDrivingDistanceRequestDto request, Guid adminId)
        {
            var record = await _unitOfWork.StudentDrivingDistances.GetByIdAsync(id);
            if (record == null) throw new Exception("Record not found");

            if (request.MorningDistanceKm > 0)
                record.AddMorningDistance(request.MorningDistanceKm, adminId);
            
            if (request.EveningDistanceKm > 0)
                record.AddEveningDistance(request.EveningDistanceKm, adminId);

            await _unitOfWork.StudentDrivingDistances.UpdateAsync(record);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(record);
        }

        public async Task<bool> DeleteDistanceRecordAsync(Guid id)
        {
            var record = await _unitOfWork.StudentDrivingDistances.GetByIdAsync(id);
            if (record == null) throw new Exception("Record not found");

            await _unitOfWork.StudentDrivingDistances.RemoveAsync(record);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<StudentDrivingDistanceResponseDto> GetDistanceRecordByIdAsync(Guid id)
        {
            var record = await _unitOfWork.StudentDrivingDistances.GetByIdAsync(id);
            if (record == null) throw new Exception("Record not found");

            return await MapToDtoAsync(record);
        }

        public async Task<IEnumerable<StudentDrivingDistanceResponseDto>> GetMyDrivingDistancesAsync(Guid studentId)
        {
            var records = await _unitOfWork.StudentDrivingDistances.FindAsync(d => d.StudentId == studentId);
            var dtos = new List<StudentDrivingDistanceResponseDto>();
            foreach (var r in records) dtos.Add(await MapToDtoAsync(r));
            return dtos;
        }

        public async Task<IEnumerable<StudentDrivingDistanceResponseDto>> GetAllDrivingDistancesAsync()
        {
            var records = await _unitOfWork.StudentDrivingDistances.GetAllAsync();
            var dtos = new List<StudentDrivingDistanceResponseDto>();
            foreach (var r in records) dtos.Add(await MapToDtoAsync(r));
            return dtos;
        }

        private async Task<StudentDrivingDistanceResponseDto> MapToDtoAsync(StudentDrivingDistance distance)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(distance.StudentId);
            return new StudentDrivingDistanceResponseDto
            {
                Id = distance.Id,
                StudentId = distance.StudentId,
                StudentName = student?.FullName ?? "Unknown",
                MorningDistanceKm = distance.MorningDistanceKm,
                EveningDistanceKm = distance.EveningDistanceKm,
                MaxMorningDistanceKm = distance.MaxMorningDistanceKm,
                MaxEveningDistanceKm = distance.MaxEveningDistanceKm,
                CreatedAt = distance.CreatedAt
            };
        }
    }
}
