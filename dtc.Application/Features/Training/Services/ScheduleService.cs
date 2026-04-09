using dtc.Application.Features.Training.DTOs;
using dtc.Application.Features.Training.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Classes;
using dtc.Domain.Entities.Location;
using dtc.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dtc.Application.Features.Training.Services
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
            var classEntity = await GetClassOrThrowAsync(request.ClassId);
            await GetInstructorOrThrowAsync(request.InstructorId);
            var address = await GetAddressOrThrowAsync(request.AddressId);

            await EnsureInstructorAvailableAsync(request.InstructorId, request.StartTime, request.EndTime);

            var schedule = new ClassSchedule(
                classId: request.ClassId,
                instructorId: request.InstructorId,
                startTime: request.StartTime,
                endTime: request.EndTime,
                addressId: request.AddressId,
                createdBy: adminId);

            await _unitOfWork.ClassSchedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return await MapToDtoCompleteAsync(schedule, address, classEntity);
        }

        public async Task<IEnumerable<ClassScheduleResponseDto>> CreateBulkSchedulesAsync(BulkCreateClassScheduleRequestDto request, Guid adminId)
        {
            var classEntity = await GetClassOrThrowAsync(request.ClassId);
            if (request.Schedules == null || request.Schedules.Count == 0)
                throw new InvalidOperationException("At least one schedule is required.");

            var createdSchedules = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var created = new List<ClassSchedule>();
                var drafts = request.Schedules.OrderBy(s => s.StartTime).ToList();

                for (var i = 0; i < drafts.Count; i++)
                {
                    var draft = drafts[i];
                    await GetInstructorOrThrowAsync(draft.InstructorId);
                    await GetAddressOrThrowAsync(draft.AddressId);
                    await EnsureInstructorAvailableAsync(draft.InstructorId, draft.StartTime, draft.EndTime);

                    var overlapsInBatch = created.Any(s =>
                        s.InstructorId == draft.InstructorId &&
                        s.IsOverlapping(draft.StartTime, draft.EndTime));
                    if (overlapsInBatch)
                        throw new InvalidOperationException("One or more imported schedules overlap for the same instructor.");

                    var schedule = new ClassSchedule(
                        request.ClassId,
                        draft.InstructorId,
                        draft.StartTime,
                        draft.EndTime,
                        draft.AddressId,
                        adminId);

                    await _unitOfWork.ClassSchedules.AddAsync(schedule);
                    created.Add(schedule);
                }

                return created;
            });

            var responses = new List<ClassScheduleResponseDto>();
            foreach (var schedule in createdSchedules)
            {
                responses.Add(await MapToDtoCompleteAsync(schedule, classEntity: classEntity));
            }

            return responses.OrderBy(s => s.StartTime);
        }

        public async Task<ScheduleImportPreviewDto> ImportSchedulePreviewAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Schedule file is required.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            List<Dictionary<string, string>> rawRows;

            using var stream = file.OpenReadStream();
            rawRows = extension switch
            {
                ".csv" => await ReadCsvRowsAsync(stream),
                ".xlsx" => await ReadXlsxRowsAsync(stream),
                _ => throw new InvalidOperationException("Only .xlsx or .csv schedule files are supported.")
            };

            var addresses = (await _unitOfWork.Addresses.GetAllAsync()).ToList();
            var instructors = (await _unitOfWork.Users.FindAsync(u => u.RoleId == UserRole.Instructor && u.IsActive)).ToList();
            var warnings = new List<string>();
            var schedules = new List<ClassScheduleDraftDto>();

            foreach (var row in rawRows)
            {
                var lineLabel = row.TryGetValue("__row", out var rowNo) ? $"Row {rowNo}" : "A row";

                var start = ParseDateTime(GetValue(row, "starttime", "start", "batdau", "start_time"), lineLabel, "StartTime");
                var end = ParseDateTime(GetValue(row, "endtime", "end", "ketthuc", "end_time"), lineLabel, "EndTime");
                var addressId = ResolveAddressId(row, addresses, lineLabel);
                var instructorId = ResolveInstructorId(row, instructors, lineLabel);

                schedules.Add(new ClassScheduleDraftDto
                {
                    InstructorId = instructorId,
                    StartTime = start,
                    EndTime = end,
                    AddressId = addressId
                });
            }

            if (schedules.Count == 0)
            {
                warnings.Add("No schedule rows were found in the uploaded file.");
            }

            return new ScheduleImportPreviewDto
            {
                Schedules = schedules.OrderBy(s => s.StartTime).ToList(),
                Warnings = warnings
            };
        }

        public async Task<ClassScheduleResponseDto> UpdateScheduleAsync(Guid id, UpdateClassScheduleRequestDto request, Guid adminId)
        {
            var schedule = await _unitOfWork.ClassSchedules.GetByIdAsync(id);
            if (schedule == null)
                throw new Exception("Class schedule not found");

            bool isUpdated = false;
            Address? address = null;

            if (request.AddressId.HasValue)
                address = await GetAddressOrThrowAsync(request.AddressId.Value);

            isUpdated = schedule.Reschedule(
                newStart: request.StartTime,
                newEnd: request.EndTime,
                newAddressId: request.AddressId,
                updatedBy: adminId);

            if (request.InstructorId.HasValue && request.InstructorId.Value != schedule.InstructorId)
            {
                await GetInstructorOrThrowAsync(request.InstructorId.Value);
                schedule.ChangeInstructor(request.InstructorId.Value, adminId);
                isUpdated = true;
            }

            if (isUpdated)
            {
                await EnsureInstructorAvailableAsync(schedule.InstructorId, schedule.StartTime, schedule.EndTime, schedule.Id);
                await _unitOfWork.ClassSchedules.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
            }

            return await MapToDtoCompleteAsync(schedule, address);
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

            await GetAddressOrThrowAsync(request.AddressId);

            bool isUpdated = schedule.Reschedule(
                newStart: schedule.StartTime,
                newEnd: schedule.EndTime,
                newAddressId: request.AddressId,
                updatedBy: adminId);

            if (isUpdated)
            {
                await _unitOfWork.ClassSchedules.UpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<IEnumerable<ClassScheduleResponseDto>> GetMySchedulesAsync(Guid studentId)
        {
            var enrollments = await _unitOfWork.ClassStudents.FindAsync(cs => cs.StudentId == studentId);
            if (enrollments == null || !enrollments.Any())
                return new List<ClassScheduleResponseDto>();

            var classIds = enrollments.Select(e => e.ClassId).Distinct().ToList();
            var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => classIds.Contains(s.ClassId));
            if (schedules == null || !schedules.Any())
                return new List<ClassScheduleResponseDto>();

            var dtos = new List<ClassScheduleResponseDto>();
            foreach (var schedule in schedules)
            {
                dtos.Add(await MapToDtoCompleteAsync(schedule));
            }

            return dtos.OrderBy(s => s.StartTime);
        }

        public async Task<IEnumerable<ClassScheduleResponseDto>> GetTeachingScheduleAsync(Guid instructorId)
        {
            var schedules = await _unitOfWork.ClassSchedules.FindAsync(s => s.InstructorId == instructorId);
            if (schedules == null || !schedules.Any())
                return new List<ClassScheduleResponseDto>();

            var dtos = new List<ClassScheduleResponseDto>();
            foreach (var schedule in schedules)
            {
                dtos.Add(await MapToDtoCompleteAsync(schedule));
            }

            return dtos.OrderBy(s => s.StartTime);
        }

        private async Task<ClassScheduleResponseDto> MapToDtoCompleteAsync(
            ClassSchedule schedule,
            Address? address = null,
            Class? classEntity = null)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(schedule.InstructorId);
            classEntity ??= await _unitOfWork.Classes.GetByIdAsync(schedule.ClassId);
            address ??= await _unitOfWork.Addresses.GetByIdAsync(schedule.AddressId);
            var addressName = address?.AddressName ?? "Unknown Address";

            return new ClassScheduleResponseDto
            {
                Id = schedule.Id,
                ClassId = schedule.ClassId,
                InstructorId = schedule.InstructorId,
                ClassName = classEntity?.ClassName ?? "Unknown Class",
                InstructorName = instructor?.FullName ?? "Unknown Instructor",
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                AddressId = schedule.AddressId,
                AddressName = addressName,
                Location = addressName,
                CreatedAt = schedule.CreatedAt,
            };
        }

        private async Task<Class> GetClassOrThrowAsync(Guid classId)
        {
            return await _unitOfWork.Classes.GetByIdAsync(classId)
                ?? throw new Exception("Class not found");
        }

        private async Task<Address> GetAddressOrThrowAsync(int addressId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(addressId);
            if (address == null)
                throw new Exception("Address not found");

            return address;
        }

        private async Task EnsureInstructorAvailableAsync(Guid instructorId, DateTime startTime, DateTime endTime, Guid? ignoreScheduleId = null)
        {
            var existingSchedules = await _unitOfWork.ClassSchedules.FindAsync(s =>
                s.InstructorId == instructorId &&
                (!ignoreScheduleId.HasValue || s.Id != ignoreScheduleId.Value) &&
                s.StartTime < endTime &&
                startTime < s.EndTime);

            if (existingSchedules.Any())
                throw new InvalidOperationException("Instructor is already scheduled for another class during this time period.");
        }

        private async Task GetInstructorOrThrowAsync(Guid instructorId)
        {
            var instructor = await _unitOfWork.Users.GetByIdAsync(instructorId);
            if (instructor == null || instructor.RoleId != UserRole.Instructor)
                throw new Exception("Instructor not found");
        }

        private static string? GetValue(Dictionary<string, string> row, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (row.TryGetValue(NormalizeHeader(key), out var value) && !string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return null;
        }

        private static int ResolveAddressId(Dictionary<string, string> row, List<Address> addresses, string lineLabel)
        {
            var addressIdText = GetValue(row, "addressid", "address_id", "diachiid");
            if (!string.IsNullOrWhiteSpace(addressIdText) && int.TryParse(addressIdText, out var addressId) && addressId > 0)
                return addressId;

            var addressName = GetValue(row, "addressname", "address", "diadiem", "location");
            if (!string.IsNullOrWhiteSpace(addressName))
            {
                var address = addresses.FirstOrDefault(a => string.Equals(a.AddressName, addressName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (address != null)
                    return address.Id;
            }

            throw new InvalidOperationException($"{lineLabel}: AddressId or AddressName is required.");
        }

        private static Guid ResolveInstructorId(Dictionary<string, string> row, List<dtc.Domain.Entities.Permissions.User> instructors, string lineLabel)
        {
            var instructorIdText = GetValue(row, "instructorid", "instructor_id", "teacherid");
            if (!string.IsNullOrWhiteSpace(instructorIdText) && Guid.TryParse(instructorIdText, out var instructorId))
                return instructorId;

            var instructorEmail = GetValue(row, "instructoremail", "teacheremail", "email");
            if (!string.IsNullOrWhiteSpace(instructorEmail))
            {
                var instructor = instructors.FirstOrDefault(u => string.Equals(u.Email.Value, instructorEmail.Trim(), StringComparison.OrdinalIgnoreCase));
                if (instructor != null)
                    return instructor.Id;
            }

            var instructorName = GetValue(row, "instructorname", "teachername", "instructor");
            if (!string.IsNullOrWhiteSpace(instructorName))
            {
                var instructor = instructors.FirstOrDefault(u => string.Equals(u.FullName, instructorName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (instructor != null)
                    return instructor.Id;
            }

            throw new InvalidOperationException($"{lineLabel}: InstructorId, InstructorEmail, or InstructorName is required.");
        }

        private static DateTime ParseDateTime(string? value, string lineLabel, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{lineLabel}: {fieldName} is required.");

            var formats = new[]
            {
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy HH:mm:ss",
                "M/d/yyyy H:mm",
                "M/d/yyyy H:mm:ss"
            };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
                return parsed;

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out parsed))
                return parsed;

            throw new InvalidOperationException($"{lineLabel}: {fieldName} has an invalid date format.");
        }

        private static async Task<List<Dictionary<string, string>>> ReadCsvRowsAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, true, leaveOpen: true);
            var content = await reader.ReadToEndAsync();
            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 1)
                return new List<Dictionary<string, string>>();

            var headers = SplitCsvLine(lines[0]).Select(NormalizeHeader).ToList();
            var rows = new List<Dictionary<string, string>>();

            for (var i = 1; i < lines.Length; i++)
            {
                var values = SplitCsvLine(lines[i]);
                var row = new Dictionary<string, string> { ["__row"] = (i + 1).ToString(CultureInfo.InvariantCulture) };
                for (var j = 0; j < headers.Count; j++)
                {
                    row[headers[j]] = j < values.Count ? values[j] : string.Empty;
                }
                rows.Add(row);
            }

            return rows;
        }

        private static List<string> SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (ch == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                    continue;
                }

                current.Append(ch);
            }

            result.Add(current.ToString().Trim());
            return result;
        }

        private static async Task<List<Dictionary<string, string>>> ReadXlsxRowsAsync(Stream stream)
        {
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            memory.Position = 0;

            using var archive = new ZipArchive(memory, ZipArchiveMode.Read, leaveOpen: true);
            var workbookEntry = archive.GetEntry("xl/workbook.xml")
                ?? throw new InvalidOperationException("Invalid xlsx file.");
            var workbookRelsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels")
                ?? throw new InvalidOperationException("Invalid xlsx file.");
            var sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");

            var workbook = XDocument.Load(workbookEntry.Open());
            var workbookRels = XDocument.Load(workbookRelsEntry.Open());
            var ns = workbook.Root?.Name.Namespace ?? XNamespace.None;
            var relNs = workbookRels.Root?.Name.Namespace ?? XNamespace.None;

            var firstSheet = workbook.Descendants(ns + "sheet").FirstOrDefault()
                ?? throw new InvalidOperationException("No worksheet found in xlsx file.");
            var relationId = firstSheet.Attribute(XName.Get("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships"))?.Value
                ?? throw new InvalidOperationException("Worksheet relation not found.");

            var target = workbookRels.Descendants(relNs + "Relationship")
                .FirstOrDefault(r => string.Equals(r.Attribute("Id")?.Value, relationId, StringComparison.Ordinal))
                ?.Attribute("Target")?.Value
                ?? throw new InvalidOperationException("Worksheet target not found.");

            var sheetPath = target.StartsWith("/") ? target.TrimStart('/') : $"xl/{target.TrimStart('/')}";
            var sheetEntry = archive.GetEntry(sheetPath.Replace("\\", "/"))
                ?? throw new InvalidOperationException("Worksheet data not found.");

            var sharedStrings = new List<string>();
            if (sharedStringsEntry != null)
            {
                var sharedDoc = XDocument.Load(sharedStringsEntry.Open());
                var sharedNs = sharedDoc.Root?.Name.Namespace ?? XNamespace.None;
                sharedStrings = sharedDoc.Descendants(sharedNs + "si")
                    .Select(si => string.Concat(si.Descendants(sharedNs + "t").Select(t => t.Value)))
                    .ToList();
            }

            var sheetDoc = XDocument.Load(sheetEntry.Open());
            var sheetNs = sheetDoc.Root?.Name.Namespace ?? XNamespace.None;
            var rows = sheetDoc.Descendants(sheetNs + "row").ToList();
            if (rows.Count <= 1)
                return new List<Dictionary<string, string>>();

            var headers = ReadSheetRow(rows[0], sheetNs, sharedStrings).Select(NormalizeHeader).ToList();
            var result = new List<Dictionary<string, string>>();

            foreach (var row in rows.Skip(1))
            {
                var values = ReadSheetRow(row, sheetNs, sharedStrings);
                var dictionary = new Dictionary<string, string>
                {
                    ["__row"] = row.Attribute("r")?.Value ?? string.Empty
                };

                for (var i = 0; i < headers.Count; i++)
                {
                    dictionary[headers[i]] = i < values.Count ? values[i] : string.Empty;
                }

                result.Add(dictionary);
            }

            return result;
        }

        private static List<string> ReadSheetRow(XElement row, XNamespace ns, List<string> sharedStrings)
        {
            var cells = row.Elements(ns + "c").ToList();
            var values = new List<string>();
            foreach (var cell in cells)
            {
                var cellType = cell.Attribute("t")?.Value;
                var rawValue = cell.Element(ns + "v")?.Value ?? string.Empty;

                if (cellType == "s" && int.TryParse(rawValue, out var sharedIndex) && sharedIndex >= 0 && sharedIndex < sharedStrings.Count)
                {
                    values.Add(sharedStrings[sharedIndex]);
                }
                else if (cellType == "inlineStr")
                {
                    values.Add(cell.Element(ns + "is")?.Value ?? string.Empty);
                }
                else
                {
                    values.Add(rawValue);
                }
            }

            return values;
        }

        private static string NormalizeHeader(string header)
        {
            return new string((header ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(ch => char.IsLetterOrDigit(ch))
                .ToArray());
        }
    }
}
