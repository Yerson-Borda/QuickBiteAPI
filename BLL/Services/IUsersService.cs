using BLL.Configuration;
using DAL.Data;
using DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public interface IUsersService
    {
        Task Register(UserCreateDto model);
        Task<string> Login(LoginCredentialsDto model);
        Task<UserPublicModelDto> GetProfile(string email);
        Task UpdateProfile(string email, EditProfileDto model);
    }

    public class UsersService : IUsersService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtBearerTokenSettings _jwtTokenSettings;
        private readonly ApplicationDbContext _context;
        public UsersService(UserManager<User> userManager, IOptions<JwtBearerTokenSettings> options, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtTokenSettings = options.Value;
            _context = context;
        }
        public async Task Register(UserCreateDto model)
        {
            var existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                throw new ArgumentException("User with same email already exists");
            }
            var identityUser = new User()
            {
                Name = model.Name,
                Email = model.Email,
                BirthDate = DateOnly.FromDateTime(model.BirthDate),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception($"Some errors during creating user! Data: {result.Errors}");
            }
        }

        public async Task<string> Login(LoginCredentialsDto model)
        {
            var user = await ValidateUser(model);
            return await GenerateToken(user);
        }

        public async Task<UserPublicModelDto> GetProfile(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email: {email} was not found!");
            }
            // Return everytime a new DTO, because if we recycle model they may contain sensitive information for the frontend - user
            return new UserPublicModelDto
            {
                BirthDate = user.BirthDate,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task UpdateProfile(string email, EditProfileDto model)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email: {email} was not found!");
            }

            user.Name = model.Name;
            user.BirthDate = DateOnly.FromDateTime(model.BirthDate);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Some errors during updating user! Data: {result.Errors}");
            }
        }

        private async Task<User> ValidateUser(LoginCredentialsDto credentials)
        {
            var identityUser = await _userManager.FindByEmailAsync(credentials.Email);
            if (identityUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash,
                    credentials.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    return identityUser;
                }
            }
            throw new ArgumentException("Login data was incorrect");
        }
        private async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtTokenSettings.SecretKey);
            var descriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtTokenSettings.Audience,
                Issuer = _jwtTokenSettings.Issuer,
                Expires = DateTime.UtcNow.AddSeconds(_jwtTokenSettings.ExpiryTimeInSeconds),
                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("Id", user.Id.ToString()),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var logoutIds = await _context.LogoutUsers
                .Where(x => x.Identifier == user.Id && !x.DeleteDateTime.HasValue)
                .ToListAsync();
            _context.LogoutUsers.RemoveRange(logoutIds);
            await _context.SaveChangesAsync();

            var token = tokenHandler.CreateToken(descriptor);
            var userToken = tokenHandler.WriteToken(token);
            return userToken;
        }
    }
}
