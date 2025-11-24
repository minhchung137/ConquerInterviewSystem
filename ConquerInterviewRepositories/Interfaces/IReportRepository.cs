using ConquerInterviewBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IReportRepository
    {
        Task<ReportQuestion> GenerateAIReportAsync(InterviewAnswer answer, string question);
        Task<List<ReportQuestion>> GetReportsBySessionAsync(int sessionId);

        Task<InterviewSession> GetSessionByIdAsync(int sessionId);
    }
}
