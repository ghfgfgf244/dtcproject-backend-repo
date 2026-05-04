using System.Threading;
using System.Threading.Tasks;
using dtc.Application.Features.AI.DTOs;
using dtc.Application.Features.AI.Interfaces;

namespace dtc.Application.Features.AI.Services
{
    public class DashboardInsightService : IDashboardInsightService
    {
        private readonly IAiRouterService _aiRouterService;

        public DashboardInsightService(IAiRouterService aiRouterService)
        {
            _aiRouterService = aiRouterService;
        }

        public async Task<DashboardInsightResponseDto> SummarizeAsync(
            DashboardInsightRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var prompt =
                $"""
                Bạn là trợ lý phân tích dashboard cho role {request.Role}.
                Dữ liệu ngữ cảnh: {request.ContextJson ?? "{}"}

                Hãy tóm tắt bằng tiếng Việt có dấu, ngắn gọn, rõ ràng, dễ đọc trên giao diện dashboard.
                Bắt buộc tuân thủ đúng định dạng sau:
                I. Tổng quan
                - tối đa 2 gạch đầu dòng
                II. Xu hướng chính
                - tối đa 3 gạch đầu dòng
                III. Vấn đề cần ưu tiên
                - tối đa 3 gạch đầu dòng
                IV. Đề xuất hành động
                - tối đa 3 gạch đầu dòng

                Quy tắc định dạng:
                - Mỗi tiêu đề phải nằm trên dòng riêng và dùng đúng dạng I., II., III., IV.
                - Mỗi ý phải bắt đầu bằng "- " và nằm trên dòng riêng.
                - Không được tạo mục rỗng, không được trả về một dòng chỉ có dấu "-".
                - Không dùng markdown table.
                - Không dùng ký tự "|" hoặc "*".
                - Không viết thành một đoạn văn dài liên tục.
                - Không nhắc tới JSON, contextJson hay dữ liệu thô.
                """;

            var result = await _aiRouterService.GenerateAsync(
                "dashboard-summary",
                prompt,
                cancellationToken);

            return new DashboardInsightResponseDto
            {
                Summary = result.Content,
                Model = result.Model
            };
        }
    }
}
