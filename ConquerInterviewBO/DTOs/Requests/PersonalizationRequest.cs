using System;
using System.Collections.Generic;

namespace ConquerInterviewBO.DTOs.Requests
{
    public class PersonalizationRequest
    {
        // ID của người dùng để gán lộ trình
        public int CustomerId { get; set; }

        // ID của một ReportQuestion (ví dụ: report cuối cùng) 
        // để liên kết các bước lộ trình này với một báo cáo cụ thể
        public int ReportQId { get; set; }

        // Danh sách các báo cáo chi tiết để gửi cho AI
        public List<ReportDTO> Reports { get; set; } = new List<ReportDTO>();
    }

    // DTO phụ, đại diện cho cấu trúc của một báo cáo mà AI cần
    public class ReportDTO
    {
        public string? OverallAssessment { get; set; }
        public string? FacialExpression { get; set; }
        public string? SpeakingSpeedClarity { get; set; }
        public string? ExpertiseExperience { get; set; }
        public string? ResponseDurationPerQuestion { get; set; }
        public string? AnswerContentAnalysis { get; set; }
        public string? ComparisonWithOtherCandidates { get; set; }
        public string? ProblemSolvingSkills { get; set; }
        public string? Status { get; set; }
    }
}