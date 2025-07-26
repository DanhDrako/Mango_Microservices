using log4net;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ResponseDto _response;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            try
            {
                var errorMessage = await _authService.Register(model);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    _response.IsSuccess = false;
                    _response.Message = errorMessage;
                    return BadRequest(_response);
                }
                _logger.Info($"Registration successfully with Email: {model.Email}");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred during registration for user {model.Email}", ex);
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                return StatusCode(500, _response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                var loginResponse = await _authService.Login(model);
                if (loginResponse.User == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Username or password is incorrect";
                    return BadRequest(_response);
                }
                _response.Result = loginResponse;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred during login for user {model.Email}", ex);
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                return StatusCode(500, _response);
            }
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            try
            {
                var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
                if (!assignRoleSuccessful)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error encountered";
                    return BadRequest(_response);
                }
                _logger.Info($"AssignRole successfully with Email: {model.Email} and Role: {model.Role}");
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred during registration for user {model.Email}", ex);
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                return StatusCode(500, _response);
            }
        }

        [HttpGet("user-info")]
        public async Task<IActionResult> UserInfo()
        {
            try
            {
                var userInfo = await _authService.UserInfo(Request.Headers.Authorization.ToString());
                _response.Result = userInfo;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching user info.", ex);
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                return StatusCode(500, _response);
            }

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _ = await _authService.Logout();
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred during logout.", ex);
                _response.IsSuccess = false;
                _response.Message = "An error occurred while processing your request.";
                return StatusCode(500, _response);
            }
        }

        [HttpPost("address")]
        public async Task<ResponseDto> CreateOrUpdateAddress(Address address)
        {
            try
            {
                var response = await _authService.CreateOrUpdateAddress(address, User.Identity.Name);
                if (response == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to create or update address.";
                    return _response;
                }
                _response.Result = response;
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while create or update address.", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpGet("address")]
        public async Task<ResponseDto> GetSavedAddress()
        {
            try
            {
                var response = await _authService.GetSavedAddress(User.Identity.Name);
                if (response == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to fetching address.";
                    return _response;
                }
                _response.Result = response;
            }
            catch (Exception ex)
            {
                _logger.Error("An error occurred while fetching address.", ex);
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
