using Microsoft.AspNetCore.Identity;

namespace OrderManagementApi.Models
{
    // Extends the base IdentityUser to allow for custom fields later if needed.
    public class ApplicationUser : IdentityUser
    {
        // No custom fields added as per assignment, but this structure is necessary for IdentityDbContext.
    }
}