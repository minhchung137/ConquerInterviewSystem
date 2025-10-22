﻿using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.Models;
using ConquerInterviewDAO; // Cần dùng DAO
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class PersonalizationRepository : IPersonalizationRepository
    {
        public PersonalizationRepository()
        {
            // Constructor này có thể trống nếu bạn dùng DAO Singleton
        }

        public async Task<List<Personalization>> CreatePersonalizationAsync(int customerId, int reportQId, List<ReportDTO> reports)
        {
            // 1. Gọi Python AI qua DAO
            var aiResponse = await PersonalizationDAO.Instance.CallAIPersonalizationAsync(reports);

            if (aiResponse?.PersonalizedPath == null || !aiResponse.PersonalizedPath.Any())
            {
                throw new Exception("AI did not return a valid personalization path.");
            }

            // 2. Chuyển đổi DTO từ AI sang Model của DB
            var dbStepsList = aiResponse.PersonalizedPath.Select(step => new Personalization
            {
                NamePractice = step.NamePractice,
                Practice = step.Practice,
                Exercise = step.Exercise,
                Objective = step.Objective,
                CustomerId = customerId, // Gán CustomerId
                ReportQId = reportQId     // Gán ReportQId
            }).ToList();

            // 3. Lưu danh sách các bước này vào DB
            return await PersonalizationDAO.Instance.SavePersonalizationStepsAsync(dbStepsList);
        }

        public async Task<List<Personalization>> GetPersonalizationByUserIdAsync(int customerId)
        {
            return await PersonalizationDAO.Instance.GetByUserIdAsync(customerId);
        }
    }
}