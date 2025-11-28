using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class SessionDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static SessionDAO instance;
        public static SessionDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SessionDAO();
                }
                return instance;
            }
        }
        public SessionDAO()
        {
            _context = new ConquerInterviewDbContext();
        }

        public async Task<InterviewSession> CreateSessionAsync(int userId, string jobPosition, int durationMinutes)
        {
            var session = new InterviewSession
            {
                UserId = userId,
                JobPosition = jobPosition,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(durationMinutes),
                Status = "Active"
            };

            _context.InterviewSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<InterviewSession?> GetByIdAsync(int sessionId)
        {
            return await _context.InterviewSessions
                .Include(s => s.InterviewAnswers)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }
        // ✅ Thêm hàm gán danh sách câu hỏi vào session
        public async Task AssignQuestionsToSessionAsync(int sessionId, List<Question> questions)
        {
            var session = await _context.InterviewSessions
                .Include(s => s.InterviewAnswers)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                throw new Exception($"Session {sessionId} not found");

            foreach (var question in questions)
            {
                var answer = new InterviewAnswer
                {
                    SessionId = sessionId,
                    QuestionId = question.QuestionId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InterviewAnswers.Add(answer);
            }

            await _context.SaveChangesAsync();
        }

        // ✅ Lấy danh sách câu hỏi theo session
        public async Task<List<Question>> GetQuestionsBySessionAsync(int sessionId)
        {
            return await _context.InterviewAnswers
                .Include(a => a.Question)
                .Where(a => a.SessionId == sessionId)
                .Select(a => a.Question)
                .ToListAsync();
        }
        public async Task UpdateSessionAsync(InterviewSession session)
        {
            _context.InterviewSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int sessionId, string status)
        {
                 await _context.InterviewSessions
                .Where(s => s.SessionId == sessionId)
                .ExecuteUpdateAsync(s => s.SetProperty(sess => sess.Status, status));
        }
        public async Task<List<InterviewSession>> GetAllAsync()
        {
            return await _context.InterviewSessions
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }
    }
}
