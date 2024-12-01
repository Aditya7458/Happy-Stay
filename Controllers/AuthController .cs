using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cozy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if username or email already exists
                var users = await _userRepository.GetAllUsersAsync();
                if (users.Any(u => u.Username == registerRequest.Username))
                    return BadRequest("Username already exists.");
                if (users.Any(u => u.Email == registerRequest.Email))
                    return BadRequest("Email already exists.");

                // Validate role
                var validRoles = new[] { "Guest", "HotelOwner", "Admin" };
                if (!validRoles.Contains(registerRequest.Role))
                    return BadRequest("Invalid role. Allowed roles are 'Guest', 'HotelOwner', 'Admin'.");

                // Hash password (in production, use a secure hashing algorithm)
                var newUser = new User
                {
                    Username = registerRequest.Username,
                    PasswordHash = registerRequest.Password, // Hash in real app
                    Email = registerRequest.Email,
                    Role = registerRequest.Role, // Assign role from request
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.AddUserAsync(newUser);
                return Ok(new { Message = "User registered successfully", UserId = createdUser.UserID });
            }
            catch (Exception ex)
            {
                // Log the exception details (consider using a logging framework)
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var users = await _userRepository.GetAllUsersAsync();
                var matchedUser = users.FirstOrDefault(u =>
                    u.Email == loginRequest.Email && // Search by email
                    u.PasswordHash == loginRequest.Password); // Use hashed password comparison in production

                if (matchedUser == null)
                    return Unauthorized("Invalid email or password.");

                var token = GenerateJwtToken(matchedUser);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Log the exception details (consider using a logging framework)
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)  // Ensure Role is added here
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO classes for Login and Registration
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 255 characters.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
        public string? Role { get; set; }
    }
}
