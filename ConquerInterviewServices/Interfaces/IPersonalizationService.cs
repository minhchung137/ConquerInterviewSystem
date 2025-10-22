using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IPersonalizationService
    {
        Task<List<PersonalizationResponse>> GetPersonalizationByUserIdAsync(int customerId);
        Task<List<PersonalizationResponse>> CreatePersonalizationAsync(PersonalizationRequest request);
    }
}