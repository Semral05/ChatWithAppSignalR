using Microsoft.AspNetCore.Identity;

namespace Chat.Models
{
    public class AppUser:IdentityUser
    {
        public string? ConnectionId { get; set; }
    }
}
