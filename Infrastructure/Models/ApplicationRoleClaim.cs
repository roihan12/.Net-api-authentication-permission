using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public string Descriptions { get; set; }
        public string Group { get; set; }
    }
}
