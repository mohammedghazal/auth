using Microsoft.AspNetCore.Identity;

namespace auth.API.Models
{
    public class UserRole : IdentityUserRole<int>
    {
         public User User { get; set; } 

         public Role Role { get; set; }
    }
}