using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
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

        public ClassService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                var otherClassIds = termClasses.Where(c => c.Id != classId).Select(c => c.Id).ToList();

                if (otherClassIds.Count > 0)
                {
                    var duplicates = (await _unitOfWork.ClassStudents.FindAsync(cs => otherClassIds.Contains(cs.ClassId) && requestedSet.Contains(cs.StudentId)))
                        .Select(cs => cs.StudentId)
                        .Distinct()
                        .ToList();

                    if (duplicates.Count > 0)
                        throw new InvalidOperationException("One or more students are already assigned to another class in this term.");
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
            var currentTermClassIds = currentTermClasses.Select(c => c.Id).ToList();

            var alreadyAssignedIds = currentTermClassIds.Count == 0
                ? new HashSet<Guid>()
                : (await _unitOfWork.ClassStudents.FindAsync(cs => currentTermClassIds.Contains(cs.ClassId)))
                    .Select(cs => cs.StudentId)
                    .ToHashSet();

            var availableIds = candidateStudentIds.Where(id => !alreadyAssignedIds.Contains(id)).ToList();
            return await MapStudentsAsync(availableIds);
        }

        public async Task<AutoAssignClassesResponseDto> AutoAssignClassesAsync(AutoAssignClassesRequestDto request, Guid adminId)
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
            if (studentIds.Count == 0)
            {
                return new AutoAssignClassesResponseDto
                {
                    Message = "No approved students were found in this term to auto-assign."
                };
            }

            var termClasses = (await _unitOfWork.Classes.FindAsync(c => c.TermId == term.Id && !c.IsDeleted)).ToList();
            var existingClassIds = termClasses.Select(c => c.Id).ToList();
            var alreadyAssignedIds = existingClassIds.Count == 0
                ? new HashSet<Guid>()
                : (await _unitOfWork.ClassStudents.FindAsync(cs => existingClassIds.Contains(cs.ClassId)))
                    .Select(cs => cs.StudentId)
                    .ToHashSet();

            var eligibleStudentIds = studentIds.Where(id => !alreadyAssignedIds.Contains(id)).ToList();
            if (eligibleStudentIds.Count == 0)
            {
                return new AutoAssignClassesResponseDto
                {
                    Message = "All students in this term are already assigned to classes."
                };
            }

            var instructors = await GetAvailableInstructorsForCenterAsync(course.CenterId);
            if (instructors.Count == 0)
                throw new InvalidOperationException("No active instructors were found for this center.");

            var targetSize = center.MaxStudentPerClass > 0 ? center.MaxStudentPerClass : Math.Max(1, course.MaxStudents);
            var maxSize = Math.Max(targetSize, (int)Math.Ceiling(targetSize * 1.1m));
            var classCount = Math.Max(1, (int)Math.Ceiling(eligibleStudentIds.Count / (double)maxSize));
            var distributions = BuildDistributions(eligibleStudentIds.Count, classCount);

            var createdClasses = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var created = new List<Class>();
                var baseSequence = termClasses.Count(c => c.ClassType == request.ClassType);
                var studentCursor = 0;

                for (var i = 0; i < distributions.Count; i++)
                {
                    var size = distributions[i];
                    var assignedStudents = eligibleStudentIds.Skip(studentCursor).Take(size).ToList();
                    studentCursor += size;

                    var instructor = instructors[i % instructors.Count];
                    var classEntity = new Class(
                        term.Id,
                        instructor.Id,
                        BuildAutoClassName(course.CourseName, term.TermName, request.ClassType, baseSequence + i + 1),
                        request.ClassType,
                        Math.Max(targetSize, size),
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
                responseClasses.Add(await MapToDtoAsync(createdClass, term, course, center));
            }

            return new AutoAssignClassesResponseDto
            {
                Message = $"Created {responseClasses.Count} class(es) and assigned {eligibleStudentIds.Count} student(s).",
                EligibleStudents = eligibleStudentIds.Count,
                CreatedClasses = responseClasses.Count,
                Classes = responseClasses
            };
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

        private async Task<List<ClassStudentResponseDto>> MapStudentsAsync(List<Guid> studentIds)
        {
            if (studentIds.Count == 0)
                return new List<ClassStudentResponseDto>();

            var students = (await _unitOfWork.Users.FindAsync(u => studentIds.Contains(u.Id))).ToList();
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
