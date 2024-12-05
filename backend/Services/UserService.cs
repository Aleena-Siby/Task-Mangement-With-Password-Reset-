using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;
using backend.Models;
using BCrypt.Net;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUserAsync(RegisterDTO registerDto)
        {
            // Check if the user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            // Create a new user
            var newUser = new AppUser
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password) // Hash the password
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AppUser> AuthenticateUserAsync(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            if(user!=null)
     {       Console.WriteLine("User Found") ;

          if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {  Console.WriteLine("Password is correct");
                return user; // Return the authenticated user
                
            }}
            Console.WriteLine("Password Incorrect");
            return null; // Authentication failed
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
