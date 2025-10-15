using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task<InterviewAnswer> SaveAnswerAsync(SubmitAnswerRequest request);
        Task<List<InterviewAnswer>> GetAnswersBySessionAsync(int sessionId);
    }
}
