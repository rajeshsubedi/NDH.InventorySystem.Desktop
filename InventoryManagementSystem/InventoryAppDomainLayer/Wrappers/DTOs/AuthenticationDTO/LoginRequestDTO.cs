using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO
{
    public class LoginRequestDTO
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}
