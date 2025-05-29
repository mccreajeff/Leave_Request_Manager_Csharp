using LeaveRequestManager.Data;
using LeaveRequestManager.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveRequestManager.Services
{
    /// <summary>
    /// Provides authentication and session management services for the application.
    /// Handles user login validation and maintains the current user session.
    /// </summary>
    public static class AuthService
    {
        #region Private Fields

        /// <summary>
        /// Stores the currently authenticated user for the session.
        /// </summary>
        private static User? _currentUser;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the currently authenticated user.
        /// Returns null if no user is logged in.
        /// </summary>
        public static User? CurrentUser => _currentUser;

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to authenticate a user with the provided credentials.
        /// </summary>
        /// <param name="username">The username to authenticate</param>
        /// <param name="password">The password to verify</param>
        /// <returns>The authenticated User object if successful, null otherwise</returns>
        public static async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                using var context = new AppDbContext();
                
                // Query for user by username (case-insensitive)
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);

                if (user == null)
                {
                    return null; // User not found or inactive
                }

                // Verify password using BCrypt
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return null; // Invalid password
                }

                // Set current user for the session
                _currentUser = user;
                return user;
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging framework)
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Logs out the current user by clearing the session.
        /// </summary>
        public static void Logout()
        {
            _currentUser = null;
        }

        /// <summary>
        /// Checks if a user is currently authenticated.
        /// </summary>
        /// <returns>True if a user is logged in, false otherwise</returns>
        public static bool IsAuthenticated()
        {
            return _currentUser != null;
        }

        /// <summary>
        /// Checks if the current user has a specific role.
        /// </summary>
        /// <param name="role">The role to check for</param>
        /// <returns>True if the user has the specified role, false otherwise</returns>
        public static bool HasRole(string role)
        {
            return _currentUser?.Role == role;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies a password against a BCrypt hash.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="passwordHash">The BCrypt hash to verify against</param>
        /// <returns>True if the password matches, false otherwise</returns>
        private static bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                // Return false on any BCrypt error
                return false;
            }
        }

        #endregion
    }
} 