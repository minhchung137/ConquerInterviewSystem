using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface ISessionService
    {
        Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request);
        Task<List<QuestionResponse>> GetQuestionsBySessionAsync(int sessionId);
        Task<AnswerReportResponse> SubmitAnswerAsync(SubmitAnswerRequest request);
        Task<ReportResponse> GetReportBySessionAsync(int sessionId, int currentUserId);
        Task UpdateStatusAsync(int sessionId, string status);
        
        Task<List<ReportResponse>> GetReportsByUserIdAsync(int currentUserId);
    }
}
