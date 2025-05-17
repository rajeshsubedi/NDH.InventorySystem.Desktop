using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppDomainLayer.DataModels.AuthenticationModels;
using InventoryAppDomainLayer.Exceptions;
using InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO;
using InventoryAppServicesLayer.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace InventoryAppServicesLayer.ServiceImplementations
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserAuthenticationRepo _userRepository;
        private static Random random = new Random();
        private readonly IEmailService _emailService;
        public UserAuthenticationService(IConfiguration configuration, IUserAuthenticationRepo userRepository, IEmailService emailService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
        }


        //Login Section
        public async Task<UserLoginResponseDTO> AuthenticateLogin(LoginRequestDTO loginModelDTO)
        {
            try
            {
                // Retrieve all users by username (if possible)
                var PasswordHashhh = BCrypt.Net.BCrypt.HashPassword(loginModelDTO.Password);
                var user = await _userRepository.GetUserByEmaileAndConfirmFlagLogin(loginModelDTO.UserEmail);
                if (user == null)
                {
                    throw new UserUnauthenticatedException("Wrong Email or Password");
                }
                if (BCrypt.Net.BCrypt.Verify(loginModelDTO.Password, user.PasswordHash))
                {
                    var refreshTokenValue = GenerateRefreshToken();
                    var refreshTokenExpirationValue = GetTokenExpiration();
                    user.RefreshToken = refreshTokenValue;
                    user.TokenExpiration = refreshTokenExpirationValue;
                    await _userRepository.AddOrUpdateRefreshTokenAsync(user);
                    var userResponseDTO = new UserLoginResponseDTO
                    {
                        UserId = user.UserId,
                        JwtToken = GenerateJSONWebToken(user),
                        RefreshToken = refreshTokenValue,
                        RefreshTokenExpiration = refreshTokenExpirationValue
                    };

                    return userResponseDTO;
                }
                // If no user with the correct password is found
                Log.Error("User not found or incorrect password for username: {Username}", loginModelDTO.UserEmail);
                throw new UserUnauthenticatedException("Wrong UserId or Password");
            }
            catch (UserUnauthenticatedException ex)
            {
                Log.Error("{ex}", ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error during authentication for username: {Username}", loginModelDTO.UserEmail);
                throw; // Ensure exceptions are not being swallowed or rethrown improperly
            }
        }

        public DateTime GetTokenExpiration()
        {
            int refreshTokenExpirationDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpirationDays");
            return DateTime.UtcNow.AddDays(refreshTokenExpirationDays); // Corrected
        }
        //Register Email Section
        public async Task<bool> StoreUnregisterdUserForVerificationAsync(UserRegistrationRequestDTO userRegistrationDetailsDTO)
        {
            try
            {
                if (_userRepository.CheckIfUserExists(userRegistrationDetailsDTO.Email))
                {
                    throw new DuplicateRecordException("User Already Exists with this Email.");
                }

                var user = new UserRegistrationDetails
                {
                    UserId = Guid.NewGuid(),
                    UserName = userRegistrationDetailsDTO.Username,
                    CreatedBy = "RMSRegApi",
                    LastModified = DateTime.Now,
                    LastModifiedBy = userRegistrationDetailsDTO.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDetailsDTO.Password),
                    Role = "user",
                    Email = userRegistrationDetailsDTO.Email,
                    EmailConfirmToken = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    LastLogin = DateTime.Now,
                    EmailConfirmed = false,
                    SecurityStamp = "asdaidsnads",
                    PhoneNumber = userRegistrationDetailsDTO.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = true,
                    AccessFailedCount = 0,
                };
                await _userRepository.RegisterUserForVerificationAsync(user);
                return true;
            }
            catch (DuplicateRecordException ex)
            {
                Log.Error("{ex}", ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("{ex}", ex);
                throw new Exception("An error occurred while registering the user.", ex);
            }
        }

        public async Task StoreEmailVerificationToken(string email, string token)
        {
            var user = await _userRepository.GetUserByEmailOnlyAsync(email);
            if (user != null)
            {
                user.EmailConfirmToken = token;
                user.EmailConfirmed = false;
                await _userRepository.UpdateOrRegisterUserAsync(user);
            }
        }

        public async Task<bool> VerifyEmailAsync(string emailToken)
        {
            var user = await _userRepository.GetUserByEmailVerificationTokenAsync(emailToken);
            if (user != null && user.EmailConfirmed != true)
            {
                user.EmailConfirmed = true;
                user.EmailConfirmToken = null;
                await _userRepository.UpdateOrRegisterUserAsync(user);
                return true;
            }
            else
            {
                throw new NotFoundException("User already verified or Verification expired");
            }
        }


        //Signout Section
        public async Task<bool> SignOutAsync(RefreshTokenRequestDTO refreshTokenRequestDTO)
        {
            try
            {
                // Attempt to invalidate the refresh token
                bool success = await _userRepository.InvalidateRefreshTokenAsync(refreshTokenRequestDTO.RefreshToken, refreshTokenRequestDTO.UserId);

                // Return true if the token was successfully invalidated
                return success;
            }
            catch (Exception ex)
            {
                // Handle or log exception as needed
                throw new CustomInvalidOperationException("An error occurred while invalidating the refresh token");
            }
        }


        //Forget Password Section
        public async Task<bool> SendPasswordResetOtpAsync(string email)
        {
            var user = await _userRepository.GetUserByEmaileAndConfirmFlagLogin(email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var otp = GenerateOTP();
            user.ResetPasswordOTP = otp;
            user.OTPExpiration = DateTime.UtcNow.AddMinutes(10);

            await _userRepository.UpdateOrRegisterUserAsync(user);

            // Attempt to send the email with retries
            var maxRetries = 3;
            var currentRetry = 0;
            bool emailSent = false;

            while (currentRetry < maxRetries && !emailSent)
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Your OTP for Password Reset", $"<p>Your OTP for password reset is: <strong>{otp}</strong></p>");
                    emailSent = true; // Email sent successfully
                }
                catch (Exception ex)
                {
                    // Log the error and retry
                    Log.Error(ex, "Failed to send password reset email. Retrying...");
                    currentRetry++;
                    if (currentRetry >= maxRetries)
                    {
                        // Log final failure and handle accordingly
                        Log.Error("Failed to send password reset email after multiple attempts.");
                        throw;
                    }
                    await Task.Delay(2000); // Wait before retrying
                }
            }

            return emailSent;
        }
        public async Task<bool> ConfirmPasswordResetOtpAsync(string email, int otp)
        {
            var user = await _userRepository.GetUserByEmaileAndConfirmFlagLogin(email);
            if (user == null) throw new Exception("User not found");

            if (user.ResetPasswordOTP == otp && user.OTPExpiration >= DateTime.UtcNow)
            {
                user.ResetPasswordOTP = null;
                user.OTPExpiration = null;
                await _userRepository.UpdateOrRegisterUserAsync(user);
                return true;
            }
            return false;
        }
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userRepository.GetDetailsByUserIdEmailAndFlagAsync(email);

            if (user == null)
            {
                return false;
            }

            // Update the password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            //user.ResetPasswordOTP = null; // Ensure OTP is null
            //user.OTPExpiration = null; // Ensure OTP expiration is null

            await _userRepository.UpdateOrRegisterUserAsync(user);

            return true;
        }



        //GetUser services
        public async Task<UserRegistrationDetails> GetUserByEmaileAndConfirmFlagLogin(string email)
        {
            var user = await _userRepository.GetUserByEmaileAndConfirmFlagLogin(email);
            return user;
        }
        public async Task<UserRegistrationDetails> GetUserByEmailOnlyAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailOnlyAsync(email);
            return user;
        }



        //Extra Services
        private string GenerateJSONWebToken(UserRegistrationDetails user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("user_id", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),

                 }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GeneratJWTToken(UserRegistrationDetails user)
        {
            return GenerateJSONWebToken(user);
        }
        public async Task<UserLoginResponseDTO> RefreshJwtUserToken(RefreshTokenRequestDTO refreshTokenRequest)
        {
            // Validate the existing refresh token
            var user = await _userRepository.GetByRefreshTokenAsync(refreshTokenRequest.RefreshToken, refreshTokenRequest.UserId);
            if (user == null || user.TokenExpiration < DateTime.UtcNow)
            {
                throw new CustomInvalidOperationException("Invalid or expired refresh token");
            }
            // Generate a new JWT token
            var jwtToken = GenerateJSONWebToken(user);

            // Return the response with the new JWT token
            return new UserLoginResponseDTO
            {
                UserId = user.UserId,
                JwtToken = jwtToken,
                RefreshToken = refreshTokenRequest.RefreshToken, // Optional: you might choose to return the same or a new refresh token
                RefreshTokenExpiration = refreshTokenRequest.TokenExpiration // Optional: you might want to return the expiration date of the refresh token
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public int GenerateOTP()
        {
            return random.Next(100000, 999999); // Generates a 6-digit OTP
        }


        //UserDetails

        public async Task<List<UserDetailsResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();

            return users.Select(user => new UserDetailsResponseDTO
            {
                userId = user.UserId,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }).ToList();
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }
        public async Task UpdateUserInfoAsync(Guid userId, UpdateUserInfoRequestDTO updateDto)
        {
            var user = await _userRepository.GetDetailsByUserIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found.");

            user.UserName = updateDto.Username;
            user.PhoneNumber = updateDto.PhoneNumber;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

    }
}
