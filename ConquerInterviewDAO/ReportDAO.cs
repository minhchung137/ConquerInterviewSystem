using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

            public async Task<ReportQuestion> GenerateAIReportAsync(InterviewAnswer answer)
            {
                using var client = new HttpClient();

                // Gửi answer đến Flask
                var payload = new { answer = answer.TextAnswer ?? string.Empty };
                var response = await client.PostAsJsonAsync("http://localhost:5000/api/generate_report", payload);

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"AI service returned {response.StatusCode}");

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
                    .Where(r => r.InterviewA.SessionId == sessionId)
                    .ToListAsync();
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
