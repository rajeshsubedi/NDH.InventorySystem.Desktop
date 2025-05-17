using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO
{
    public class UserLoginResponseDTO
    {
        public Guid UserId { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
