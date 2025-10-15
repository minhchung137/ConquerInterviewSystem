using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepo;
        private readonly IQuestionRepository _questionRepo;
        private readonly IAnswerRepository _answerRepo;
        private readonly IReportRepository _reportRepo;

        public SessionService(ISessionRepository sessionRepo, IQuestionRepository questionRepo, IAnswerRepository answerRepo, IReportRepository reportRepo)
        {
            _sessionRepo = sessionRepo;
            _questionRepo = questionRepo;
            _answerRepo = answerRepo;
            _reportRepo = reportRepo;
        }

        // Async version
        public async Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request)
        {
            // 1. create session (30 minutes default - you may take from request)
            var session = await _sessionRepo.CreateSessionAsync(request.UserId, request.JobPosition, request.DurationMinutes ?? 30);

            // 2. get 4 random level=1 questions
            var qList = await _questionRepo.GetRandomQuestionsAsync(4, 1); // returns List<Question>

            // 3. generate 1 AI-level2 question
            var aiQ = await _questionRepo.GenerateAIQuestionAsync(
                request.Topic ?? "General",
                request.Industry ?? "General",
                request.JobPosition ?? "General"
            );


            qList.Add(aiQ);

            // 4. assign to session (persist mapping)
            await _sessionRepo.AssignQuestionsToSessionAsync(session.SessionId, qList);

            // 5. map response
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

            return response;
        }

        public async Task<List<QuestionResponse>> GetQuestionsBySessionAsync(int sessionId)
        {
            var session = await _sessionRepo.GetByIdAsync(sessionId);
            if (session == null)
                throw new AppException(AppErrorCode.SessionNotFound);

            if (session.EndTime.HasValue && DateTime.UtcNow > session.EndTime.Value)
                throw new AppException(AppErrorCode.ForbiddenAccess);

            var questions = await _sessionRepo.GetQuestionsBySessionAsync(sessionId);

            return questions.Select(q => new QuestionResponse
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                DifficultyLevel = q.DifficultyLevel ?? 1
            }).ToList();
        }

        public async Task<AnswerReportResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var session = await _sessionRepo.GetByIdAsync(request.SessionId);
            if (session == null)
                throw new AppException(AppErrorCode.SessionNotFound);

            // ⏰ Check thời gian hợp lệ
            if (session.StartTime.HasValue && session.EndTime.HasValue)
            {
                if (DateTime.UtcNow < session.StartTime.Value)
                    throw new AppException(AppErrorCode.ForbiddenAccess);
                if (DateTime.UtcNow > session.EndTime.Value)
                    throw new AppException(AppErrorCode.ForbiddenAccess);
            }

            // ✅ Lưu hoặc update answer
            var answer = await _answerRepo.SaveAnswerAsync(request);

            // ✅ Gọi AI để tạo report và lưu DB
            var report = await _reportRepo.GenerateAIReportAsync(answer);

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
            var reports = await _reportRepo.GetReportsBySessionAsync(sessionId);
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
