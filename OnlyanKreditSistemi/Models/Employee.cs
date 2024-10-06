using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Employee : BaseEntity
    {
        public string Position { get; set; }
        public string Phone { get; set; }
        public Guid? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
