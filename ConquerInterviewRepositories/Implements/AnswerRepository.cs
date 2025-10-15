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
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ConquerInterviewDbContext _context;

        public AnswerRepository(ConquerInterviewDbContext context)
        {
            _context = context;
        }
        public Task<InterviewAnswer> SaveAnswerAsync(SubmitAnswerRequest request)
            => AnswerDAO.Instance.SaveAnswerAsync(request);

        public Task<List<InterviewAnswer>> GetAnswersBySessionAsync(int sessionId)
            => AnswerDAO.Instance.GetAnswersBySessionAsync(sessionId);
    }
}
