using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Auth.DTOs;
using dtc.Application.Features.Auth.Interfaces;
using dtc.Application.Features.Auth.DTOs;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces;
using dtc.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace dtc.Application.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var targetEmail = Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User(
                email: Email.Create(request.Email),
                passwordHash: passwordHash,
                fullName: request.FullName,
                phone: PhoneNumber.Create(request.Phone)
            );

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = newUser.Id,
                Email = newUser.Email.Value,
                FullName = newUser.FullName,
                Token = ""
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var targetEmail = Email.Create(request.Email);
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail, u => u.Roles);
            
            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new Exception("Your account has been banned or deactivated.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            user.UpdateLastLogin();
            await _unitOfWork.SaveChangesAsync();

            string token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email.Value,
                FullName = user.FullName,
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var claimsList = new System.Collections.Generic.List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email.Value),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.FullName)
            };

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claimsList.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role.RoleName.ToString()));
                }
            }

            var claims = claimsList.ToArray();

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:ExpireDays"] ?? "7")),
                signingCredentials: credentials);

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
