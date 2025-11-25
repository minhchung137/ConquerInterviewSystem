using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.Models;
using ConquerInterviewDAO;
using ConquerInterviewRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Implements
{
    public class ReportRepository : IReportRepository
    {
        private readonly ConquerInterviewDbContext _context;

        public ReportRepository(ConquerInterviewDbContext context)
        {
            _context = context;
        }

        public Task<ReportQuestion> GenerateAIReportAsync(InterviewAnswer answer, string question)
            => ReportDAO.Instance.GenerateAIReportAsync(answer, question);

        public Task<List<ReportQuestion>> GetReportsBySessionAsync(int sessionId)
            => ReportDAO.Instance.GetReportsBySessionAsync(sessionId);

        public Task<InterviewSession> GetSessionByIdAsync(int sessionId)
            => SessionDAO.Instance.GetByIdAsync(sessionId);

        public Task<List<ReportQuestion>> GetReportsByUserAsync(int userId)
            => ReportDAO.Instance.GetReportsByUserAsync(userId);
    }
}
