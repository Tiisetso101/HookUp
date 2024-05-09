using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public User User { get; set; }

        public AppRole AppRole { get; set; }
    }
}