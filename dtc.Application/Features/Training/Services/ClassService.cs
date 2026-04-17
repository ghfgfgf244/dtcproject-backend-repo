using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Application.Features.AI.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Terms;
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
        private readonly IAiRouterService _aiRouterService;

        public ClassService(IUnitOfWork unitOfWork, IAiRouterService aiRouterService)
        {
            _unitOfWork = unitOfWork;
            _aiRouterService = aiRouterService;
        }

        public async Task<ClassResponseDto> CreateClassAsync(CreateClassRequestDto request, Guid adminId)
        {
            var term = await GetActiveTermOrThrowAsync(request.TermId);
            var instructor = await GetInstructorOrThrowAsync(request.InstructorId);

            var newClass = new Class(
                request.TermId,
                instructor.Id,
                request.ClassName,
                request.ClassType,
                request.MaxStudents,
                adminId);

            await _unitOfWork.Classes.AddAsync(newClass);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoAsync(newClass, term);
        }

        public async Task<ClassResponseDto> UpdateClassAsync(Guid classId, UpdateClassRequestDto request, Guid adminId)
        {
            var existingClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (existingClass == null)
                throw new Exception("Class not found");

            var changed = existingClass.UpdateInfo(request.ClassName, request.ClassType, request.MaxStudents, adminId);

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

            return await MapToDtoAsync(existingClass);
        }

        public async Task<IEnumerable<ClassResponseDto>> GetAllClassesAsync()
        {
            var classes = (await _unitOfWork.Classes.FindAsync(c => !c.IsDeleted))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var results = new List<ClassResponseDto>();
            foreach (var classEntity in classes)
            {
                results.Add(await MapToDtoAsync(classEntity));
            }

            return results;
        }

        public async Task<ClassResponseDto> GetClassDetailAsync(Guid classId)
        {
            var existingClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (existingClass == null || existingClass.IsDeleted)
                throw new KeyNotFoundException("Class not found");

            return await MapToDtoAsync(existingClass);
        }

        public async Task<bool> DeleteClassAsync(Guid classId, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
                throw new Exception("Class not found");

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
            if (classEntity == null)
                throw new Exception("Class not found");

            var instructorId = request.InstructorIds.FirstOrDefault();
            if (instructorId == Guid.Empty)
                throw new InvalidOperationException("Instructor is required.");

            await GetInstructorOrThrowAsync(instructorId);

            classEntity.ChangeInstructor(instructorId, adminId);
            await _unitOfWork.Classes.UpdateAsync(classEntity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AssignStudentsToClassAsync(Guid classId, AssignStudentsRequestDto request, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
                throw new Exception("Class not found");

            var distinctIds = (request.StudentIds ?? new List<Guid>()).Distinct().ToList();
            if (distinctIds.Count > classEntity.MaxStudents)
                throw new InvalidOperationException("Too many students for this class capacity.");

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await ValidateStudentsBelongToTermAsync(classEntity.TermId, distinctIds);

                var existing = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId)).ToList();
                var existingSet = existing.Select(e => e.StudentId).ToHashSet();
                var requestedSet = distinctIds.ToHashSet();

                var termClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == classEntity.TermId && !c.IsDeleted)).ToList();
                var sameTypeClassIds = termClasses
                    .Where(c => c.Id != classId && c.ClassType == classEntity.ClassType)
                    .Select(c => c.Id)
                    .ToList();

                if (sameTypeClassIds.Count > 0)
                {
                    var duplicates = (await _unitOfWork.ClassStudents.FindAsync(cs => sameTypeClassIds.Contains(cs.ClassId) && requestedSet.Contains(cs.StudentId)))
                        .Select(cs => cs.StudentId)
                        .Distinct()
                        .ToList();

                    if (duplicates.Count > 0)
                        throw new InvalidOperationException($"One or more students are already assigned to another {classEntity.ClassType} class in this term.");
                }

                var toRemove = existing.Where(e => !requestedSet.Contains(e.StudentId)).ToList();
                if (toRemove.Count > 0)
                    await _unitOfWork.ClassStudents.RemoveRange(toRemove);

                var toAdd = distinctIds.Where(id => !existingSet.Contains(id)).ToList();
                foreach (var studentId in toAdd)
                {
                    await _unitOfWork.ClassStudents.AddAsync(new ClassStudent(classId, studentId));
                }

                classEntity.SyncEnrollmentCount(distinctIds.Count, adminId);
                await _unitOfWork.Classes.UpdateAsync(classEntity);
                return true;
            });
        }

        public async Task<bool> RemoveStudentFromClassAsync(Guid classId, Guid studentId, Guid adminId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
                throw new Exception("Class not found");

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var enrollment = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId && cs.StudentId == studentId))
                    .FirstOrDefault();

                if (enrollment == null)
                    throw new InvalidOperationException("Student is not assigned to this class.");

                await _unitOfWork.ClassStudents.RemoveAsync(enrollment);
                classEntity.RemoveStudent(adminId);
                await _unitOfWork.Classes.UpdateAsync(classEntity);
                return true;
            });
        }

        public async Task<bool> TransferStudentAsync(Guid classId, Guid studentId, TransferStudentRequestDto request, Guid adminId)
        {
            if (request.TargetClassId == Guid.Empty)
                throw new InvalidOperationException("Target class is required.");
            if (request.TargetClassId == classId)
                throw new InvalidOperationException("Target class must be different from current class.");

            var currentClass = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (currentClass == null || currentClass.IsDeleted)
                throw new KeyNotFoundException("Current class not found.");

            var targetClass = await _unitOfWork.Classes.GetByIdAsync(request.TargetClassId);
            if (targetClass == null || targetClass.IsDeleted)
                throw new KeyNotFoundException("Target class not found.");

            var currentTerm = await GetActiveTermOrThrowAsync(currentClass.TermId);
            var targetTerm = await GetActiveTermOrThrowAsync(targetClass.TermId);

            if (currentClass.TermId != targetClass.TermId)
                throw new InvalidOperationException("Students can only be transferred within the same term.");

            if (currentTerm.CourseId != targetTerm.CourseId)
                throw new InvalidOperationException("Students can only be transferred to classes in the same course.");

            if (currentClass.ClassType != targetClass.ClassType)
                throw new InvalidOperationException("Students can only be transferred between classes of the same type.");

            if (targetClass.Status == ClassStatus.Cancelled || targetClass.Status == ClassStatus.Completed)
                throw new InvalidOperationException("Target class is not available for transfer.");

            var student = await _unitOfWork.Users.GetByIdAsync(studentId);
            if (student == null || student.RoleId != UserRole.Student)
                throw new InvalidOperationException("Only students can be transferred.");

            var currentEnrollment = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId && cs.StudentId == studentId))
                .FirstOrDefault();
            if (currentEnrollment == null)
                throw new InvalidOperationException("Student is not assigned to the current class.");

            var targetEnrollment = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == request.TargetClassId && cs.StudentId == studentId))
                .FirstOrDefault();
            if (targetEnrollment != null)
                throw new InvalidOperationException("Student is already assigned to the target class.");

            if (targetClass.CurrentStudents >= targetClass.MaxStudents)
                throw new InvalidOperationException("Target class is full.");

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.ClassStudents.RemoveAsync(currentEnrollment);
                await _unitOfWork.ClassStudents.AddAsync(new ClassStudent(targetClass.Id, studentId));

                currentClass.RemoveStudent(adminId);
                targetClass.EnrollStudent(adminId);

                await _unitOfWork.Classes.UpdateAsync(currentClass);
                await _unitOfWork.Classes.UpdateAsync(targetClass);
                return true;
            });
        }

        public async Task<IEnumerable<ClassResponseDto>> GetClassesByInstructorAsync(Guid instructorId)
        {
            var classes = (await _unitOfWork.Classes.FindAsync(c => c.InstructorId == instructorId && !c.IsDeleted))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var results = new List<ClassResponseDto>();
            foreach (var classEntity in classes)
            {
                results.Add(await MapToDtoAsync(classEntity));
            }

            return results;
        }

        public async Task<IEnumerable<ClassStudentResponseDto>> GetClassStudentsAsync(Guid classId)
        {
            var enrollment = (await _unitOfWork.ClassStudents.FindAsync(cs => cs.ClassId == classId)).ToList();
            if (enrollment.Count == 0)
                return new List<ClassStudentResponseDto>();

            var studentIds = enrollment.Select(e => e.StudentId).ToList();
            return await MapStudentsAsync(studentIds);
        }

        public async Task<IEnumerable<ClassStudentResponseDto>> GetAvailableStudentsAsync(Guid classId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
                throw new KeyNotFoundException("Class not found");

            var assignedRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.AssignedTermId == classEntity.TermId &&
                    r.Status == CourseRegistrationStatus.Approved))
                .ToList();

            var candidateStudentIds = assignedRegistrations.Select(r => r.UserId).Distinct().ToList();
            if (candidateStudentIds.Count == 0)
                return new List<ClassStudentResponseDto>();

            var currentTermClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == classEntity.TermId && !c.IsDeleted)).ToList();
            var sameTypeClassIds = currentTermClasses
                .Where(c => c.ClassType == classEntity.ClassType)
                .Select(c => c.Id)
                .ToList();

            var alreadyAssignedIds = sameTypeClassIds.Count == 0
                ? new HashSet<Guid>()
                : (await _unitOfWork.ClassStudents.FindAsync(cs => sameTypeClassIds.Contains(cs.ClassId)))
                    .Select(cs => cs.StudentId)
                    .ToHashSet();

            var availableIds = candidateStudentIds.Where(id => !alreadyAssignedIds.Contains(id)).ToList();
            return await MapStudentsAsync(availableIds);
        }

        public async Task<AutoAssignClassesResponseDto> AutoAssignClassesAsync(AutoAssignClassesRequestDto request, Guid adminId)
        {
            var plan = await BuildAutoAssignPlanAsync(request);
            if (!plan.CanAssign)
            {
                return new AutoAssignClassesResponseDto
                {
                    Message = plan.Message,
                    EligibleStudents = plan.EligibleStudentIds.Count,
                    CreatedClasses = 0,
                    TargetClassSize = plan.TargetSize,
                    MinSuggestedSize = plan.MinSize,
                    MaxSuggestedSize = plan.MaxSize,
                    Explanation = plan.Explanation,
                    Model = plan.Model,
                    Notes = plan.Notes
                };
            }

            var createdClasses = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var created = new List<Class>();
                var baseSequence = plan.ExistingClasses.Count(c => c.ClassType == request.ClassType);
                var studentCursor = 0;

                for (var i = 0; i < plan.Distributions.Count; i++)
                {
                    var size = plan.Distributions[i];
                    var assignedStudents = plan.EligibleStudentIds.Skip(studentCursor).Take(size).ToList();
                    studentCursor += size;

                    var instructor = plan.Instructors[i % plan.Instructors.Count];
                    var classEntity = new Class(
                        plan.Term.Id,
                        instructor.Id,
                        BuildAutoClassName(plan.Course.CourseName, plan.Term.TermName, request.ClassType, baseSequence + i + 1),
                        request.ClassType,
                        Math.Max(plan.TargetSize, size),
                        adminId);

                    if (assignedStudents.Count > 0)
                    {
                        classEntity.SyncEnrollmentCount(assignedStudents.Count, adminId);
                    }

                    await _unitOfWork.Classes.AddAsync(classEntity);

                    foreach (var studentId in assignedStudents)
                    {
                        await _unitOfWork.ClassStudents.AddAsync(new ClassStudent(classEntity.Id, studentId));
                    }

                    created.Add(classEntity);
                }

                return created;
            });

            var responseClasses = new List<ClassResponseDto>();
            foreach (var createdClass in createdClasses)
            {
                responseClasses.Add(await MapToDtoAsync(createdClass, plan.Term, plan.Course, plan.Center));
            }

            return new AutoAssignClassesResponseDto
            {
                Message = $"Created {responseClasses.Count} class(es) and assigned {plan.EligibleStudentIds.Count} student(s).",
                EligibleStudents = plan.EligibleStudentIds.Count,
                CreatedClasses = responseClasses.Count,
                TargetClassSize = plan.TargetSize,
                MinSuggestedSize = plan.MinSize,
                MaxSuggestedSize = plan.MaxSize,
                Explanation = plan.Explanation,
                Model = plan.Model,
                Notes = plan.Notes,
                Classes = responseClasses
            };
        }

        public async Task<AutoAssignClassesExplainResponseDto> PreviewAutoAssignClassesAsync(AutoAssignClassesRequestDto request)
        {
            var plan = await BuildAutoAssignPlanAsync(request);
            return new AutoAssignClassesExplainResponseDto
            {
                Message = plan.Message,
                EligibleStudents = plan.EligibleStudentIds.Count,
                PlannedClassCount = plan.PlannedClasses.Count,
                TargetClassSize = plan.TargetSize,
                MinSuggestedSize = plan.MinSize,
                MaxSuggestedSize = plan.MaxSize,
                Explanation = plan.Explanation,
                Model = plan.Model,
                Notes = plan.Notes,
                Classes = plan.PlannedClasses
            };
        }

        private async Task<AutoAssignPlan> BuildAutoAssignPlanAsync(AutoAssignClassesRequestDto request)
        {
            var term = await GetActiveTermOrThrowAsync(request.TermId);
            var course = await _unitOfWork.Courses.GetByIdAsync(term.CourseId)
                ?? throw new InvalidOperationException("Course not found for this term.");
            var center = await _unitOfWork.Centers.GetByIdAsync(course.CenterId)
                ?? throw new InvalidOperationException("Center not found for this term.");

            var approvedRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.AssignedTermId == term.Id &&
                    r.Status == CourseRegistrationStatus.Approved))
                .OrderBy(r => r.RegistrationDate)
                .ToList();

            var studentIds = approvedRegistrations.Select(r => r.UserId).Distinct().ToList();
            var termClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == term.Id && !c.IsDeleted)).ToList();
            var sameTypeClassIds = termClasses
                .Where(c => c.ClassType == request.ClassType)
                .Select(c => c.Id)
                .ToList();
            var alreadyAssignedIds = sameTypeClassIds.Count == 0
                ? new HashSet<Guid>()
                : (await _unitOfWork.ClassStudents.FindAsync(cs => sameTypeClassIds.Contains(cs.ClassId)))
                    .Select(cs => cs.StudentId)
                    .ToHashSet();

            var eligibleStudentIds = studentIds.Where(id => !alreadyAssignedIds.Contains(id)).ToList();

            var instructors = await GetAvailableInstructorsForCenterAsync(course.CenterId);
            var targetSize = request.PreferredClassSize.GetValueOrDefault() > 0
                ? request.PreferredClassSize!.Value
                : center.MaxStudentPerClass > 0 ? center.MaxStudentPerClass : Math.Max(1, course.MaxStudents);

            var tolerancePercent = request.TolerancePercent.GetValueOrDefault(10);
            tolerancePercent = Math.Clamp(tolerancePercent, 0, 50);

            var minSize = Math.Max(1, (int)Math.Floor(targetSize * (1 - tolerancePercent / 100m)));
            var maxSize = Math.Max(targetSize, (int)Math.Ceiling(targetSize * (1 + tolerancePercent / 100m)));

            if (studentIds.Count == 0)
            {
                return new AutoAssignPlan
                {
                    CanAssign = false,
                    Message = "No approved students were found in this term to auto-assign.",
                    Explanation = "Term này hiện chưa có học viên đủ điều kiện để hệ thống chia lớp tự động.",
                    Model = "rule-based",
                    Notes = BuildAutoAssignNotes(request, targetSize, tolerancePercent, minSize, maxSize),
                    Term = term,
                    Course = course,
                    Center = center,
                    ExistingClasses = termClasses,
                    Instructors = instructors,
                    TargetSize = targetSize,
                    MinSize = minSize,
                    MaxSize = maxSize
                };
            }

            if (eligibleStudentIds.Count == 0)
            {
                return new AutoAssignPlan
                {
                    CanAssign = false,
                    Message = "All students in this term are already assigned to classes.",
                    Explanation = "Tất cả học viên đã được gán vào lớp trong kỳ này, nên hiện chưa cần chia thêm lớp mới.",
                    Model = "rule-based",
                    Notes = BuildAutoAssignNotes(request, targetSize, tolerancePercent, minSize, maxSize),
                    Term = term,
                    Course = course,
                    Center = center,
                    ExistingClasses = termClasses,
                    Instructors = instructors,
                    EligibleStudentIds = eligibleStudentIds,
                    TargetSize = targetSize,
                    MinSize = minSize,
                    MaxSize = maxSize
                };
            }

            if (instructors.Count == 0)
                throw new InvalidOperationException("No active instructors were found for this center.");

            var classCount = Math.Max(1, (int)Math.Ceiling(eligibleStudentIds.Count / (double)maxSize));
            var distributions = BuildDistributions(eligibleStudentIds.Count, classCount);
            var previewClasses = new List<AutoAssignPreviewClassDto>();
            var baseSequence = termClasses.Count(c => c.ClassType == request.ClassType);

            for (var i = 0; i < distributions.Count; i++)
            {
                var size = distributions[i];
                var instructor = instructors[i % instructors.Count];
                var maxStudents = Math.Max(targetSize, size);
                previewClasses.Add(new AutoAssignPreviewClassDto
                {
                    ClassName = BuildAutoClassName(course.CourseName, term.TermName, request.ClassType, baseSequence + i + 1),
                    ClassType = request.ClassType.ToString(),
                    InstructorId = instructor.Id,
                    InstructorName = instructor.FullName,
                    StudentCount = size,
                    SuggestedMaxStudents = maxStudents,
                    OccupancyRate = maxStudents <= 0 ? 0 : Math.Round(size / (double)maxStudents, 4)
                });
            }

            var notes = BuildAutoAssignNotes(request, targetSize, tolerancePercent, minSize, maxSize);
            var explanation = await BuildAutoAssignExplanationAsync(
                term,
                course,
                eligibleStudentIds.Count,
                targetSize,
                minSize,
                maxSize,
                previewClasses,
                notes);

            return new AutoAssignPlan
            {
                CanAssign = true,
                Message = $"Preview generated for {previewClasses.Count} class(es).",
                Explanation = explanation.summary,
                Model = explanation.model,
                Notes = notes,
                Term = term,
                Course = course,
                Center = center,
                ExistingClasses = termClasses,
                Instructors = instructors,
                EligibleStudentIds = eligibleStudentIds,
                Distributions = distributions,
                PlannedClasses = previewClasses,
                TargetSize = targetSize,
                MinSize = minSize,
                MaxSize = maxSize
            };
        }

        private async Task<(string summary, string model)> BuildAutoAssignExplanationAsync(
            Term term,
            dtc.Domain.Entities.Training.Course course,
            int eligibleStudents,
            int targetSize,
            int minSize,
            int maxSize,
            IReadOnlyCollection<AutoAssignPreviewClassDto> previewClasses,
            IReadOnlyCollection<string> notes)
        {
            var fallback = $"Hệ thống đang chia {eligibleStudents} học viên của kỳ {term.TermName} thành {previewClasses.Count} lớp, cân bằng quanh sĩ số mục tiêu {targetSize} học viên/lớp và cho phép dao động trong khoảng {minSize}-{maxSize}. Phân công giảng viên được xoay vòng theo danh sách giảng viên đang khả dụng của trung tâm để tránh dồn tải vào một người.";

            try
            {
                var prompt =
                    "Ban la tro ly dieu phoi dao tao. " +
                    $"Hay giai thich ngan gon bang tieng Viet vi sao he thong chia {eligibleStudents} hoc vien cua khoa {course.CourseName} / ky {term.TermName} thanh {previewClasses.Count} lop. " +
                    $"Si so muc tieu: {targetSize}, khoang cho phep: {minSize}-{maxSize}. " +
                    $"Ke hoach lop: {string.Join("; ", previewClasses.Select(item => $"{item.ClassName}-{item.InstructorName}-{item.StudentCount} hoc vien"))}. " +
                    $"Ghi chu: {string.Join("; ", notes)}. " +
                    "Tra ve 3-4 cau, khong dung markdown.";

                var aiResult = await _aiRouterService.GenerateAsync("class-auto-assign-explain", prompt);
                return (
                    string.IsNullOrWhiteSpace(aiResult.Content) ? fallback : aiResult.Content.Trim(),
                    string.IsNullOrWhiteSpace(aiResult.Model) ? "rule-based" : aiResult.Model
                );
            }
            catch
            {
                return (fallback, "rule-based");
            }
        }

        private static List<string> BuildAutoAssignNotes(
            AutoAssignClassesRequestDto request,
            int targetSize,
            int tolerancePercent,
            int minSize,
            int maxSize)
        {
            var notes = new List<string>
            {
                $"Si so muc tieu: {targetSize} hoc vien/lop.",
                $"Dung sai cho phep: {tolerancePercent}%, tuong duong {minSize}-{maxSize} hoc vien/lop."
            };

            if (!string.IsNullOrWhiteSpace(request.PreferredShift))
            {
                notes.Add($"Ca hoc uu tien: {request.PreferredShift} (hien dang dung de giai thich, chua rang buoc lich cu the).");
            }

            return notes;
        }

        private async Task<Term> GetActiveTermOrThrowAsync(Guid termId)
        {
            var term = await _unitOfWork.Terms.GetByIdAsync(termId);
            if (term == null || term.IsDeleted)
                throw new KeyNotFoundException("Term not found");
            if (!term.IsActive)
                throw new InvalidOperationException("Cannot create or assign classes in an inactive term.");

            return term;
        }

        private async Task<dtc.Domain.Entities.Permissions.User> GetInstructorOrThrowAsync(Guid instructorId)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(instructorId);
            if (instructor == null || instructor.RoleId != UserRole.Instructor)
                throw new InvalidOperationException("Instructor not found.");

            return instructor;
        }

        private async Task ValidateStudentsBelongToTermAsync(Guid termId, IEnumerable<Guid> studentIds)
        {
            var ids = studentIds.Distinct().ToList();
            if (ids.Count == 0)
                return;

            var approvedRegistrations = (await _unitOfWork.CourseRegistrations.FindAsync(r =>
                    r.AssignedTermId == termId &&
                    r.Status == CourseRegistrationStatus.Approved &&
                    ids.Contains(r.UserId)))
                .ToList();

            var approvedIds = approvedRegistrations.Select(r => r.UserId).ToHashSet();
            var invalidIds = ids.Where(id => !approvedIds.Contains(id)).ToList();
            if (invalidIds.Count > 0)
                throw new InvalidOperationException("One or more students are not assigned to this term.");
        }

        private async Task<List<dtc.Domain.Entities.Permissions.User>> GetAvailableInstructorsForCenterAsync(Guid centerId)
        {
            var userCenters = (await _unitOfWork.UserCenters.FindAsync(uc => uc.CenterId == centerId)).ToList();
            var userIds = userCenters.Select(uc => uc.UserId).ToList();

            var centerInstructors = userIds.Count == 0
                ? new List<dtc.Domain.Entities.Permissions.User>()
                : (await _unitOfWork.Users.FindAsync(u => userIds.Contains(u.Id) && u.RoleId == UserRole.Instructor && u.IsActive)).ToList();

            if (centerInstructors.Count > 0)
                return centerInstructors;

            return (await _unitOfWork.Users.FindAsync(u => u.RoleId == UserRole.Instructor && u.IsActive)).ToList();
        }

        private static List<int> BuildDistributions(int totalStudents, int classCount)
        {
            var distributions = new List<int>();
            var baseSize = totalStudents / classCount;
            var remainder = totalStudents % classCount;

            for (var i = 0; i < classCount; i++)
            {
                distributions.Add(baseSize + (i < remainder ? 1 : 0));
            }

            return distributions.Where(size => size > 0).ToList();
        }

        private static string BuildAutoClassName(string courseName, string termName, ClassType classType, int sequence)
        {
            var courseToken = new string(courseName.Where(char.IsLetterOrDigit).Take(6).ToArray());
            if (string.IsNullOrWhiteSpace(courseToken))
                courseToken = "CLASS";

            var termToken = new string(termName.Where(char.IsLetterOrDigit).Take(8).ToArray());
            if (string.IsNullOrWhiteSpace(termToken))
                termToken = "TERM";

            var typeToken = classType == ClassType.Theory ? "LT" : "TH";
            return $"{courseToken}-{termToken}-{typeToken}-{sequence:D2}";
        }

        private sealed class AutoAssignPlan
        {
            public bool CanAssign { get; set; }
            public string Message { get; set; } = string.Empty;
            public string Explanation { get; set; } = string.Empty;
            public string Model { get; set; } = "rule-based";
            public List<string> Notes { get; set; } = new();
            public Term Term { get; set; } = default!;
            public dtc.Domain.Entities.Training.Course Course { get; set; } = default!;
            public dtc.Domain.Entities.Location.Center Center { get; set; } = default!;
            public List<Class> ExistingClasses { get; set; } = new();
            public List<dtc.Domain.Entities.Permissions.User> Instructors { get; set; } = new();
            public List<Guid> EligibleStudentIds { get; set; } = new();
            public List<int> Distributions { get; set; } = new();
            public List<AutoAssignPreviewClassDto> PlannedClasses { get; set; } = new();
            public int TargetSize { get; set; }
            public int MinSize { get; set; }
            public int MaxSize { get; set; }
        }

        private async Task<List<ClassStudentResponseDto>> MapStudentsAsync(List<Guid> studentIds)
        {
            if (studentIds.Count == 0)
                return new List<ClassStudentResponseDto>();

            var students = (await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id) && u.RoleId == UserRole.Student)).ToList();
            var distances = (await _unitOfWork.StudentDrivingDistances.FindAsync(d => studentIds.Contains(d.StudentId)))
                .ToDictionary(d => d.StudentId);

            var result = new List<ClassStudentResponseDto>();
            foreach (var student in students.OrderBy(s => s.FullName))
            {
                distances.TryGetValue(student.Id, out var dist);
                result.Add(new ClassStudentResponseDto
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email.Value,
                    Phone = student.Phone?.Value ?? string.Empty,
                    AvatarUrl = student.AvatarUrl,
                    IsActive = student.IsActive,
                    LastLoginAt = student.LastLoginAt,
                    Roles = new List<string> { student.RoleId.ToString() },
                    MorningDistanceKm = dist?.MorningDistanceKm ?? 0,
                    EveningDistanceKm = dist?.EveningDistanceKm ?? 0,
                    MaxMorningDistanceKm = dist?.MaxMorningDistanceKm ?? 0,
                    MaxEveningDistanceKm = dist?.MaxEveningDistanceKm ?? 0,
                    DistanceRecordId = dist?.Id
                });
            }

            return result;
        }

        private async Task<ClassResponseDto> MapToDtoAsync(
            Class classEntity,
            Term? term = null,
            dtc.Domain.Entities.Training.Course? course = null,
            dtc.Domain.Entities.Location.Center? center = null,
            dtc.Domain.Entities.Permissions.User? instructor = null)
        {
            term ??= await _unitOfWork.Terms.GetByIdAsync(classEntity.TermId);
            course ??= term == null ? null : await _unitOfWork.Courses.GetByIdAsync(term.CourseId);
            center ??= course == null ? null : await _unitOfWork.Centers.GetByIdAsync(course.CenterId);
            instructor ??= await _unitOfWork.Users.GetByIdAsync(classEntity.InstructorId);

            return new ClassResponseDto
            {
                Id = classEntity.Id,
                TermId = classEntity.TermId,
                CourseId = term?.CourseId ?? Guid.Empty,
                InstructorId = classEntity.InstructorId,
                ClassName = classEntity.ClassName,
                CurrentStudents = classEntity.CurrentStudents,
                MaxStudents = classEntity.MaxStudents,
                ClassType = classEntity.ClassType.ToString(),
                Status = classEntity.Status.ToString(),
                TermName = term?.TermName,
                CourseName = course?.CourseName,
                InstructorName = instructor?.FullName,
                CenterId = center?.Id,
                CenterName = center?.CenterName,
                TermStartDate = term?.StartDate,
                TermEndDate = term?.EndDate,
                CreatedAt = classEntity.CreatedAt
            };
        }
    }
}
