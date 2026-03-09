using dtc.Application.DTOs.Training.Classes;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
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

            var newClass = new Class(request.TermId, request.ClassName, request.MaxStudents, adminId);
            
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
            var classes = await _unitOfWork.Classes.FindAsync(c => c.Id == classId, c => c.Instructors);
            var classEntity = classes.FirstOrDefault();
            if (classEntity == null) throw new Exception("Class not found");

            var users = await _unitOfWork.Users.FindAsync(u => request.InstructorIds.Contains(u.Id));
            classEntity.SyncInstructors(users, adminId);

            await _unitOfWork.Classes.UpdateAsync(classEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignStudentsToClassAsync(Guid classId, AssignStudentsRequestDto request, Guid adminId)
        {
            var classes = await _unitOfWork.Classes.FindAsync(c => c.Id == classId, c => c.Students);
            var classEntity = classes.FirstOrDefault();
            if (classEntity == null) throw new Exception("Class not found");

            var users = await _unitOfWork.Users.FindAsync(u => request.StudentIds.Contains(u.Id));
            classEntity.SyncStudents(users, adminId);

            await _unitOfWork.Classes.UpdateAsync(classEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
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
