﻿using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.Email.ToLower());

            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || !isValid)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            // if user was found, Generate JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(x => x.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = registrationRequestDto.Email,
                        ID = userToReturn.Id,
                        Name = registrationRequestDto.Name,
                        PhoneNumber = registrationRequestDto.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    var message = string.Join(",", result.Errors.Select(e => e.Description));
                    return message;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<UserInfoDto> UserInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var jwtEncoded = token["Bearer ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(jwtEncoded))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(jwtEncoded))
            {
                return null;
            }

            var jwtDecoded = handler.ReadToken(jwtEncoded) as JwtSecurityToken;
            if (jwtDecoded == null)
            {
                return null;
            }

            var claims = jwtDecoded.Claims
            .Where(c => c.Type == "email" || c.Type == "role" || c.Type == "sub")
            .ToList();

            if (!claims.Any())
            {
                return null;
            }

            var userInfo = new UserInfoDto
            {
                ID = claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                Email = claims.FirstOrDefault(c => c.Type == "email")?.Value,
                Role = claims.FirstOrDefault(c => c.Type == "role")?.Value
            };
            return await Task.FromResult(userInfo);
        }

        public Task<bool> Logout()
        {
            return Task.FromResult(true);
        }
    }
}