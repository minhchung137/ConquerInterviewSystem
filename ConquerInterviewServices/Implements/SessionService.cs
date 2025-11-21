using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using Microsoft.Extensions.Logging; // Thêm thư viện logging
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IReportRepository _reportRepo;
        private readonly ILogger<SessionService> _logger; // Khai báo logger

        public SessionService(
            ISessionRepository sessionRepo,
            IQuestionRepository questionRepo,
            IAnswerRepository answerRepo,
            IReportRepository reportRepo,
            ILogger<SessionService> logger) // Inject ILogger
        {
            _sessionRepo = sessionRepo;
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _reportRepo = reportRepo;
            _logger = logger; // Gán logger
        }

        public async Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request)
        {
            _logger.LogInformation("Bắt đầu xử lý StartSessionAsync cho User ID: {UserId}", request.UserId);

            // 1. Create session
            _logger.LogInformation("Đang tạo session với thời lượng {DurationMinutes} phút.", request.DurationMinutes ?? 30);
            var session = await _sessionRepo.CreateSessionAsync(request.UserId, request.JobPosition, request.DurationMinutes ?? 30);
            _logger.LogInformation("Tạo session ID: {SessionId} thành công.", session.SessionId);

            // 2. Get random level=1 questions
            _logger.LogInformation("Đang lấy 4 câu hỏi ngẫu nhiên mức độ 1.");
            var qList = await _questionRepo.GetRandomQuestionsAsync(request.QuestionEasy, 1);
            _logger.LogInformation("Lấy được {QuestionCount} câu hỏi từ DB.", qList.Count);

            // 3. Generate 1 AI-level2 question
            _logger.LogInformation("Đang gọi AI để tạo 1 câu hỏi...");
            var aiQ = await _questionRepo.GenerateAIQuestionAsync(
                request.Topic ?? "General",
                request.Industry ?? "General",
                request.JobPosition ?? "General",
                request.QuestionDifficult
            );
            _logger.LogInformation("Tạo câu hỏi AI thành công, Question ID: {QuestionId}", aiQ.FirstOrDefault()?.QuestionId);

            qList.AddRange(aiQ);

            // 4. Assign to session
            _logger.LogInformation("Đang gán {QuestionCount} câu hỏi cho Session ID: {SessionId}", qList.Count, session.SessionId);
            await _sessionRepo.AssignQuestionsToSessionAsync(session.SessionId, qList);

            // 5. Map response
            var response = new StartSessionResponse
            {
                SessionId = session.SessionId,
                StartTime = session.StartTime ?? DateTime.UtcNow,
                EndTime = session.EndTime ?? DateTime.UtcNow.AddMinutes(30),
                Questions = qList.Select(q => new QuestionResponse
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    DifficultyLevel = q.DifficultyLevel ?? 1
                }).ToList()
            };

            _logger.LogInformation("Hoàn tất StartSessionAsync cho Session ID: {SessionId}. Trả về {QuestionCount} câu hỏi.", response.SessionId, response.Questions.Count);
            return response;
        }

        public async Task<List<QuestionResponse>> GetQuestionsBySessionAsync(int sessionId)
        {
            _logger.LogInformation("Đang lấy câu hỏi cho Session ID: {SessionId}", sessionId);
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null)
            {
                _logger.LogWarning("Không tìm thấy Session ID: {SessionId}", sessionId);
                throw new AppException(AppErrorCode.SessionNotFound);
            }

            if (session.EndTime.HasValue && DateTime.UtcNow > session.EndTime.Value)
            {
                _logger.LogWarning("Session ID: {SessionId} đã hết hạn. Thời gian hiện tại: {CurrentTime}, Thời gian kết thúc: {EndTime}", sessionId, DateTime.UtcNow, session.EndTime.Value);
                throw new AppException(AppErrorCode.ForbiddenAccess);
            }

            var questions = await _sessionRepo.GetQuestionsBySessionAsync(sessionId);
            _logger.LogInformation("Tìm thấy {QuestionCount} câu hỏi cho Session ID: {SessionId}", questions.Count(), sessionId);

            return questions.Select(q => new QuestionResponse
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                DifficultyLevel = q.DifficultyLevel ?? 1
            }).ToList();
        }

        public async Task<AnswerReportResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            _logger.LogInformation("Bắt đầu xử lý SubmitAnswerAsync cho Session ID: {SessionId}, Question ID: {QuestionId}", request.SessionId, request.QuestionText);
            _logger.LogDebug("Nội dung câu trả lời: {AnswerText}", request.AnswerText); // Dùng LogDebug cho nội dung chi tiết

            var session = await _sessionRepo.GetByIdAsync(request.SessionId);
            if (session == null)
            {
                _logger.LogWarning("Không tìm thấy Session ID: {SessionId} khi submit câu trả lời.", request.SessionId);
                throw new AppException(AppErrorCode.SessionNotFound);
            }

            // ⏰ Check thời gian hợp lệ
            if (session.StartTime.HasValue && session.EndTime.HasValue)
            {
                if (DateTime.UtcNow < session.StartTime.Value || DateTime.UtcNow > session.EndTime.Value)
                {
                    session.Status = "Expired"; // Cập nhật trạng thái nếu cần
                    await _sessionRepo.UpdateSessionAsync(session); // Lưu thay đổi vào DB
                    _logger.LogWarning("Submit bị từ chối do hết hạn. Session ID: {SessionId}", request.SessionId);
                    throw new AppException(AppErrorCode.SessionExpired);
                }
            }

            // ✅ Lưu hoặc update answer
            _logger.LogInformation("Đang lưu câu trả lời vào DB...");
            var answer = await _answerRepo.SaveAnswerAsync(request);
            _logger.LogInformation("Lưu câu trả lời thành công, Answer ID: {AnswerId}", answer.InterviewAId);

            // ✅ Gọi AI để tạo report và lưu DB
            _logger.LogInformation("Đang gọi AI để tạo báo cáo cho Answer ID: {AnswerId}", answer.InterviewAId);

            // ✅ TRUYỀN THÊM request.QuestionText VÀO HÀM
            var report = await _reportRepo.GenerateAIReportAsync(answer, request.QuestionText);

            _logger.LogInformation("Tạo và lưu báo cáo thành công, Report ID: {ReportId}", report.ReportQId);

            return new AnswerReportResponse
            {
                AnswerId = answer.InterviewAId,
                OverallAssessment = report.OverallAssessment ?? "Pending",
                ExpertiseExperience = report.ExpertiseExperience ?? "Pending",
                FacialExpression = report.FacialExpression ?? "Pending",
                SpeakingSpeedClarity = report.SpeakingSpeedClarity ?? "Pending",
                ResponseDurationPerQuestion = report.ResponseDurationPerQuestion ?? "Pending",
                AnswerContentAnalysis = report.AnswerContentAnalysis ?? "Pending",
                ComparisonWithOtherCandidates = report.ComparisonWithOtherCandidates ?? "Pending",
                ProblemSolvingSkills = report.ProblemSolvingSkills ?? "Pending",
                Status = report.Status ?? "Pending"
            };
        }

        public async Task<ReportResponse> GetReportBySessionAsync(int sessionId)
        {
            _logger.LogInformation("Đang lấy báo cáo tổng hợp cho Session ID: {SessionId}", sessionId);
            var reports = await _reportRepo.GetReportsBySessionAsync(sessionId);
            _logger.LogInformation("Tìm thấy {ReportCount} báo cáo cho Session ID: {SessionId}", reports.Count(), sessionId);
            return new ReportResponse
            {
                SessionId = sessionId,
                Reports = reports.Select(r => new AnswerReportResponse
                {
                    AnswerId = r.InterviewAId,
                    OverallAssessment = r.OverallAssessment ?? string.Empty,
                    ExpertiseExperience = r.ExpertiseExperience ?? string.Empty,
                    FacialExpression = r.FacialExpression ?? string.Empty,
                    SpeakingSpeedClarity = r.SpeakingSpeedClarity ?? string.Empty,
                    ResponseDurationPerQuestion = r.ResponseDurationPerQuestion ?? string.Empty,
                    AnswerContentAnalysis = r.AnswerContentAnalysis ?? string.Empty,
                    ComparisonWithOtherCandidates = r.ComparisonWithOtherCandidates ?? string.Empty,
                    ProblemSolvingSkills = r.ProblemSolvingSkills ?? string.Empty,
                    Status = r.Status ?? "Pending"
                }).ToList()
            };
        }
    }
}
