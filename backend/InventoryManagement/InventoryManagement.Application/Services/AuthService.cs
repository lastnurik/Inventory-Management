using InventoryManagement.Application.DTO;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository authRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User with that email does not exist.");
            }

            if (user.IsBlocked)
            {
                throw new UnauthorizedAccessException("This account is blocked.");
            }

            var validPassword = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!validPassword)
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            var roles = await _userRepository.GetUserRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles[0]);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Name = user.Name,
                Role = roles.FirstOrDefault() ?? "User",
            };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.FindByEmailAsync(registerDto.Email);

            if (existingUser != null)
            {
                return false;
            }

            var user = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                Name = registerDto.Name,
            };

            var result = await _userRepository.AddAsync(user, registerDto.Password);
            return result.Succeeded;
        }
    }
}
