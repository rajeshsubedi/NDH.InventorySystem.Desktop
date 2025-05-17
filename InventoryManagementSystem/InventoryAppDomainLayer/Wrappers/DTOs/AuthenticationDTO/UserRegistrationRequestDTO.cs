using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.DTOs.AuthenticationDTO
{
    public class UserRegistrationRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }

    public class UserDetailsResponseDTO
    {
        public Guid? userId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
    public class UpdateUserInfoRequestDTO
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
    }
}
