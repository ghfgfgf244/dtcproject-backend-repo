using dtc.Application.DTOs.Auth;
using dtc.Application.Interfaces;
using dtc.Domain.Entities.Permissions;
using dtc.Domain.Interfaces;
using dtc.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace dtc.Application.Services
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
            // 1. Validate if user already exists
            var targetEmail = Email.Create(request.Email);
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == targetEmail);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            // 2. Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Create User Entity
            var newUser = new User(
                email: Email.Create(request.Email),
                passwordHash: passwordHash,
                fullName: request.FullName,
                phone: PhoneNumber.Create(request.Phone)
            );

            // 4. Save to Database
            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // 5. Return Response
            return new AuthResponseDto
            {
                UserId = newUser.Id,
                Email = newUser.Email.Value,
                FullName = newUser.FullName,
                Token = "" // Will generate later if needed, but for now Login is required
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. Find User
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

            // 2. Verify Password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            // 3. Update Last Login (Assuming UnitOfWork tracks changes)
            user.UpdateLastLogin();
            await _unitOfWork.SaveChangesAsync();

            // 4. Generate JWT Token
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
