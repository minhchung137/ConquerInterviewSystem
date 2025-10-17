using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewDAO
{
    public class AnswerDAO
    {
        private readonly ConquerInterviewDbContext _context;
        private static AnswerDAO instance;
        public static AnswerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnswerDAO();
                }
                return instance;
            }
        }
        public AnswerDAO()
        {
            _context = new ConquerInterviewDbContext();
        }

        public async Task<InterviewAnswer> SaveAnswerAsync(SubmitAnswerRequest request)
        {
            // ✅ 1️⃣ Kiểm tra session hợp lệ
            var session = await _context.InterviewSessions
                .FirstOrDefaultAsync(s => s.SessionId == request.SessionId);

            if (session == null)
                throw new Exception($"Session ID {request.SessionId} not found.");

            // ✅ 2️⃣ Kiểm tra thời gian session còn hiệu lực
            var now = DateTime.UtcNow;
            if (now < session.StartTime || now > session.EndTime)
                throw new Exception("Interview session has ended or not started yet.");

            // ✅ 3️⃣ Kiểm tra question hợp lệ
            var questionExists = await _context.Questions.AnyAsync(q => q.QuestionText == request.QuestionText);
            if (!questionExists)
                throw new Exception($"Question ID {request.QuestionText} not found.");

            // ✅ 4️⃣ Tìm answer hiện có (SessionId + QuestionId)
            var existing = await _context.InterviewAnswers
                .FirstOrDefaultAsync(a => a.SessionId == request.SessionId && a.QuestionId == request.QuestionId);

            if (existing != null)
            {
                // 🔁 Update nội dung
                existing.TextAnswer = request.AnswerText;
                existing.CreatedAt = DateTime.UtcNow;

                _context.InterviewAnswers.Update(existing);
                await _context.SaveChangesAsync();

                return existing;
            }

            // ✅ 5️⃣ Nếu chưa có, tạo mới
            var newAnswer = new InterviewAnswer
            {
                SessionId = request.SessionId,
                QuestionId = request.QuestionId,
                TextAnswer = request.AnswerText,
                CreatedAt = DateTime.UtcNow
            };

            _context.InterviewAnswers.Add(newAnswer);
            await _context.SaveChangesAsync();

            return newAnswer;
        }


        public async Task<List<InterviewAnswer>> GetAnswersBySessionAsync(int sessionId)
        {
            return await _context.InterviewAnswers
                .Include(a => a.Question)
                .Where(a => a.SessionId == sessionId)
                .ToListAsync();
        }
    }
}
