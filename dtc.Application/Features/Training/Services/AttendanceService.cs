using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Notifications.Interfaces;
using dtc.Application.Features.Email.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IEmailService emailService,
            ILogger<AttendanceService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<bool> MarkAttendanceAsync(MarkAttendanceRequestDto request, Guid adminId)
        {
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

            var existingAttendances = await _unitOfWork.Attendances.FindAsync(
                a => a.ClassScheduleId == request.ClassScheduleId && a.StudentId == request.StudentId);
            var existingAttendance = existingAttendances.FirstOrDefault();

            if (existingAttendance != null)
            {
                existingAttendance.UpdateAttendance(request.IsPresent, adminId);
                await _unitOfWork.Attendances.UpdateAsync(existingAttendance);
            }
            else
            {
                var newAttendance = new Attendance(
                    classScheduleId: request.ClassScheduleId,
                    studentId: request.StudentId,
                    isPresent: request.IsPresent,
                    createdBy: adminId
                );
                await _unitOfWork.Attendances.AddAsync(newAttendance);
            }

            await _unitOfWork.SaveChangesAsync();

            if (!request.IsPresent)
            {
                await HandleAbsentStudentAsync(schedule.ClassId, request.StudentId);
            }

            return true;
        }

        private async Task HandleAbsentStudentAsync(Guid classId, Guid studentId)
        {
            try
            {
                var student = await _unitOfWork.Users.GetByIdAsync(studentId);
                if (student == null)
                {
                    return;
                }

                await _notificationService.CreateForUserAsync(
                    student.Id,
                    "Thông báo vắng mặt",
                    "Bạn vừa được đánh dấu vắng mặt trong một buổi học.",
                    NotificationType.Attendance);

                var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == classId);
                var totalSessions = schedules.Count();

                if (totalSessions <= 0)
                {
                    return;
                }

                var allAttendances = new List<Attendance>();
                foreach (var scheduleId in schedules.Select(s => s.Id))
                {
                    var scheduleAttendances = await _unitOfWork.Attendances.FindAsync(
                        a => a.ClassScheduleId == scheduleId && a.StudentId == student.Id);
                    allAttendances.AddRange(scheduleAttendances);
                }

                var absentCount = allAttendances.Count(a => !a.IsPresent);
                var threshold = totalSessions * 0.2;

                if (absentCount >= threshold && totalSessions >= 5)
                {
                    await _notificationService.CreateForUserAsync(
                        student.Id,
                        "Cảnh báo nghỉ học",
                        $"Bạn đã nghỉ {absentCount}/{totalSessions} buổi. Vui lòng đi học đầy đủ để đảm bảo điều kiện thi.",
                        NotificationType.Attendance);

                    await _emailService.SendAsync(new Application.Features.Email.DTOs.SendEmailRequestDto
                    {
                        ToEmail = student.Email.Value,
                        Subject = "[DTC] Cảnh báo nghỉ học",
                        Body = $"Chào {student.FullName},<br/><br/>Bạn đã vắng mặt {absentCount} trên tổng số {totalSessions} buổi học. Nếu tiếp tục vắng mặt quá số buổi quy định, bạn sẽ không đủ điều kiện tham dự kỳ thi sắp tới.<br/><br/>Trân trọng,<br/>Đội ngũ DTC.",
                        IsHtml = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process absence notification/email for student {StudentId} in class {ClassId}.",
                    studentId,
                    classId);
            }
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
            var classSchedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == classId);
            if (classSchedules == null || !classSchedules.Any())
                return new { Message = "No schedules found for this class" };

            var classScheduleIds = classSchedules.Select(s => s.Id).ToList();

            var attendances = new List<Attendance>();
            foreach (var scheduleId in classScheduleIds)
            {
                var scheduleAttendances = await _unitOfWork.Attendances.FindAsync(a => a.ClassScheduleId == scheduleId);
                attendances.AddRange(scheduleAttendances);
            }

            var enrollments = await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId);
            var studentIds = enrollments.Select(e => e.StudentId).Distinct().ToList();
            if (studentIds.Count == 0)
                return new { Message = "No students enrolled in this class" };

            var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));

            var totalSessions = classScheduleIds.Count;

            var report = students.Select(student =>
            {
                var studentAttendances = attendances.Where(a => a.StudentId == student.Id).ToList();
                var presentCount = studentAttendances.Count(a => a.IsPresent);
                var absentCount = studentAttendances.Count(a => !a.IsPresent);
                var unrecordedCount = totalSessions - (presentCount + absentCount);

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

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByStudentAsync(Guid studentId)
        {
            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId);
            var dtos = new List<AttendanceResponseDto>();

            foreach (var a in attendances)
            {
                await _unitOfWork.ClassSchedules.GetByIdAsync(a.ClassScheduleId);
                dtos.Add(new AttendanceResponseDto
                {
                    Id = a.Id,
                    ClassScheduleId = a.ClassScheduleId,
                    StudentId = a.StudentId,
                    IsPresent = a.IsPresent,
                    CheckedAt = a.CheckedAt,
                });
            }

            return dtos;
        }

        public async Task<object> GetStudentAttendanceSummaryAsync(Guid studentId, Guid? classId = null)
        {
            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            if (student == null) throw new Exception("Student not found");

            IEnumerable<Guid> relevantScheduleIds;
            if (classId.HasValue)
            {
                var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.ClassId == classId.Value);
                relevantScheduleIds = schedules.Select(s => s.Id);
            }
            else
            {
                var enrollments = await _unitOfWork.ClassStudents.FindAsync(cs => cs.StudentId == studentId);
                var classIds = enrollments.Select(e => e.ClassId).ToList();
                var allSchedules = await _unitOfWork.ClassSchedules.FindAsync(s => classIds.Contains(s.ClassId));
                relevantScheduleIds = allSchedules.Select(s => s.Id);
            }

            var totalSessions = relevantScheduleIds.Count();
            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId && relevantScheduleIds.Contains(a.ClassScheduleId));

            var presentCount = attendances.Count(a => a.IsPresent);
            var absentCount = attendances.Count(a => !a.IsPresent);
            var rate = totalSessions > 0 ? Math.Round(((double)presentCount / totalSessions) * 100, 2) : 0;

            return new
            {
                StudentId = studentId,
                FullName = student.FullName,
                TotalSessions = totalSessions,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                AttendanceRate = rate
            };
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetMyAttendanceAsync(Guid studentId)
        {
            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId);
            var dtos = new List<AttendanceResponseDto>();

            foreach (var a in attendances)
            {
                var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(a.ClassScheduleId);
                var theClass = schedule != null ? await _unitOfWork.Classes.GetByIdAsync(schedule.ClassId) : null;

                dtos.Add(new AttendanceResponseDto
                {
                    Id = a.Id,
                    ClassScheduleId = a.ClassScheduleId,
                    StudentId = a.StudentId,
                    IsPresent = a.IsPresent,
                    CheckedAt = a.CheckedAt,
                    SessionDate = schedule?.StartTime,
                    SubjectName = theClass?.ClassName
                });
            }

            return dtos.OrderByDescending(d => d.CheckedAt);
        }

        public async Task<StudentAttendanceReportDto> GetMyAttendanceReportAsync(Guid studentId)
        {
            var report = new StudentAttendanceReportDto();

            var enrollments = await _unitOfWork.ClassStudents.FindAsync(cs => cs.StudentId == studentId);
            var classIds = enrollments.Select(e => e.ClassId).ToList();
            if (!classIds.Any()) return report;

            var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => classIds.Contains(s.ClassId));
            var scheduleIds = schedules.Select(s => s.Id).ToList();

            var attendances = await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId && scheduleIds.Contains(a.ClassScheduleId));
            var attendanceMap = attendances.ToDictionary(a => a.ClassScheduleId);

            var instructorIds = schedules.Select(s => s.InstructorId).Distinct().ToList();
            var instructors = await _unitOfWork.Users.FindAsync(u => instructorIds.Contains(u.Id));
            var instructorMap = instructors.ToDictionary(u => u.Id, u => u.FullName);

            var classes = await _unitOfWork.Classes.FindAsync(c => classIds.Contains(c.Id));
            var classMap = classes.ToDictionary(c => c.Id, c => c.ClassName);

            foreach (var s in schedules.OrderByDescending(s => s.StartTime))
            {
                var status = "Pending";
                if (attendanceMap.TryGetValue(s.Id, out var att))
                {
                    status = att.IsPresent ? "Present" : "Absent";
                }

                report.Sessions.Add(new StudentAttendanceSessionDto
                {
                    ScheduleId = s.Id,
                    Date = s.StartTime.Date,
                    StartTime = s.StartTime.ToString("HH:mm"),
                    EndTime = s.EndTime.ToString("HH:mm"),
                    LessonName = classMap.GetValueOrDefault(s.ClassId, "Unknown Lesson"),
                    InstructorName = instructorMap.GetValueOrDefault(s.InstructorId, "Unknown Instructor"),
                    Status = status
                });
            }

            report.Summary.TotalSessions = schedules.Count();
            report.Summary.PresentCount = attendances.Count(a => a.IsPresent);
            report.Summary.AbsentCount = attendances.Count(a => !a.IsPresent);
            report.Summary.AttendanceRate = report.Summary.TotalSessions > 0
                ? Math.Round(((double)report.Summary.PresentCount / report.Summary.TotalSessions) * 100, 2)
                : 0;

            return report;
        }
    }
}
