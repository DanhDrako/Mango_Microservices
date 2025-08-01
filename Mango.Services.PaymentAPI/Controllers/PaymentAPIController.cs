using log4net;
using Mango.Services.PaymentAPI.Models.Dto;
using Mango.Services.PaymentAPI.Models.Dto.Payment;
using Mango.Services.PaymentAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.PaymentAPI.Controllers
{
    [Route("api/payment")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ResponseDto _response;
        private readonly IPaymentService _paymentService;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentController(IPaymentService paymentService)
        {
            _response = new();
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ResponseDto> CreateOrUpdatePaymentIntent(PaymentDto paymentDto)
        {
            try
            {
                var result = await _paymentService.CreateOrUpdatePayment(paymentDto);
                if (result == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Payment create or update failed.";
                    return _response;
                }
                _logger.Info($"Order updated successfully with ID: {paymentDto.PaymentIntentId}");
                _response.Result = result;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while create or update payment intent:", ex);
                _response.IsSuccess = false;
                _response.Message = $"Error create or update payment intent: {ex.Message}";
            }
            return _response;
        }
    }
}
