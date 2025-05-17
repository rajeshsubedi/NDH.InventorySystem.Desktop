using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryAppDataAccessLayer.Data;
using InventoryAppDataAccessLayer.Repositories.RepoInterfaces;
using InventoryAppDomainLayer.DataModels.AuthenticationModels;
using InventoryAppDomainLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InventoryAppDataAccessLayer.Repositories.RepoImplementations
{
    public class UserAuthenticationRepo : IUserAuthenticationRepo
    {
        private readonly InventoryServiceDbContext _rmsServicedb;

        public UserAuthenticationRepo(InventoryServiceDbContext rMSServiceDbContext)
        {
            _rmsServicedb = rMSServiceDbContext;
        }

        //Login Section
        public async Task<UserRegistrationDetails> GetUserByEmaileAndConfirmFlagLogin(string userEmail)
        {
            try
            {
                var user = await _rmsServicedb.UserRegistration
                                .FirstOrDefaultAsync(u => u.Email == userEmail && u.EmailConfirmed == true);

                if (user == null)
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching user details by login.");
                return null;
            }
        }


        //Add Section
        public async Task RegisterUserForVerificationAsync(UserRegistrationDetails userRegistrationDetails)
        {
            _rmsServicedb.UserRegistration.AddAsync(userRegistrationDetails);
            await _rmsServicedb.SaveChangesAsync();
        }


        //Check Section
        public bool CheckIfUserExists(string email)
        {
            try
            {
                return _rmsServicedb.UserRegistration.Any(u =>
                    (email != null && u.Email == email)
                );
            }
            catch (Exception ex)
            {
                // Log exception
                Log.Error("{ex}", ex);
                throw new UserUnauthenticatedException(ex.Message);
            }
        }


        //Get User Section 
        public async Task<UserRegistrationDetails> GetUserByEmailOnlyAsync(string email)
        {
            return await _rmsServicedb.UserRegistration.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserRegistrationDetails?> GetUserByEmailVerificationTokenAsync(string emailTokenString)
        {
            return await _rmsServicedb.UserRegistration
                .FirstOrDefaultAsync(u => u.EmailConfirmToken == emailTokenString);
        }

        public async Task<UserRegistrationDetails> GetDetailsByUserIdAsync(Guid userId)
        {
            return await _rmsServicedb.UserRegistration.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<UserRegistrationDetails?> GetDetailsByUserIdEmailAndFlagAsync(string email)
        {
            return await _rmsServicedb.UserRegistration.FirstOrDefaultAsync(u => u.Email == email && u.EmailConfirmed == true);
        }

        public async Task<UserRegistrationDetails> GetByRefreshTokenAsync(string refreshToken, Guid userId)
        {
            return await _rmsServicedb.UserRegistration
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.UserId == userId);
        }



        //Update Section
        public async Task UpdateOrRegisterUserAsync(UserRegistrationDetails userRegistrationDetails)
        {
            _rmsServicedb.UserRegistration.Update(userRegistrationDetails);
            await _rmsServicedb.SaveChangesAsync();
        }

        public async Task AddOrUpdateRefreshTokenAsync(UserRegistrationDetails userRegistrationDetails)
        {
            var existingUser = await GetDetailsByUserIdAsync(userRegistrationDetails.UserId);
            if (existingUser != null)
            {
                existingUser.RefreshToken = userRegistrationDetails.RefreshToken;
                existingUser.TokenExpiration = userRegistrationDetails.TokenExpiration;
                _rmsServicedb.UserRegistration.Update(existingUser);
                await _rmsServicedb.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("User not exists");
            }
        }

        public async Task<bool> InvalidateRefreshTokenAsync(string refreshToken, Guid userId)
        {
            try
            {
                // Find the user with the given refresh token and user ID
                var user = await _rmsServicedb.UserRegistration
                    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.UserId == userId);

                if (user != null)
                {
                    // Clear the refresh token and token expiration
                    user.RefreshToken = null; // or an empty string if not nullable
                    user.TokenExpiration = null; // ensure this is nullable
                    // Save the changes to the database
                    await _rmsServicedb.SaveChangesAsync();
                    return true;
                }

                return false; // No matching user found
            }
            catch (Exception ex)
            {
                // Handle or log exception as needed
                throw new CustomInvalidOperationException("An error occurred while invalidating the refresh token");
            }
        }


        //Userdatails

        public async Task<List<UserRegistrationDetails>> GetAllUsersAsync()
        {
            try
            {
                return await _rmsServicedb.UserRegistration.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching all users.");
                return new List<UserRegistrationDetails>();
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _rmsServicedb.UserRegistration.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return false;
                }

                _rmsServicedb.UserRegistration.Remove(user);
                await _rmsServicedb.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the user.");
                return false;
            }
        }
        public async Task UpdateUserAsync(UserRegistrationDetails user)
        {
            _rmsServicedb.UserRegistration.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await _rmsServicedb.SaveChangesAsync();
        }


    }
}
