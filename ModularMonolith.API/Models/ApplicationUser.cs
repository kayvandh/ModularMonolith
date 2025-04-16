using Microsoft.AspNetCore.Identity;

namespace ModularMonolith.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}
