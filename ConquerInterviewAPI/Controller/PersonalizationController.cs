using ConquerInterviewBO.Common; // Cần cho APIResponse
using ConquerInterviewBO.Commons; // Cần cho AppErrorCode
using ConquerInterviewBO.DTOs.Requests;
using ConquerInterviewBO.DTOs.Responses;
using ConquerInterviewServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConquerInterviewAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalizationController : ControllerBase
    {
        private readonly IPersonalizationService _service;
        private readonly ILogger<PersonalizationController> _logger;

        public PersonalizationController(IPersonalizationService service, ILogger<PersonalizationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lấy lộ trình học tập cá nhân hóa cho một user
        /// </summary>
        [HttpGet("user/{customerId}")]
        public async Task<IActionResult> Get(int customerId)
        {
            _logger.LogInformation("[GET] /api/Personalization/user/{CustomerId}", customerId);
            try
            {
                var res = await _service.GetPersonalizationByUserIdAsync(customerId);
                return StatusCode((int)ResponseStatus.Success, APIResponse<List<PersonalizationResponse>>.Success(res, "Success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong [GET] /api/Personalization/user/{CustomerId}", customerId);
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }

        /// <summary>
        /// Tạo một lộ trình học tập mới dựa trên các báo cáo
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PersonalizationRequest request)
        {
            _logger.LogInformation("[POST] /api/Personalization cho CustomerId: {CustomerId}", request.CustomerId);
            try
            {
                var res = await _service.CreatePersonalizationAsync(request);
                _logger.LogInformation("Tạo lộ trình thành công cho CustomerId: {CustomerId}", request.CustomerId);
                return StatusCode((int)ResponseStatus.Created, APIResponse<List<PersonalizationResponse>>.Success(res, "Personalization path created", ResponseStatus.Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong [POST] /api/Personalization");
                return StatusCode((int)ResponseStatus.InternalServerError, APIResponse<string>.Fail(AppErrorCode.InternalError, ResponseStatus.InternalServerError));
            }
        }
    }
}