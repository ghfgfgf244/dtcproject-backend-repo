using dtc.Application.Features.Exams.Interfaces;
using dtc.Application.Features.Exams.DTOs;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dtc.Application.Features.Exams.Services
{
    public class ExamRegistrationService : IExamRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamRegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExamRegistrationResponseDto> RegisterAsync(CreateExamRegistrationRequestDto request)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(request.ExamBatchId);
            if (batch == null) throw new Exception("Không tìm thấy đợt thi đã chọn.");

            // Prevent registration if closed
            if (batch.Status != ExamBatchStatus.OpenForRegistration)
                throw new Exception("Đợt thi này hiện không mở đăng ký.");

            var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId);
            if (student == null) throw new Exception("Không tìm thấy học viên đã chọn.");

            var existingRegs = await _unitOfWork.ExamRegistrations.FindAsync(r => r.ExamBatchId == request.ExamBatchId && r.StudentId == request.StudentId);
            if (existingRegs != null && existingRegs.Any())
            {
                var existing = existingRegs.First();
                if (existing.Status != ExamRegistrationStatus.Cancelled && existing.Status != ExamRegistrationStatus.Rejected)
                    throw new Exception("Học viên này đã đăng ký ở đợt thi này rồi.");
            }

            var reg = new ExamRegistration(
                examBatchId: request.ExamBatchId,
                studentId: request.StudentId,
                isPaid: request.IsPaid,
                createdBy: request.StudentId
            );

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                batch.AddCandidate(request.StudentId);
                await _unitOfWork.ExamBatches.UpdateAsync(batch);
                await _unitOfWork.ExamRegistrations.AddAsync(reg);
                await _unitOfWork.SaveChangesAsync();
            });

            return await MapToDtoAsync(reg);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, UpdateExamRegistrationStatusRequestDto request, Guid adminId)
        {
            var reg = await _unitOfWork.ExamRegistrations.GetByIdAsync(id);
            if (reg == null) throw new Exception("Registration not found");
            var previousStatus = reg.Status;

            switch (request.Status)
            {
                case ExamRegistrationStatus.Approved:
                    reg.Approve(adminId);
                    // Could potentially auto-generate ExamResult placeholder here, but usually done right before exam
                    break;
                case ExamRegistrationStatus.Rejected:
                    reg.Reject(adminId);
                    break;
                case ExamRegistrationStatus.Cancelled:
                    reg.Cancel(adminId);
                    break;
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.ExamRegistrations.UpdateAsync(reg);

                if (previousStatus != request.Status &&
                    (request.Status == ExamRegistrationStatus.Cancelled || request.Status == ExamRegistrationStatus.Rejected))
                {
                    var batch = await _unitOfWork.ExamBatches.GetByIdAsync(reg.ExamBatchId);
                    if (batch != null)
                    {
                        batch.RemoveCandidate(adminId);
                        await _unitOfWork.ExamBatches.UpdateAsync(batch);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            });

            return true;
        }

        public async Task<bool> MarkAsPaidAsync(Guid id, Guid adminId)
        {
            return await UpdatePaymentStatusAsync(id, true, adminId);
        }

        public async Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid, Guid adminId)
        {
            var reg = await _unitOfWork.ExamRegistrations.GetByIdAsync(id);
            if (reg == null) throw new Exception("Registration not found");

            reg.SetPaymentStatus(isPaid, adminId);

            await _unitOfWork.ExamRegistrations.UpdateAsync(reg);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ExamRegistrationBatchPagedResponseDto> GetByExamBatchAsync(Guid examBatchId, ExamRegistrationBatchQueryDto query, Guid? centerId = null)
        {
            var pageNumber = Math.Max(1, query.PageNumber);
            var pageSize = Math.Max(1, query.PageSize);

            var regs = (await _unitOfWork.ExamRegistrations.FindAsync(r => r.ExamBatchId == examBatchId)).ToList();

            if (query.Status.HasValue)
            {
                regs = regs.Where(r => r.Status == query.Status.Value).ToList();
            }

            var examsInBatch = (await _unitOfWork.Exams.FindAsync(e => e.ExamBatchId == examBatchId && !e.IsDeleted)).ToList();
            var courseIdsInBatch = examsInBatch.Select(e => e.CourseId).Distinct().ToHashSet();

            var dtos = new List<ExamRegistrationResponseDto>();
            foreach (var r in regs)
            {
                dtos.Add(await MapToDtoAsync(r, courseIdsInBatch));
            }

            var orderedDtos = dtos
                .Where(item => !centerId.HasValue || item.CenterId == centerId.Value)
                .OrderByDescending(item => item.RegistrationDate)
                .ThenBy(item => item.StudentName)
                .ToList();

            var totalItems = orderedDtos.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedItems = orderedDtos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ExamRegistrationBatchPagedResponseDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                PendingCount = orderedDtos.Count(item => item.Status == ExamRegistrationStatus.Pending),
                EligibleCount = orderedDtos.Count(item => item.IsEligibleForApproval),
                Items = pagedItems
            };
        }

        public async Task<IEnumerable<TermExamRegistrationCandidateDto>> GetCandidatesByTermAsync(Guid termId, Guid examBatchId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null) throw new Exception("Không tìm thấy kỳ học đã chọn.");

            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(examBatchId);
            if (batch == null) throw new Exception("Không tìm thấy đợt thi đã chọn.");

            var registrations = (await _unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.AssignedTermId == termId &&
                    r.Status == CourseRegistrationStatus.Approved))
                .ToList();

            var studentIds = registrations.Select(r => r.UserId).Distinct().ToList();
            if (studentIds.Count == 0)
            {
                return new List<TermExamRegistrationCandidateDto>();
            }

            var existingExamRegistrations = (await _unitOfWork.ExamRegistrations.FindAsync(r =>
                    r.ExamBatchId == examBatchId &&
                    studentIds.Contains(r.StudentId) &&
                    r.Status != ExamRegistrationStatus.Cancelled &&
                    r.Status != ExamRegistrationStatus.Rejected))
                .ToList();

            var existingSet = existingExamRegistrations.Select(r => r.StudentId).ToHashSet();
            var students = (await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id))).ToList();
            var course = await _unitOfWork.Courses.GetByIdAsync(term.CourseId);

            var result = new List<TermExamRegistrationCandidateDto>();
            foreach (var student in students.OrderBy(s => s.FullName))
            {
                var metrics = await CalculateAttendanceMetricsAsync(termId, student.Id);
                result.Add(new TermExamRegistrationCandidateDto
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    Email = student.Email.Value,
                    Phone = student.Phone?.Value ?? string.Empty,
                    CourseName = course?.CourseName ?? string.Empty,
                    LicenseTypeLabel = course?.LicenseType.ToString() ?? string.Empty,
                    AttendanceRate = metrics.AttendanceRate,
                    TotalSessions = metrics.TotalSessions,
                    PresentCount = metrics.PresentCount,
                    IsEligibleForApproval = metrics.AttendanceRate >= 80,
                    AlreadyRegistered = existingSet.Contains(student.Id)
                });
            }

            return result;
        }

        public async Task<IEnumerable<ExamRegistrationResponseDto>> GetByStudentAsync(Guid studentId)
        {
            var regs = await _unitOfWork.ExamRegistrations.FindAsync(r => r.StudentId == studentId);
            var dtos = new List<ExamRegistrationResponseDto>();
            foreach (var r in regs)
            {
                dtos.Add(await MapToDtoAsync(r));
            }
            return dtos.OrderByDescending(r => r.RegistrationDate);
        }

        public async Task<bool> CreateBulkRegistrationsAsync(BulkExamRegistrationRequestDto request, Guid adminId)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(request.ExamBatchId);
            if (batch == null) throw new Exception("Không tìm thấy đợt thi đã chọn.");

            if (batch.Status != ExamBatchStatus.OpenForRegistration)
                throw new Exception("Đợt thi này hiện không mở đăng ký.");

            var studentIds = request.StudentIds.Distinct().ToList();
            var students = await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id));
            if (students.Count() != studentIds.Count)
                throw new Exception("Có ít nhất một học viên không tồn tại trong hệ thống.");

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingRegs = await _unitOfWork.ExamRegistrations.FindAsync(r => 
                    r.ExamBatchId == request.ExamBatchId && studentIds.Contains(r.StudentId));
                
                var activeByStudent = existingRegs
                    .Where(r => r.Status != ExamRegistrationStatus.Cancelled && r.Status != ExamRegistrationStatus.Rejected)
                    .Select(r => r.StudentId)
                    .ToHashSet();

                foreach (var studentId in studentIds)
                {
                    if (activeByStudent.Contains(studentId)) continue; // skip already registered

                    batch.AddCandidate(adminId);
                    var reg = new ExamRegistration(
                        examBatchId: request.ExamBatchId,
                        studentId: studentId,
                        isPaid: request.IsPaid,
                        createdBy: adminId
                    );
                    await _unitOfWork.ExamRegistrations.AddAsync(reg);
                }

                await _unitOfWork.ExamBatches.UpdateAsync(batch);
                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        private async Task<ExamRegistrationResponseDto> MapToDtoAsync(
            ExamRegistration reg,
            IReadOnlySet<Guid>? preferredCourseIds = null)
        {
            var batch = await _unitOfWork.ExamBatches.GetByIdAsync(reg.ExamBatchId);
            var student = await _unitOfWork.Users.GetByIdAsync(reg.StudentId);
            var approvedCourseRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.UserId == reg.StudentId &&
                    r.Status == CourseRegistrationStatus.Approved))
                .OrderByDescending(r => r.RegistrationDate)
                .ToList();

            var courseRegistration = preferredCourseIds is { Count: > 0 }
                ? approvedCourseRegistrations.FirstOrDefault(r => preferredCourseIds.Contains(r.CourseId))
                : approvedCourseRegistrations.FirstOrDefault();

            var term = courseRegistration?.AssignedTermId.HasValue == true
                ? await _unitOfWork.Terms.GetByIdAsync(courseRegistration.AssignedTermId.Value)
                : null;

            var course = courseRegistration != null
                ? await _unitOfWork.Courses.GetByIdAsync(courseRegistration.CourseId)
                : term != null
                    ? await _unitOfWork.Courses.GetByIdAsync(term.CourseId)
                    : null;

            var metrics = term != null
                ? await CalculateAttendanceMetricsAsync(term.Id, reg.StudentId)
                : new AttendanceMetrics();

            return new ExamRegistrationResponseDto
            {
                Id = reg.Id,
                ExamBatchId = reg.ExamBatchId,
                StudentId = reg.StudentId,
                CenterId = course?.CenterId,
                StudentName = student?.FullName ?? "Unknown",
                Email = student?.Email.Value ?? string.Empty,
                Phone = student?.Phone?.Value ?? string.Empty,
                BatchName = batch?.BatchName ?? "Unknown",
                TermId = term?.Id,
                TermName = term?.TermName,
                CourseName = course?.CourseName ?? string.Empty,
                LicenseTypeLabel = course?.LicenseType.ToString() ?? string.Empty,
                RegistrationDate = reg.RegistrationDate,
                IsPaid = reg.IsPaid,
                AttendanceRate = metrics.AttendanceRate,
                TotalSessions = metrics.TotalSessions,
                PresentCount = metrics.PresentCount,
                IsEligibleForApproval = reg.IsPaid && metrics.AttendanceRate >= 80,
                Status = reg.Status,
                CreatedAt = reg.CreatedAt
            };
        }

        private async Task<AttendanceMetrics> CalculateAttendanceMetricsAsync(Guid termId, Guid studentId)
        {
            var classes = (await _unitOfWork.Classes.FindAsync(c => c.TermId == termId && !c.IsDeleted)).ToList();
            if (classes.Count == 0)
            {
                return new AttendanceMetrics();
            }

            var classIds = classes.Select(c => c.Id).ToList();
            var studentClassIds = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.StudentId == studentId && classIds.Contains(cs.ClassId)))
                .Select(cs => cs.ClassId)
                .Distinct()
                .ToList();

            if (studentClassIds.Count == 0)
            {
                return new AttendanceMetrics();
            }

            var schedules = (await _unitOfWork.ClassSchedules.FindAsync(s => studentClassIds.Contains(s.ClassId))).ToList();
            if (schedules.Count == 0)
            {
                return new AttendanceMetrics();
            }

            var scheduleIds = schedules.Select(s => s.Id).ToList();
            var attendances = (await _unitOfWork.Attendances.FindAsync(a => a.StudentId == studentId && scheduleIds.Contains(a.ClassScheduleId))).ToList();

            var totalSessions = schedules.Count;
            var presentCount = attendances.Count(a => a.IsPresent);
            var attendanceRate = totalSessions > 0
                ? Math.Round((double)presentCount * 100 / totalSessions, 2)
                : 0;

            return new AttendanceMetrics
            {
                TotalSessions = totalSessions,
                PresentCount = presentCount,
                AttendanceRate = attendanceRate
            };
        }

        private sealed class AttendanceMetrics
        {
            public int TotalSessions { get; set; }
            public int PresentCount { get; set; }
            public double AttendanceRate { get; set; }
        }
    }
}
