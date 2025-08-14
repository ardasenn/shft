using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SHFTAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>JWT token if successful</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            return await ValidateModelAndExecuteAsync<Token>(async () =>
            {
                var result = await _authService.LoginAsync(loginDto);
                return CreateResponse(result);
            });
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto">Registration information</param>
        /// <returns>Success message if registration is successful</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            return await ValidateModelAndExecuteAsync<string>(async () =>
            {
                var result = await _authService.RegisterAsync(registerDto);
                return CreateResponse(result);
            });
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New JWT token</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return CreateResponse(result);
        }

        /// <summary>
        /// User logout
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = GetCurrentUserId();
            var result = await _authService.LogoutAsync(userId);
            return CreateResponse(result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="changePasswordDto">Password change information</param>
        /// <returns>Success message if password is changed</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            // Ensure user can only change their own password
            var currentUserId = GetCurrentUserId();
            if (changePasswordDto.UserId != currentUserId && !IsAdmin())
            {
                return Forbid();
            }

            var result = await _authService.ChangePasswordAsync(changePasswordDto);
            return CreateResponse(result);
        }

        /// <summary>
        /// Forgot password - send reset token
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>Success message</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _authService.ForgotPasswordAsync(email);
            return CreateResponse(result);
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        /// <param name="resetPasswordDto">Reset password information</param>
        /// <returns>Success message if password is reset</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            return CreateResponse(result);
        }
    }
}
