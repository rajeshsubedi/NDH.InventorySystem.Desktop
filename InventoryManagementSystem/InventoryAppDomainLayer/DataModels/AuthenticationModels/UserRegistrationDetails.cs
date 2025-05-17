using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.DataModels.AuthenticationModels
{
    public class UserRegistrationDetails
    {
        public virtual Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public int? ResetPasswordOTP { get; set; }
        public DateTime? OTPExpiration { get; set; }
        public string? Role { get; set; }
        public string Email { get; set; }
        public string? EmailConfirmToken { get; set; }
        public bool? EmailConfirmed { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool? PhoneNumberConfirmed { get; set; }
        public bool? TwoFactorEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
