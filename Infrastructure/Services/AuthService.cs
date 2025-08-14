using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.Services;
using Application.Utilities.Constants;
using Application.Utilities.Response;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<GenericResponse<Token>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return GenericResponse<Token>.Fail(Messages.UserNotFound, 404);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return GenericResponse<Token>.Fail(Messages.InvalidCredentials, 401);
                }

                var token = await GenerateTokenAsync(user);
                return GenericResponse<Token>.Success(token, Messages.LoginSuccessful);
            }
            catch (Exception ex)
            {
                return GenericResponse<Token>.Fail($"{Messages.LoginFailed}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                if (registerDto.Password != registerDto.ConfirmPassword)
                {
                    return GenericResponse<string>.Fail(Messages.PasswordMismatch, 400);
                }

                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return GenericResponse<string>.Fail(Messages.EmailAlreadyExists, 400);
                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    UserType = registerDto.UserType,
                    PhoneNumber = registerDto.PhoneNumber,
                    EmailConfirmed = true // For demo purposes
                };

                // Set type-specific properties
                if (registerDto.UserType == "Dietitian")
                {
                    user.LicenseNumber = registerDto.LicenseNumber;
                    user.Specialization = registerDto.Specialization;
                    user.YearsOfExperience = registerDto.YearsOfExperience;
                    user.Bio = registerDto.Bio;
                }
                else if (registerDto.UserType == "Client")
                {
                    user.DateOfBirth = registerDto.DateOfBirth;
                    user.Gender = registerDto.Gender;
                    user.Height = registerDto.Height;
                    user.InitialWeight = registerDto.InitialWeight;
                    user.CurrentWeight = registerDto.CurrentWeight;
                    user.TargetWeight = registerDto.TargetWeight;
                    user.ActivityLevel = registerDto.ActivityLevel;
                    user.MedicalConditions = registerDto.MedicalConditions;
                    user.Allergies = registerDto.Allergies;
                    user.FoodPreferences = registerDto.FoodPreferences;
                    user.Notes = registerDto.Notes;
                    user.DietitianId = registerDto.DietitianId;
                }

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<string>.Fail($"{Messages.RegistrationFailed}: {errors}", 400);
                }

                // Add user to role
                await _userManager.AddToRoleAsync(user, registerDto.UserType);

                return GenericResponse<string>.Success(user.Id, Messages.RegistrationSuccessful);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"{Messages.RegistrationFailed}: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<Token>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Implementation for refresh token logic
                // This would typically involve validating the refresh token and generating a new access token
                await Task.CompletedTask;
                return GenericResponse<Token>.Fail("Refresh token implementation pending", 501);
            }
            catch (Exception ex)
            {
                return GenericResponse<Token>.Fail($"Refresh token failed: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> LogoutAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                await _signInManager.SignOutAsync();
                return GenericResponse<string>.Success("Logged out successfully", Messages.LogoutSuccessful);
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"Logout failed: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                {
                    return GenericResponse<string>.Fail(Messages.PasswordMismatch, 400);
                }

                var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);
                if (user == null)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<string>.Fail($"Password change failed: {errors}", 400);
                }

                return GenericResponse<string>.Success("Password changed successfully", "Password changed successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"Password change failed: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that the user doesn't exist
                    return GenericResponse<string>.Success("If the email exists, a reset link has been sent", "Reset email sent");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // Here you would typically send an email with the reset token
                // For demo purposes, we'll just return success

                return GenericResponse<string>.Success(token, "Password reset token generated");
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"Password reset failed: {ex.Message}", 500);
            }
        }

        public async Task<GenericResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                {
                    return GenericResponse<string>.Fail(Messages.PasswordMismatch, 400);
                }

                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return GenericResponse<string>.Fail(Messages.UserNotFound, 404);
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<string>.Fail($"Password reset failed: {errors}", 400);
                }

                return GenericResponse<string>.Success("Password reset successfully", "Password reset successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<string>.Fail($"Password reset failed: {ex.Message}", 500);
            }
        }

        private async Task<Token> GenerateTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("UserType", user.UserType),
                new("FullName", user.FullName)
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "DefaultSecretKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new Token
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationDate = token.ValidTo,
                RefreshToken = Guid.NewGuid().ToString() // Simplified refresh token
            };
        }
    }
}
