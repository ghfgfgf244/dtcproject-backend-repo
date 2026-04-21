using System.Collections.Generic;
using System.Text;
using dtc.Domain.Entities.Exams;

namespace dtc.Application.Features.AI.ChunkBuilders
{
    internal static class QuestionChunkBuilder
    {
        public static KnowledgeChunkPayload Build(Question question)
        {
            var text = new StringBuilder()
                .AppendLine($"Loai tai lieu: Cau hoi sat hach lai xe.")
                .AppendLine($"Nhom cau hoi: {question.Category}.")
                .AppendLine($"Noi dung cau hoi: {question.Content}")
                .AppendLine($"Dap an A: {question.AnswerA ?? "Khong co"}")
                .AppendLine($"Dap an B: {question.AnswerB ?? "Khong co"}")
                .AppendLine($"Dap an C: {question.AnswerC ?? "Khong co"}")
                .AppendLine($"Dap an D: {question.AnswerD ?? "Khong co"}")
                .AppendLine($"Dap an dung: {(int)question.CorrectAnswer}")
                .AppendLine($"Giai thich: {question.Explanation ?? "Chua co giai thich"}")
                .AppendLine($"Ti le lam sai: {(question.WrongRate * 100):0.##}%")
                .ToString();

            return new KnowledgeChunkPayload
            {
                Id = $"question-{question.Id}",
                Text = text,
                Metadata = new Dictionary<string, string>
                {
                    ["documentType"] = "question",
                    ["referenceId"] = question.Id.ToString(),
                    ["title"] = $"Cau hoi {question.Id}",
                    ["category"] = question.Category,
                    ["correctAnswer"] = ((int)question.CorrectAnswer).ToString(),
                    ["wrongRate"] = question.WrongRate.ToString("0.####")
                }
            };
        }
    }
}
