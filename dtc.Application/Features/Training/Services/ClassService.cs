using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClassResponseDto> CreateClassAsync(CreateClassRequestDto request, Guid adminId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(request.TermId);
            if (term == null)
                throw new Exception("Term not found");

            var newClass = new Class(request.TermId, request.InstructorId, request.ClassName, request.MaxStudents, adminId);
            
            await _unitOfWork.Classes.AddAsync(newClass);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(newClass);
        }

        public async Task<ClassResponseDto> UpdateClassAsync(Guid classId, UpdateClassRequestDto request, Guid adminId)
        {
            var existingClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (existingClass == null)
                throw new Exception("Class not found");

            var changed = existingClass.UpdateInfo(request.ClassName, request.MaxStudents, adminId);
            
            if (request.Status.HasValue && existingClass.Status != request.Status.Value)
            {
                existingClass.ChangeStatus(request.Status.Value, adminId);
                changed = true;
            }

            if (changed)
            {
                await _unitOfWork.Classes.UpdateAsync(existingClass);
                await _unitOfWork.SaveChangesAsync();
            }

            return MapToDto(existingClass);
        }

        public async Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            return classes.Select(MapToDto);
        }

        public async Task<ClassResponseDto> GetClassDetailAsync(Guid classId)
        {
            var existingClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (existingClass == null)
                throw new Exception("Class not found");

            return MapToDto(existingClass);
        }

        public async Task<bool> DeleteClassAsync(Guid classId, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null) throw new Exception("Class not found");

            if (classEntity.Status == ClassStatus.InProgress || classEntity.Status == ClassStatus.Completed)
            {
                classEntity.ChangeStatus(ClassStatus.Cancelled, adminId);
            }
            
            classEntity.SoftDelete(adminId);
            await _unitOfWork.Classes.UpdateAsync(classEntity);
            
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTeachersToClassAsync(Guid classId, AssignTeachersRequestDto request, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null) throw new Exception("Class not found");

            var instructorId = request.InstructorIds.FirstOrDefault();
            if (instructorId != Guid.Empty)
            {
                classEntity.ChangeInstructor(instructorId, adminId);
                await _unitOfWork.Classes.UpdateAsync(classEntity);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> AssignStudentsToClassAsync(Guid classId, AssignStudentsRequestDto request, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null) throw new Exception("Class not found");

            var distinctIds = (request.StudentIds ?? new List<Guid>()).Distinct().ToList();
            if (distinctIds.Count > classEntity.MaxStudents)
                throw new InvalidOperationException("Too many students for this class capacity.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId);
                var existingSet = existing.Select(e => e.StudentId).ToHashSet();
                var requestedSet = distinctIds.ToHashSet();

                var toRemove = existing.Where(e => !requestedSet.Contains(e.StudentId)).ToList();
                if (toRemove.Count > 0)
                    await _unitOfWork.ClassStudents.RemoveRange(toRemove);

                var toAdd = distinctIds.Where(id => !existingSet.Contains(id)).ToList();
                if (toAdd.Count > 0)
                {
                    var students = await _unitOfWork.Users.FindAsync(u => toAdd.Contains(u.Id));
                    if (students.Count() != toAdd.Count)
                        throw new InvalidOperationException("One or more students were not found.");
                    
                    foreach (var studentId in toAdd)
                        await _unitOfWork.ClassStudents.AddAsync(new ClassStudent(classId, studentId));
                }

                classEntity.SyncEnrollmentCount(distinctIds.Count, adminId);
                await _unitOfWork.Classes.UpdateAsync(classEntity);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private ClassResponseDto MapToDto(Class classEntity)
        {
            return new ClassResponseDto
            {
                Id = classEntity.Id,
                TermId = classEntity.TermId,
                ClassName = classEntity.ClassName,
                CurrentStudents = classEntity.CurrentStudents,
                MaxStudents = classEntity.MaxStudents,
                Status = classEntity.Status.ToString(),
                CreatedAt = classEntity.CreatedAt
            };
        }
    }
}
