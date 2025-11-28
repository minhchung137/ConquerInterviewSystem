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
    public class SessionRepository : ISessionRepository
    {
        private readonly ConquerInterviewDbContext _context;

        public SessionRepository(ConquerInterviewDbContext context)
        {
            _context = context;
        }

        public Task<InterviewSession> CreateSessionAsync(int userId, string jobPosition, int durationMinutes)
            => SessionDAO.Instance.CreateSessionAsync(userId, jobPosition, durationMinutes);

        public Task<InterviewSession?> GetByIdAsync(int sessionId)
            => SessionDAO.Instance.GetByIdAsync(sessionId);

        public Task AssignQuestionsToSessionAsync(int sessionId, List<Question> questions)
            => SessionDAO.Instance.AssignQuestionsToSessionAsync(sessionId, questions);

        public Task<List<Question>> GetQuestionsBySessionAsync(int sessionId)
            => SessionDAO.Instance.GetQuestionsBySessionAsync(sessionId);
        public Task UpdateSessionAsync(InterviewSession session)
            => SessionDAO.Instance.UpdateSessionAsync(session);
        public Task UpdateStatusAsync(int sessionId, string status)
            => SessionDAO.Instance.UpdateStatusAsync(sessionId, status);
        public Task<List<InterviewSession>> GetAllSessionsAsync()
        => SessionDAO.Instance.GetAllAsync();
    }
}
