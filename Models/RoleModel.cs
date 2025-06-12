

using Microsoft.AspNetCore.Identity;

namespace TraNgheCore.Models
{
    public class RoleModel : IdentityRole
    {
        //// 🆔 PRIMARY PROPERTIES
        //public string Id { get; set; }           // Unique identifier (GUID)
        //public string Name { get; set; }         // Role name (e.g., "Admin", "Manager")

        //// 🔧 INTERNAL PROPERTIES (used by Identity system)
        //public string NormalizedName { get; set; }  // Uppercase version for searching
        //public string ConcurrencyStamp { get; set; } // For optimistic concurrency

        //// 🔗 NAVIGATION PROPERTIES
        //public virtual ICollection<IdentityUserRole<string>> Users { get; set; }
        //public virtual ICollection<IdentityRoleClaim<string>> Claims { get; set; }
    }
}