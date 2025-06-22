namespace Mango.Services.AuthAPI.Models.Dto
{
    public class UserInfoDto : UserDto
    {
        public string Role { get; set; } = string.Empty;
    }
}
