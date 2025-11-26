using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewBO.DTOs.Responses
{
    public class PersonalizationHistoryItemResponse
    {
        // Thiếu các trường cần thiết trong DTO cũ, tôi sẽ bổ sung
        public int PersonalizationId { get; set; } // <--- Cần thiết
        public string? QuestionText { get; set; } // <--- Tên câu hỏi (đã bị thiếu trong DTO bạn cung cấp)

        public string? NamePractice { get; set; }

        public string? Practice { get; set; }

        public string? Exercise { get; set; }

        public string? Objective { get; set; }

        // Khóa ngoại được giữ lại để tham chiếu
        public int ReportQId { get; set; }
        public int SessionId { get; set; }
    }
}
