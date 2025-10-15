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
        Task<ReportQuestion> GenerateAIReportAsync(InterviewAnswer answer);
        Task<List<ReportQuestion>> GetReportsBySessionAsync(int sessionId);
    }
}
