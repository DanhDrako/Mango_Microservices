using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);
        Task<UserInfoDto> UserInfo(string token);
        Task<bool> Logout();
        Task<Address?> CreateOrUpdateAddress(Address address, string userName);
        Task<Address?> GetSavedAddress(string userName);
    }
}
