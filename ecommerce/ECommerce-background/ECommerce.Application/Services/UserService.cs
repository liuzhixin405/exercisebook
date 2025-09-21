using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;
using static BCrypt.Net.BCrypt;

namespace ECommerce.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if user with email already exists
            if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
                throw new InvalidOperationException($"User with email {createUserDto.Email} already exists");

            var user = new User
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                PhoneNumber = createUserDto.PhoneNumber,
                Address = createUserDto.Address,
                Role = createUserDto.Role,
                IsActive = true
            };

            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Created user: {UserId} - {Email}", 
                createdUser.Id, createdUser.Email);
            
            return MapToDto(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new ArgumentException($"User with id {id} not found");

            existingUser.FirstName = updateUserDto.FirstName;
            existingUser.LastName = updateUserDto.LastName;
            existingUser.PhoneNumber = updateUserDto.PhoneNumber;
            existingUser.Address = updateUserDto.Address;
            existingUser.IsActive = updateUserDto.IsActive;
            existingUser.Role = updateUserDto.Role;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            _logger.LogInformation("Updated user: {UserId} - {Email}", 
                updatedUser.Id, updatedUser.Email);
            
            return MapToDto(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Deleted user: {UserId}", id);
            }
            return result;
        }

        public async Task<bool> AuthenticateUserAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
                return false;

            var isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (isValidPassword)
            {
                await _userRepository.UpdateLastLoginAsync(user.Id);
                _logger.LogInformation("User authenticated: {UserId} - {Email}", 
                    user.Id, user.Email);
            }

            return isValidPassword;
        }

        public async Task<bool> UpdateLastLoginAsync(Guid id)
        {
            return await _userRepository.UpdateLastLoginAsync(id);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                Role = user.Role
            };
        }
    }
}
