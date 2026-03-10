using dtc.Application.DTOs.Training.Schedules;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClassScheduleResponseDto> CreateScheduleAsync(CreateClassScheduleRequestDto request, Guid adminId)
        {
            // Verify class exists
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(request.ClassId);
            if (classEntity == null)
                throw new Exception("Class not found");

            // Verify instructor exists
            var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId);
            if (instructor == null)
                throw new Exception("Instructor not found");

            // Verify if instructor is already scheduled during this time
            var existingSchedules = await _unitOfWork.ClassSchedules.FindAsync(s => 
                s.InstructorId == request.InstructorId && 
                s.StartTime < request.EndTime && 
                request.StartTime < s.EndTime);
                
            if (existingSchedules != null && existingSchedules.Any())
                throw new InvalidOperationException("Instructor is already scheduled for another class during this time period.");

            var schedule = new ClassSchedule(
                classId: request.ClassId,
                instructorId: request.InstructorId,
                startTime: request.StartTime,
                endTime: request.EndTime,
                location: request.Location,
                createdBy: adminId
            );

            await _unitOfWork.ClassSchedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return await GetScheduleDetailAsync(schedule.Id);
        }

        public async Task<ClassScheduleResponseDto> UpdateScheduleAsync(Guid id, UpdateClassScheduleRequestDto request, Guid adminId)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Class schedule not found");

            bool isUpdated = false;

            // Handle rescheduling
            isUpdated = schedule.Reschedule(
                newStart: request.StartTime,
                newEnd: request.EndTime,
                newLocation: request.Location,
                updatedBy: adminId
            );

            // Handle instructor change if provided
            if (request.InstructorId.HasValue && request.InstructorId.Value != schedule.InstructorId)
            {
                var newInstructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId.Value);
                if (newInstructor == null)
                    throw new Exception("New instructor not found");

                schedule.ChangeInstructor(request.InstructorId.Value, adminId);
                isUpdated = true;
            }

            // Verify overlapping if time or instructor changed
            if (isUpdated)
            {
                var existingSchedules = await _unitOfWork.ClassSchedules.FindAsync(s => 
                    s.Id != schedule.Id &&
                    s.InstructorId == schedule.InstructorId && 
                    s.StartTime < schedule.EndTime && 
                    schedule.StartTime < s.EndTime);
                    
                if (existingSchedules != null && existingSchedules.Any())
                    throw new InvalidOperationException("Instructor is already scheduled for another class during this time period.");

                await _unitOfWork.ClassSchedules.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
            }

            return await GetScheduleDetailAsync(schedule.Id);
        }

        public async Task<bool> DeleteScheduleAsync(Guid id, Guid adminId)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Schedule not found");

            schedule.SoftDelete(adminId);
            await _unitOfWork.ClassSchedules.UpdateAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ClassScheduleResponseDto> GetScheduleDetailAsync(Guid id)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Schedule not found");

            return await MapToDtoCompleteAsync(schedule);
        }

        public async Task<IEnumerable<ClassScheduleResponseDto>> GetSchedulesByClassAsync(Guid classId)
        {
            var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == classId);
            if (schedules == null || !schedules.Any())
                return new List<ClassScheduleResponseDto>();

            var dtos = new List<ClassScheduleResponseDto>();
            foreach (var schedule in schedules)
            {
                dtos.Add(await MapToDtoCompleteAsync(schedule));
            }
            return dtos.OrderBy(s => s.StartTime);
        }

        public async Task<bool> AssignLocationAsync(Guid id, AssignLocationRequestDto request, Guid adminId)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Schedule not found");

            bool isUpdated = schedule.Reschedule(
                newStart: schedule.StartTime, 
                newEnd: schedule.EndTime, 
                newLocation: request.Location, 
                updatedBy: adminId);

            if (isUpdated)
            {
                await _unitOfWork.ClassSchedules.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        private async Task<ClassScheduleResponseDto> MapToDtoCompleteAsync(ClassSchedule schedule)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(schedule.InstructorId);
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(schedule.ClassId);

            return new ClassScheduleResponseDto
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                InstructorId = schedule.InstructorId,
                ClassName = classEntity?.ClassName ?? "Unknown Class",
                InstructorName = instructor?.FullName ?? "Unknown Instructor",
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Location = schedule.Location,
                CreatedAt = schedule.CreatedAt,
            };
        }
    }
}
