using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO
{
    public class ForgotPasswordRequestDTO
    {
        public string UserEmail { get; set; }
    }
    public class ConfirmOtpRequest
    {
        public string Email { get; set; }
        public int Otp { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
