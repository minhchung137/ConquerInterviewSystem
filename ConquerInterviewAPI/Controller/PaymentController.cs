using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService service, ILogger<PaymentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentLinkRequest request)
        {
            try
            {
                var result = await _service.CreatePaymentLinkAsync(request);
                return StatusCode((int)ResponseStatus.Success, APIResponse<PaymentLinkResponse>.Success(result, "Link created"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating link");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        [HttpPost("webhook/payos")]
        public async Task<IActionResult> PayOSWebhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                await _service.ProcessWebhookAsync(webhookBody);
                return Ok(new { message = "Webhook received" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook error");
                return Ok(new { message = "Received with error" });
            }
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            _logger.LogInformation($"Cancelling order {orderId}");
            try
            {
                var result = await _service.CancelOrderAsync(orderId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<bool>.Success(result, "Order cancelled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var result = await _service.GetAllPaymentsAsync();
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<PaymentResponse>>.Success(result, "Get all payments success"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUser(int userId)
        {
            try
            {
                var result = await _service.GetPaymentsByUserIdAsync(userId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<PaymentResponse>>.Success(result, "Get user payments success"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError));
            }
        }


    }
}