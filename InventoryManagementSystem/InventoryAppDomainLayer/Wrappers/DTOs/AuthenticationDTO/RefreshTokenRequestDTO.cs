using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO
{
    public class RefreshTokenRequestDTO
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
