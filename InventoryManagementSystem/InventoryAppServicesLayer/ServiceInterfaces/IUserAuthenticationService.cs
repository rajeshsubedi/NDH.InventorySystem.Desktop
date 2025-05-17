using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.AuthenticationModels;
using InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO;

namespace InventoryAppServicesLayer.ServiceInterfaces
{
    public interface IUserAuthenticationService
    {
        Task<UserLoginResponseDTO> AuthenticateLogin(LoginRequestDTO loginModelDTO);
        Task<bool> StoreUnregisterdUserForVerificationAsync(UserRegistrationRequestDTO userRegistrationDetailsDTO);
        Task<bool> VerifyEmailAsync(string emailToken);
        Task<UserLoginResponseDTO> RefreshJwtUserToken(RefreshTokenRequestDTO refreshTokenRequestDTO);
        Task<bool> SignOutAsync(RefreshTokenRequestDTO refreshTokenRequestDTO);
        Task StoreEmailVerificationToken(string email, string emailToken);
        Task<bool> SendPasswordResetOtpAsync(string userEmail);
        Task<bool> ConfirmPasswordResetOtpAsync(string email, int otp);
        Task<UserRegistrationDetails> GetUserByEmaileAndConfirmFlagLogin(string email);
        Task<UserRegistrationDetails> GetUserByEmailOnlyAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        string GeneratJWTToken(UserRegistrationDetails user);
        Task<List<UserDetailsResponseDTO>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(Guid userId);
        Task UpdateUserInfoAsync(Guid userId, UpdateUserInfoRequestDTO updateDto);
    }
}
