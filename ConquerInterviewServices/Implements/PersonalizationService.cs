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
        private readonly ILogger<PersonalizationService> _logger;

        // Giả sử bạn đã tiêm (Inject) Repository và Logger
        public PersonalizationService(IPersonalizationRepository repo, ILogger<PersonalizationService> logger)
        {
            _repo = repo;
            _logger = logger;
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
    }
}