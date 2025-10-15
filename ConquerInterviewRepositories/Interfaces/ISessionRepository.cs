using ConquerInterviewBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface ISessionRepository
    {
        Task<InterviewSession> CreateSessionAsync(int userId, string jobPosition, int durationMinutes);
        Task<InterviewSession?> GetByIdAsync(int sessionId);
        Task AssignQuestionsToSessionAsync(int sessionId, List<Question> questions); // nếu bạn dùng lưu mapping
        Task<List<Question>> GetQuestionsBySessionAsync(int sessionId);
    }
}
