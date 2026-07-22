using Microsoft.AspNetCore.Identity;

namespace Al_Muzayyen.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}