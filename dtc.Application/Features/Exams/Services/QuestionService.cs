using dtc.Application.Features.Exams.DTOs;
using dtc.Application.Features.Exams.Interfaces;
using dtc.Domain.Entities;
using dtc.Domain.Entities.Exams;
using dtc.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dtc.Application.Features.Exams.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<QuestionResponseDto> CreateQuestionAsync(CreateQuestionRequestDto request)
        {
            var nextId = await GetNextQuestionIdAsync();
            var question = BuildQuestion(request);
            question.AssignIdentity(nextId);

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(question);
        }

        public async Task<QuestionResponseDto> UpdateQuestionAsync(int id, UpdateQuestionRequestDto request)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
                throw new Exception("Question not found");

            question.UpdateContent(
                category: request.Category,
                content: request.Content,
                a: request.AnswerA,
                b: request.AnswerB,
                c: request.AnswerC,
                d: request.AnswerD,
                correctAnswer: request.CorrectAnswer,
                imageLink: request.ImageLink,
                explanation: request.Explanation);

            await _unitOfWork.Questions.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(question);
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
                throw new Exception("Question not found");

            await _unitOfWork.Questions.RemoveAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<QuestionResponseDto> GetQuestionDetailAsync(int id)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
                throw new Exception("Question not found");

            return MapToDto(question);
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetAllQuestionsAsync(string? category = null)
        {
            var questions = (await _unitOfWork.Questions.GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var normalizedCategory = QuestionCategoryNames.Normalize(category);
                questions = questions
                    .Where(q => string.Equals(q.Category, normalizedCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return questions
                .OrderBy(q => q.Id)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<QuestionImportResponseDto> ImportQuestionsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Question file is required.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            List<Dictionary<string, string>> rawRows;

            using (var stream = file.OpenReadStream())
            {
                rawRows = extension switch
                {
                    ".csv" => await ReadCsvRowsAsync(stream),
                    ".xlsx" => await ReadXlsxRowsAsync(stream),
                    _ => throw new InvalidOperationException("Only .xlsx or .csv question files are supported.")
                };
            }

            var warnings = new List<string>();
            var createdQuestions = new List<Question>();
            var nextId = await GetNextQuestionIdAsync();

            foreach (var row in rawRows)
            {
                if (row
                    .Where(item => !string.Equals(item.Key, "__row", StringComparison.Ordinal))
                    .All(item => string.IsNullOrWhiteSpace(item.Value)))
                {
                    continue;
                }

                var lineLabel = row.TryGetValue("__row", out var rowNo) ? $"Row {rowNo}" : "A row";
                var content = GetRequiredValue(row, lineLabel, "content", "question", "questioncontent", "noidung");
                var category = GetRequiredValue(row, lineLabel, "category", "type", "loaicauhoi");
                var answerA = GetValue(row, "answera", "a", "optiona");
                var answerB = GetValue(row, "answerb", "b", "optionb");
                var answerC = GetValue(row, "answerc", "c", "optionc");
                var answerD = GetValue(row, "answerd", "d", "optiond");
                var correctAnswerText = GetRequiredValue(row, lineLabel, "correctanswer", "correct", "dapan");
                var imageLink = GetValue(row, "imagelink", "imageurl", "image");
                var explanation = GetValue(row, "explanation", "giaithich");

                var correctAnswer = ParseAnswerOption(correctAnswerText, lineLabel);

                var question = new Question(
                    category: category,
                    content: content,
                    correctAnswer: correctAnswer,
                    a: answerA,
                    b: answerB,
                    c: answerC,
                    d: answerD,
                    imageLink: imageLink,
                    explanation: explanation);

                question.AssignIdentity(nextId++);
                createdQuestions.Add(question);
            }

            foreach (var question in createdQuestions)
            {
                await _unitOfWork.Questions.AddAsync(question);
            }

            await _unitOfWork.SaveChangesAsync();

            return new QuestionImportResponseDto
            {
                ImportedCount = createdQuestions.Count,
                Warnings = warnings,
                Questions = createdQuestions.Select(MapToDto).ToList()
            };
        }

        public Task<byte[]> GenerateImportTemplateAsync()
        {
            var headers = new[]
            {
                "Category",
                "Content",
                "AnswerA",
                "AnswerB",
                "AnswerC",
                "AnswerD",
                "CorrectAnswer",
                "ImageLink",
                "Explanation"
            };

            var sampleRows = new List<string[]>
            {
                new[]
                {
                    QuestionCategoryNames.Theory,
                    "Nguoi lai xe gap den vang nhap nhay thi phai lam gi?",
                    "Giam toc do va quan sat ky",
                    "Tang toc de di nhanh",
                    "Dung giua giao lo",
                    "Bat den khan cap",
                    "1",
                    "",
                    "Can giam toc do, quan sat va nhuong duong khi can."
                },
                new[]
                {
                    QuestionCategoryNames.Sign,
                    "Bien nao bao hieu cam xe tai vuot?",
                    "Bien tron nen xanh",
                    "Bien tron vien do co hinh xe tai",
                    "Bien tam giac vien do",
                    "Bien chu nhat nen xanh",
                    "2",
                    "",
                    "Bien cam co vien do va hinh xe tai ben trong."
                },
                new[]
                {
                    QuestionCategoryNames.Simulation,
                    "Trong bai thi sa hinh, khi dung den do tren doc can uu tien thao tac nao?",
                    "Nhe nhang giu phanh va can bang chan con",
                    "Tat may",
                    "Nhin guong hau lien tuc",
                    "Bam coi",
                    "1",
                    "",
                    "Can giu xe khong troi va khoi hanh em."
                }
            };

            return Task.FromResult(BuildXlsxFile("CauHoiMau", headers, sampleRows));
        }

        private async Task<int> GetNextQuestionIdAsync()
        {
            var questions = await _unitOfWork.Questions.GetAllAsync();
            return questions.Any() ? questions.Max(q => q.Id) + 1 : 1;
        }

        private static Question BuildQuestion(CreateQuestionRequestDto request)
        {
            return new Question(
                category: request.Category,
                content: request.Content,
                correctAnswer: request.CorrectAnswer,
                a: request.AnswerA,
                b: request.AnswerB,
                c: request.AnswerC,
                d: request.AnswerD,
                imageLink: request.ImageLink,
                explanation: request.Explanation);
        }

        private static QuestionResponseDto MapToDto(Question question)
        {
            return new QuestionResponseDto
            {
                Id = question.Id,
                Category = question.Category,
                Content = question.Content,
                AnswerA = question.AnswerA,
                AnswerB = question.AnswerB,
                AnswerC = question.AnswerC,
                AnswerD = question.AnswerD,
                CorrectAnswer = question.CorrectAnswer,
                ImageLink = question.ImageLink,
                Explanation = question.Explanation,
                CreatedAt = question.CreatedAt
            };
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

        private static string GetRequiredValue(Dictionary<string, string> row, string lineLabel, params string[] keys)
        {
            var value = GetValue(row, keys);
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{lineLabel}: {keys[0]} is required.");

            return value;
        }

        private static AnswerOption ParseAnswerOption(string value, string lineLabel)
        {
            var normalized = value.Trim().ToUpperInvariant();

            if (int.TryParse(normalized, out var numericValue) && numericValue >= 1 && numericValue <= 4)
                return (AnswerOption)numericValue;

            return normalized switch
            {
                "A" => AnswerOption.A,
                "B" => AnswerOption.B,
                "C" => AnswerOption.C,
                "D" => AnswerOption.D,
                _ => throw new InvalidOperationException($"{lineLabel}: CorrectAnswer must be 1, 2, 3, 4 or A, B, C, D.")
            };
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
            var values = new List<string>();
            var currentColumnIndex = 0;

            foreach (var cell in row.Elements(ns + "c"))
            {
                var cellReference = cell.Attribute("r")?.Value;
                var targetColumnIndex = GetColumnIndex(cellReference);

                while (currentColumnIndex < targetColumnIndex)
                {
                    values.Add(string.Empty);
                    currentColumnIndex++;
                }

                var cellType = cell.Attribute("t")?.Value;
                var rawValue = cell.Element(ns + "v")?.Value ?? string.Empty;

                if (cellType == "s" && int.TryParse(rawValue, out var sharedIndex) && sharedIndex >= 0 && sharedIndex < sharedStrings.Count)
                {
                    values.Add(sharedStrings[sharedIndex]);
                    currentColumnIndex++;
                    continue;
                }

                if (cellType == "inlineStr")
                {
                    values.Add(string.Concat(cell.Descendants(ns + "t").Select(t => t.Value)));
                    currentColumnIndex++;
                    continue;
                }

                values.Add(rawValue);
                currentColumnIndex++;
            }

            return values;
        }

        private static int GetColumnIndex(string? cellReference)
        {
            if (string.IsNullOrWhiteSpace(cellReference))
                return 0;

            var columnName = new string(cellReference
                .TakeWhile(char.IsLetter)
                .Select(char.ToUpperInvariant)
                .ToArray());

            if (string.IsNullOrWhiteSpace(columnName))
                return 0;

            var columnIndex = 0;
            foreach (var letter in columnName)
            {
                columnIndex = (columnIndex * 26) + (letter - 'A' + 1);
            }

            return Math.Max(0, columnIndex - 1);
        }

        private static string NormalizeHeader(string header)
        {
            return new string((header ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Where(ch => char.IsLetterOrDigit(ch))
                .ToArray());
        }

        private static byte[] BuildXlsxFile(string sheetName, IReadOnlyList<string> headers, IReadOnlyList<string[]> rows)
        {
            using var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                AddEntry(archive, "[Content_Types].xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                    "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                    "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                    "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                    "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                    "<Override PartName=\"/docProps/core.xml\" ContentType=\"application/vnd.openxmlformats-package.core-properties+xml\"/>" +
                    "<Override PartName=\"/docProps/app.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.extended-properties+xml\"/>" +
                    "</Types>");

                AddEntry(archive, "_rels/.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                    "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties\" Target=\"docProps/core.xml\"/>" +
                    "<Relationship Id=\"rId3\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties\" Target=\"docProps/app.xml\"/>" +
                    "</Relationships>");

                AddEntry(archive, "docProps/app.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Properties xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/extended-properties\" xmlns:vt=\"http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes\">" +
                    "<Application>Codex</Application>" +
                    "</Properties>");

                AddEntry(archive, "docProps/core.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<cp:coreProperties xmlns:cp=\"http://schemas.openxmlformats.org/package/2006/metadata/core-properties\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:dcterms=\"http://purl.org/dc/terms/\" xmlns:dcmitype=\"http://purl.org/dc/dcmitype/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                    "<dc:title>Question Import Template</dc:title>" +
                    "<dc:creator>Codex</dc:creator>" +
                    $"<dcterms:created xsi:type=\"dcterms:W3CDTF\">{DateTime.UtcNow:O}</dcterms:created>" +
                    "</cp:coreProperties>");

                AddEntry(archive, "xl/workbook.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                    "<sheets><sheet name=\"" + EscapeXml(sheetName) + "\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                    "</workbook>");

                AddEntry(archive, "xl/_rels/workbook.xml.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                    "</Relationships>");

                var allRows = new List<string[]> { headers.ToArray() };
                allRows.AddRange(rows);
                AddEntry(archive, "xl/worksheets/sheet1.xml", BuildWorksheetXml(allRows));
            }

            return stream.ToArray();
        }

        private static string BuildWorksheetXml(IReadOnlyList<string[]> rows)
        {
            var builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                builder.Append($"<row r=\"{rowIndex + 1}\">");
                var row = rows[rowIndex];

                for (var colIndex = 0; colIndex < row.Length; colIndex++)
                {
                    var cellRef = $"{GetColumnName(colIndex + 1)}{rowIndex + 1}";
                    builder.Append($"<c r=\"{cellRef}\" t=\"inlineStr\"><is><t>{EscapeXml(row[colIndex] ?? string.Empty)}</t></is></c>");
                }

                builder.Append("</row>");
            }

            builder.Append("</sheetData></worksheet>");
            return builder.ToString();
        }

        private static void AddEntry(ZipArchive archive, string path, string content)
        {
            var entry = archive.CreateEntry(path, CompressionLevel.Fastest);
            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write(content);
        }

        private static string EscapeXml(string value)
        {
            return SecurityElement.Escape(value) ?? string.Empty;
        }

        private static string GetColumnName(int index)
        {
            var columnName = string.Empty;
            while (index > 0)
            {
                var remainder = (index - 1) % 26;
                columnName = (char)(65 + remainder) + columnName;
                index = (index - 1) / 26;
            }

            return columnName;
        }
    }
}
