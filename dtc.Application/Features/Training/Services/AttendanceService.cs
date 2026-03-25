using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Email.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Training.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public AttendanceService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        public async Task<bool> MarkAttendanceAsync(MarkAttendanceRequestDto request, Guid adminId)
        {
            // Verify schedule exists
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(request.ClassScheduleId);
            if (schedule == null)
                throw new Exception("Class schedule not found");

            var classExists = await _unitOfWork.Classes.AnyAsync(c => c.Id == schedule.ClassId);
            if (!classExists)
                throw new Exception("Class not found");

            var isEnrolled = await _unitOfWork.ClassStudents.AnyAsync(
                cs => cs.ClassId == schedule.ClassId && cs.StudentId == request.StudentId);
            if (!isEnrolled)
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

            // Side-effect: Notify student if absent
            if (!request.IsPresent)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId);
                        if (student != null)
                        {
                            await _notificationService.CreateForUserAsync(
                                student.Id,
                                "Thông báo vắng mặt",
                                "Bạn vừa được đánh dấu vắng mặt trong một buổi học.",
                                NotificationType.Attendance);

                            // Check absolute limit (20% of total sessions)
                            var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == schedule.ClassId);
                            var totalSessions = schedules.Count();
                            
                            if (totalSessions > 0)
                            {
                                var allAttendances = new List<Attendance>();
                                foreach(var sId in schedules.Select(s => s.Id))
                                {
                                     var sAttendances = await _unitOfWork.Attendances.FindAsync(a => a.ClassScheduleId == sId && a.StudentId == student.Id);
                                     allAttendances.AddRange(sAttendances);
                                }
                                
                                var absentCount = allAttendances.Count(a => !a.IsPresent);
                                var threshold = totalSessions * 0.2;

                                if (absentCount >= threshold && totalSessions >= 5) // Warning if >= 20% and enough sessions to be meaningful
                                {
                                    await _notificationService.CreateForUserAsync(
                                        student.Id,
                                        "Cảnh báo nghỉ học",
                                        $"Bạn đã nghỉ {absentCount}/{totalSessions} buổi. Vui lòng đi học đầy đủ để đảm bảo điều kiện thi.",
                                        NotificationType.Attendance);
                                    
                                    // Assuming email service has a generic warning or we can use existing one
                                    await _emailService.SendAsync(new Application.Features.Email.DTOs.SendEmailRequestDto
                                    {
                                        ToEmail = student.Email.Value,
                                        Subject = "[DTC] Cảnh báo nghỉ học",
                                        Body = $"Chào {student.FullName},<br/><br/>Bạn đã vắng mặt {absentCount} trên tổng số {totalSessions} buổi học. Nếu tiếp tục vắng mặt quá số buổi quy định, bạn sẽ không đủ điều kiện tham dự kỳ thi sắp tới.<br/><br/>Trân trọng,<br/>Đội ngũ DTC."
                                    });
                                }
                            }
                        }
                    }
                    catch { /* logic cảnh báo không được block luồng chính */ }
                });
            }

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

            var enrollments = await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId);
            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            if (studentIds.Count == 0)
                return new { Message = "No students enrolled in this class" };

            var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));

            var totalSessions = classScheduleIds.Count;

            // 4. Group by student
            var report = students.Select(student =>
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

            var theClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            return new
            {
                ClassId = classId,
                ClassName = theClass?.ClassName ?? "Unknown",
                TotalSessions = totalSessions,
                StudentReports = report
            };
        }
    }
}
