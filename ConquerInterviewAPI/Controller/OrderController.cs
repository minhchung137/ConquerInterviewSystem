using ConquerInterviewBO.Common;
using ConquerInterviewBO.Commons;
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService service, ILogger<OrderController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Tạo một đơn hàng mới (Trạng thái Pending)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode((int)ResponseStatus.BadRequest, APIResponse<string>.Fail(AppErrorCode.InvalidInput, ResponseStatus.BadRequest));
            }

            _logger.LogInformation("[POST] /api/Order - UserId: {UserId}, PlanId: {PlanId}", request.UserId, request.PlanId);
            try
            {
                var newOrder = await _service.CreateOrderAsync(request);
                return StatusCode((int)ResponseStatus.Created, APIResponse<OrderResponse>.Success(newOrder, "Order created", ResponseStatus.Created));
            }
            catch (AppException ex) when (ex.ErrorCode == AppErrorCode.UserNotFound || ex.ErrorCode == AppErrorCode.SubscriptionPlanNotFound || ex.ErrorCode == AppErrorCode.PlanIsInactive)
            {
                _logger.LogWarning("Tạo Order thất bại: {Message}", ex.Message);
                return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(ex.ErrorCode, ResponseStatus.NotFound));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [POST] /api/Order");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một đơn hàng
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            _logger.LogInformation("[GET] /api/Order/{OrderId}", orderId);
            try
            {
                var order = await _service.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Không tìm thấy Order ID: {OrderId}", orderId);
                    return StatusCode((int)ResponseStatus.NotFound, APIResponse<string>.Fail(AppErrorCode.OrderNotFound, ResponseStatus.NotFound));
                }
                return StatusCode((int)ResponseStatus.Success, APIResponse<OrderResponse>.Success(order, "Success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong [GET] /api/Order/{OrderId}", orderId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
    }
}