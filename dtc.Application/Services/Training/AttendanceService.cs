using dtc.Application.DTOs.Training.Attendances;
using dtc.Application.Interfaces.Training;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Services.Training
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> MarkAttendanceAsync(MarkAttendanceRequestDto request, Guid adminId)
        {
            // Verify schedule exists
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(request.ClassScheduleId);
            if (schedule == null)
                throw new Exception("Class schedule not found");

            // Verify student is in class
            var classEntity = await _unitOfWork.Classes.FindAsync(c => c.Id == schedule.ClassId, c => c.Students);
            var theClass = classEntity.FirstOrDefault();
            if (theClass == null)
                throw new Exception("Class not found");

            if (!theClass.Students.Any(s => s.Id == request.StudentId))
                throw new Exception("Student is not enrolled in this class");

            // Check if attendance already exists
            var existingAttendances = await _unitOfWork.Attendances.FindAsync(
                a => a.ClassScheduleId == request.ClassScheduleId && a.StudentId == request.StudentId);
            var existingAttendance = existingAttendances.FirstOrDefault();

            if (existingAttendance != null)
            {
                // Update
                existingAttendance.UpdateAttendance(request.IsPresent, adminId);
                await _unitOfWork.Attendances.UpdateAsync(existingAttendance);
            }
            else
            {
                // Create new
                var newAttendance = new Attendance(
                    classScheduleId: request.ClassScheduleId,
                    studentId: request.StudentId,
                    isPresent: request.IsPresent,
                    createdBy: adminId
                );
                await _unitOfWork.Attendances.AddAsync(newAttendance);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByClassScheduleAsync(Guid classScheduleId)
        {
            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.ClassScheduleId == classScheduleId);
            if (attendances == null || !attendances.Any())
                return new List<AttendanceResponseDto>();

            var dtos = new List<AttendanceResponseDto>();
            foreach (var a in attendances)
            {
                var student = await _unitOfWork.Users.GetByIdAsync(a.StudentId);
                dtos.Add(new AttendanceResponseDto
                {
                    Id = a.Id,
                    ClassScheduleId = a.ClassScheduleId,
                    StudentId = a.StudentId,
                    StudentName = student?.FullName ?? "Unknown",
                    IsPresent = a.IsPresent,
                    CheckedAt = a.CheckedAt
                });
            }

            return dtos;
        }

        public async Task<object> GetAttendanceReportByClassAsync(Guid classId)
        {
            // 1. Get all schedules for the class
            var classSchedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == classId);
            if (classSchedules == null || !classSchedules.Any())
                return new { Message = "No schedules found for this class" };

            var classScheduleIds = classSchedules.Select(s => s.Id).ToList();

            // 2. Get all attendances for these schedules
            var attendances = new List<Attendance>();
            foreach(var sId in classScheduleIds)
            {
                 var sAttendances = await _unitOfWork.Attendances.FindAsync(a => a.ClassScheduleId == sId);
                 attendances.AddRange(sAttendances);
            }

            // 3. Get students in the class
            var classEntityList = await _unitOfWork.Classes.FindAsync(c => c.Id == classId, c => c.Students);
            var classEntity = classEntityList.FirstOrDefault();
            if (classEntity == null) return new { Message = "Class not found" };

            var totalSessions = classScheduleIds.Count;

            // 4. Group by student
            var report = classEntity.Students.Select(student =>
            {
                var studentAttendances = attendances.Where(a => a.StudentId == student.Id).ToList();
                var presentCount = studentAttendances.Count(a => a.IsPresent);
                var absentCount = studentAttendances.Count(a => !a.IsPresent); // explicitly marked absent
                var unrecordedCount = totalSessions - (presentCount + absentCount); // not yet marked

                return new
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    TotalSessions = totalSessions,
                    PresentCount = presentCount,
                    AbsentCount = absentCount,
                    UnrecordedCount = unrecordedCount,
                    AttendanceRate = totalSessions > 0 ? Math.Round(((double)presentCount / totalSessions) * 100, 2) : 0
                };
            }).ToList();

            return new
            {
                ClassId = classId,
                ClassName = classEntity.ClassName,
                TotalSessions = totalSessions,
                StudentReports = report
            };
        }
    }
}
