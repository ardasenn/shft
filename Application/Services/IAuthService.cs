using Application.DTOs;
using Application.Utilities.Response;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<GenericResponse<Token>> LoginAsync(LoginDto loginDto);
        Task<GenericResponse<string>> RegisterAsync(RegisterDto registerDto);
        Task<GenericResponse<Token>> RefreshTokenAsync(string refreshToken);
        Task<GenericResponse<string>> LogoutAsync(string userId);
        Task<GenericResponse<string>> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<GenericResponse<string>> ForgotPasswordAsync(string email);
        Task<GenericResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
