using System.Threading.Tasks;
using backend.Models;

namespace backend.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerDto">The registration details of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result is true if registration is successful.</returns>
        Task<bool> RegisterUserAsync(RegisterDTO registerDto);

        /// <summary>
        /// Authenticates a user based on provided credentials.
        /// </summary>
        /// <param name="loginDto">The login credentials of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result is the authenticated user or null if authentication fails.</returns>
        Task<AppUser> AuthenticateUserAsync(LoginDTO loginDto);
        
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>A task representing the asynchronous operation. The task result is the user if found, or null if not found.</returns>
        Task<AppUser> GetUserByUsernameAsync(string username);
    }
}
