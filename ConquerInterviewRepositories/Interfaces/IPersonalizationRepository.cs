using ConquerInterviewBO.DTOs.Requests; // Cần ReportDTO
using ConquerInterviewBO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IPersonalizationRepository
    {
        // Lấy lộ trình
        Task<List<Personalization>> GetPersonalizationByUserIdAsync(int customerId);

        // Tạo lộ trình
        Task<List<Personalization>> CreatePersonalizationAsync(int customerId, int reportQId, List<ReportDTO> reports);

        Task<List<Personalization>> GetPersonalizationsByCustomerIdAsync(int customerId);
        Task<List<Personalization>> CallAIPersonalizationAndSaveAsync(int customerId, List<ReportDTO> reports, int representativeReportQId);
    }
}