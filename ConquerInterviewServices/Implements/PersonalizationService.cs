using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using Microsoft.Extensions.Logging; // Thêm ILogger
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class PersonalizationService : IPersonalizationService
    {
        private readonly IPersonalizationRepository _repo;
        private readonly IReportRepository _reportRepo;
        private readonly ILogger<PersonalizationService> _logger;

        // Giả sử bạn đã tiêm (Inject) Repository và Logger
        public PersonalizationService(IPersonalizationRepository repo, ILogger<PersonalizationService> logger, IReportRepository reportRepo)
        {
            _repo = repo;
            _logger = logger;
            _reportRepo = reportRepo;
        }

        public async Task<List<PersonalizationResponse>> CreatePersonalizationAsync(PersonalizationRequest request)
        {
            _logger.LogInformation("Creating personalization path for CustomerId: {CustomerId}", request.CustomerId);

            // 1. Gọi Repository để tạo và lưu
            var dbSteps = await _repo.CreatePersonalizationAsync(request.CustomerId, request.ReportQId, request.Reports);

            _logger.LogInformation("Successfully created {StepCount} steps for CustomerId: {CustomerId}", dbSteps.Count, request.CustomerId);

            // 2. Map từ Model (DB) sang DTO (Response)
            return dbSteps.Select(p => new PersonalizationResponse
            {
                PersonalizationId = p.PersonalizationId,
                NamePractice = p.NamePractice,
                Practice = p.Practice,
                Exercise = p.Exercise,
                Objective = p.Objective,
                CustomerId = p.CustomerId,
                ReportQId = p.ReportQId
            }).ToList();
        }

        
        public async Task<List<PersonalizationResponse>> CreatePersonalizationSessionIdAsync(int sessionId, int currentUserId)
            {
                _logger.LogInformation("Creating personalization path for SessionId: {SessionId} and CustomerId: {CustomerId}", sessionId, currentUserId);

                // 1. Lấy tất cả báo cáo (ReportQuestion) của Session này từ DB
                // Cần một hàm mới trong ReportDAO để lấy báo cáo theo SessionId
                var reportQuestions = await _reportRepo.GetReportsBySessionAsync(sessionId);

                if (reportQuestions == null || !reportQuestions.Any())
                {
                    throw new AppException(AppErrorCode.QuestionNotFound);
                }

                // Đảm bảo người dùng sở hữu session này (Security Check)
                if (reportQuestions.First().CustomerId != currentUserId)
                {
                    throw new AppException(AppErrorCode.UnauthorizedAccess);
                }

                // 2. Chuyển đổi ReportQuestion Entity sang ReportDTO cho AI Python
                var reportsForAI = reportQuestions.Select(r => new ReportDTO
                {
                    // Giả định ReportDTO cần các trường đánh giá nằm trong r.OverallAssessment, etc.
                    OverallAssessment = r.OverallAssessment,
                    FacialExpression = r.FacialExpression,
                    ExpertiseExperience = r.ExpertiseExperience,
                    // ... (Map tất cả các trường đánh giá khác từ r)
                }).ToList();

                // 3. Gọi AI Python qua DAO (chuyển đổi logic của hàm CreatePersonalizationAsync cũ)
                var dbSteps = await _repo.CallAIPersonalizationAndSaveAsync(currentUserId, reportsForAI, reportQuestions.First().ReportQId);

                _logger.LogInformation("Successfully created {StepCount} steps for SessionId: {SessionId}", dbSteps.Count, sessionId);

                // 4. Map từ Model (DB) sang DTO (Response)
                return dbSteps.Select(p => new PersonalizationResponse
                {
                    PersonalizationId = p.PersonalizationId,
                    NamePractice = p.NamePractice,
                    Practice = p.Practice,
                    Exercise = p.Exercise,
                    Objective = p.Objective,
                    CustomerId = p.CustomerId,
                    ReportQId = p.ReportQId
                }).ToList();
            }

        public async Task<List<PersonalizationResponse>> GetPersonalizationByUserIdAsync(int customerId)
        {
            _logger.LogInformation("Getting personalization path for CustomerId: {CustomerId}", customerId);

            // 1. Lấy dữ liệu từ Repo
            var dbSteps = await _repo.GetPersonalizationByUserIdAsync(customerId);

            // 2. Map từ Model (DB) sang DTO (Response)
            return dbSteps.Select(p => new PersonalizationResponse
            {
                PersonalizationId = p.PersonalizationId,
                NamePractice = p.NamePractice,
                Practice = p.Practice,
                Exercise = p.Exercise,
                Objective = p.Objective,
                CustomerId = p.CustomerId,
                ReportQId = p.ReportQId
            }).ToList();
        }

        public async Task<List<PersionalAllResponse>> GetPersonalizationHistoryAsync(int userId)
        {
            // 1. Gọi Repository/DAO để lấy tất cả Personalization Entity theo CustomerId
            // DAO PHẢI tải lồng ReportQ, InterviewA, Session, và Question
            var dbHistory = await _repo.GetPersonalizationsByCustomerIdAsync(userId);

            if (dbHistory == null || !dbHistory.Any())
            {
                throw new AppException(AppErrorCode.UserNotFound);
            }

            // 2. Gom nhóm các bước cá nhân hóa (Personalization) theo SessionId
            var groupedBySession = dbHistory
                .GroupBy(p => p.ReportQ.InterviewA.SessionId)
                .Select(group =>
                {
                    // Lấy bước đầu tiên trong nhóm để truy cập thông tin Session
                    var firstItem = group.First();
                    var session = firstItem.ReportQ.InterviewA.Session;

                    // 3. Ánh xạ sang PersionalAllResponse
                    return new PersionalAllResponse
                    {
                        SessionId = group.Key, // SessionId là Key của nhóm
                        SessionDate = session?.StartTime,

                        // 4. Ánh xạ các bước cá nhân hóa trong nhóm sang DTO
                        Personalizations = group.Select(p => new PersonalizationHistoryItemResponse
                        {
                            // Lấy các trường cơ bản từ Personalization
                            PersonalizationId = p.PersonalizationId,
                            NamePractice = p.NamePractice,
                            Practice = p.Practice,
                            Exercise = p.Exercise,
                            Objective = p.Objective,

                            // Lấy Question Text và các Khóa ngoại liên quan
                            QuestionText = p.ReportQ.InterviewA.Question.QuestionText, // Cần tải Entity Question trong DAO
                            ReportQId = p.ReportQId,
                            SessionId = group.Key,

                        }).ToList()
                    };
                })
                // 5. Sắp xếp Sessions theo ngày mới nhất (tùy chọn)
                .OrderByDescending(res => res.SessionDate)
                .ToList();

            return groupedBySession;
        }
    }
}