using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Customer:BaseEntity
    {
        public string Occupation { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<Loan> Loans { get;set; } = new List<Loan>();
    }
}
    