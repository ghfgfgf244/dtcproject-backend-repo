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

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Validate if user already exists
            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.Value == request.Email.Trim().ToLowerInvariant());
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
                Token = "" // Token generation will be handled in DEV-12 (Login/Authorization)
            };
        }
    }
}
