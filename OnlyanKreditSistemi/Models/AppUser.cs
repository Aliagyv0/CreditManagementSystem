using Microsoft.AspNetCore.Identity;

namespace OnlyanKreditSistemi.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public string? ConnectionId { get; set; }
        public Merchant? Merchant { get; set; }
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }


    }
}
