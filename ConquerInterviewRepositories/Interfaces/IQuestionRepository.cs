using ConquerInterviewBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetRandomQuestionsAsync(int count, int level);
        Task<Question> GenerateAIQuestionAsync(string topic, string industry, string jobPosition);
    }
}
