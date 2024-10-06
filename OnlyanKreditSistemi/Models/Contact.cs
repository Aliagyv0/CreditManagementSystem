using OnlyanKreditSistemi.Models.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace OnlyanKreditSistemi.Models
{
    public class Contact:BaseEntity
    {
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
