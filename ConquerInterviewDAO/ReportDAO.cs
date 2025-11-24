using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ConquerInterviewDAO
{
    public class ReportDAO
    {
            private readonly ConquerInterviewDbContext _context;
            private static ReportDAO instance;
            public static ReportDAO Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new ReportDAO();
                    }
                    return instance;
                }
            }

            public ReportDAO()
            {
                _context = new ConquerInterviewDbContext();
            }

            public async Task<ReportQuestion> GenerateAIReportAsync(InterviewAnswer answer, string question)
            {
                using var client = new HttpClient();

            // Gửi answer đến Flask
            var payload = new
            {
                questionText = question, // Thêm dòng này
                answerText = answer.TextAnswer ?? string.Empty
            };
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/evaluate_answer", payload);

            if (!response.IsSuccessStatusCode)
            {
                // Thêm log để xem chi tiết lỗi từ Flask
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ERROR] AI service returned {response.StatusCode}. Content: {errorContent}");
                throw new Exception($"AI service returned {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("📄 AI Report JSON: " + json);

            // Parse JSON an toàn
            AIReportResponse aiReport = null;
                try
                {
                    aiReport = JsonSerializer.Deserialize<AIReportResponse>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Deserialize error: " + ex.Message);
                    aiReport = new AIReportResponse
                    {
                        OverallAssessment = "Error parsing AI report",
                        ExpertiseExperience = json
                    };
                }

            // ✅ Tạo bản ghi report trong DB
            var report = new ReportQuestion
            {
                OverallAssessment = aiReport.OverallAssessment,
                FacialExpression = aiReport.FacialExpression,
                SpeakingSpeedClarity = aiReport.SpeakingSpeedClarity,
                ExpertiseExperience = aiReport.ExpertiseExperience,
                ResponseDurationPerQuestion = aiReport.ResponseDurationPerQuestion,
                AnswerContentAnalysis = aiReport.AnswerContentAnalysis,
                ComparisonWithOtherCandidates = aiReport.ComparisonWithOtherCandidates,
                ProblemSolvingSkills = aiReport.ProblemSolvingSkills,
                Status = aiReport.Status ?? "Completed",
                CustomerId = answer.Session.UserId,
                InterviewAId = answer.InterviewAId
            };


            _context.ReportQuestions.Add(report);
                await _context.SaveChangesAsync();

                return report;
            }

            public async Task<List<ReportQuestion>> GetReportsBySessionAsync(int sessionId)
            {
                return await _context.ReportQuestions                   
                    .Include(r => r.InterviewA)
                    .ThenInclude(a => a.Question)
                    .Where(r => r.InterviewA.SessionId == sessionId)
                    .ToListAsync();
            }

        public async Task<InterviewSession> GetSessionByIdAsync(int sessionId)
        {
            
            return await _context.InterviewSessions            
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }
    }

    public class AIReportResponse
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
