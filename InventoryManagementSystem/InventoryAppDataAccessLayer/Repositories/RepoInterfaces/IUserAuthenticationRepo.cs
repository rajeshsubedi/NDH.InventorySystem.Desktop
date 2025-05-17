using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDomainLayer.DataModels.AuthenticationModels;

namespace InventoryAppDataAccessLayer.Repositories.RepoInterfaces
{
    public interface IUserAuthenticationRepo
    {
        Task RegisterUserForVerificationAsync(UserRegistrationDetails userRegistrationDetails);
        bool CheckIfUserExists(string userEmail);
        Task<UserRegistrationDetails> GetUserByEmaileAndConfirmFlagLogin(string userEmail);
        Task<UserRegistrationDetails> GetByRefreshTokenAsync(string refreshToken, Guid userId);
        Task<UserRegistrationDetails> GetDetailsByUserIdAsync(Guid userId);
        Task AddOrUpdateRefreshTokenAsync(UserRegistrationDetails userRegistrationDetails);
        Task<bool> InvalidateRefreshTokenAsync(string refreshToken, Guid userId);
        Task<UserRegistrationDetails> GetUserByEmailVerificationTokenAsync(string emailConfirmToken);
        Task<UserRegistrationDetails> GetUserByEmailOnlyAsync(string userEmail);
        Task<UserRegistrationDetails> GetDetailsByUserIdEmailAndFlagAsync(string email);
        Task UpdateOrRegisterUserAsync(UserRegistrationDetails userRegistrationDetails);
        Task<List<UserRegistrationDetails>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(Guid userId);
        Task SaveChangesAsync();
        Task UpdateUserAsync(UserRegistrationDetails user);

    }
}
