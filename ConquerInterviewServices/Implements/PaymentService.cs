using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewRepositories.Interfaces;
using ConquerInterviewServices.Interfaces;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace ConquerInterviewServices.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;

        public PaymentService(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(CreatePaymentLinkRequest request)
        {
            return await _repo.CreatePaymentLinkAsync(request);
        }

        public async Task ProcessWebhookAsync(WebhookType webhookBody)
        {
            await _repo.ProcessWebhookAsync(webhookBody);
        }
    }
}