using Microsoft.AspNetCore.Mvc;
using backend.Models; // Adjust based on your project structure
using backend.Services; // Ensure you're using the correct service interface
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly SendGridEmailService _emailService;

        public AuthController(AppDbContext dbContext, IUserService userService, IConfiguration configuration, SendGridEmailService emailService, ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping endpoint hit.");
            return Ok("Server is running!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            _logger.LogInformation("Register endpoint called.");

            if (registerDto == null)
            {
                _logger.LogWarning("Invalid user data received for registration.");
                return BadRequest("Invalid user data.");
            }

            var result = await _userService.RegisterUserAsync(registerDto);

            if (result)
            {
                _logger.LogInformation("User {Username} registered successfully.", registerDto.Username);
                return Ok("User registered successfully.");
            }

            _logger.LogError("User registration failed for {Username}.", registerDto.Username);
            return BadRequest("User registration failed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation("Login endpoint called.");

            if (loginDto == null)
            {
                _logger.LogWarning("Invalid login data received.");
                return BadRequest("Invalid login data.");
            }

            var user = await _userService.AuthenticateUserAsync(loginDto);

            if (user != null)
            {
                _logger.LogInformation("User {Username} authenticated successfully.", loginDto.Username);

                var token = GenerateJwtToken(user);
                _logger.LogInformation("JWT token generated for user {Username}.", loginDto.Username);

                return Ok(new { token });
            }

            _logger.LogWarning("Invalid login attempt for username: {Username}.", loginDto.Username);
            return Unauthorized("Invalid username or password.");
        }

        private string GenerateJwtToken(AppUser user)
        {
            _logger.LogInformation("Generating JWT token for user {Username}.", user.Username);

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("id", user.Id.ToString(), ClaimValueTypes.Integer),
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            _logger.LogInformation("JWT token successfully generated.");
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            _logger.LogInformation("Password reset request received for email: {Email}", request.Email);

            if (string.IsNullOrEmpty(request.Email))
            {
                _logger.LogWarning("Password reset request failed due to missing email.");
                return BadRequest("Email is required.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                return Ok("If the email exists, a reset link has been sent.");
            }

            var resetToken = Guid.NewGuid().ToString();
            _logger.LogInformation("Generated reset token for user: {Email}", request.Email);

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(30);
            await _dbContext.SaveChangesAsync();

            var resetLink = $"{_configuration["Frontend:ResetPasswordUrl"]}?token={resetToken}";
            _logger.LogInformation("Reset link generated for user {Email}: {ResetLink}", request.Email, resetLink);

            try
            {
                await _emailService.SendEmailAsync(
                    request.Email,
                    "Password Reset Request",
                    $"<p>Please click the link below to reset your password:</p><p><a href='{resetLink}'>Reset Password</a></p>"
                );

                _logger.LogInformation("Password reset email sent successfully to {Email}.", request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}.", request.Email);
                return StatusCode(500, "Error sending email. Please try again later.");
            }

            return Ok("If the email exists, a reset link has been sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword request)
        {
            _logger.LogInformation("Reset password request received.");

            if (string.IsNullOrEmpty(request.ResetToken) || string.IsNullOrEmpty(request.NewPassword))
            {
                _logger.LogWarning("Reset password request failed due to missing token or password.");
                return BadRequest("Reset token and new password are required.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.ResetToken);
            if (user == null)
            {
                _logger.LogWarning("Invalid reset token used.");
                return BadRequest("Invalid or expired reset token.");
            }

            if (user.PasswordResetTokenExpiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Reset token expired for email: {Email}.", user.Email);
                return BadRequest("The reset token has expired. Please request a new password reset.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Password reset successfully for user {Email}.", user.Email);
            return Ok("Password has been reset successfully.");
        }
    }

        public class PasswordResetRequest
    {
        public string Email { get; set; }
    }

    public class ResetPassword
    {
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}


