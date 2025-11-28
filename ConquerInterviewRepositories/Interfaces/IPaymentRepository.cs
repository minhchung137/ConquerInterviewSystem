using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace ConquerInterviewRepositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request);
        Task ProcessWebhookAsync(WebhookType webhookBody);
    }
}