using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request);
        Task ProcessWebhookAsync(WebhookType webhookBody);
        Task<bool> CancelOrderAsync(int orderId);
        Task<List<PaymentResponse>> GetAllPaymentsAsync();
        Task<List<PaymentResponse>> GetPaymentsByUserIdAsync(int userId);
    }
}