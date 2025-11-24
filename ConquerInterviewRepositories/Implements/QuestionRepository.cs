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
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ConquerInterviewDbContext _context;

        public QuestionRepository(ConquerInterviewDbContext context)
        {
            _context = context;
        }

        public Task<List<Question>> GenerateAIQuestionAsync(string topic, string industry, string jobPosition, int QuestionDifficult)
              => QuestionDAO.Instance.GenerateAIQuestionsAsync(topic, industry, jobPosition, QuestionDifficult);

        public Task<List<Question>> GetRandomQuestionsAsync(int count, int level)
            => QuestionDAO.Instance.GetRandomQuestionsAsync(count, level);
    }
}
