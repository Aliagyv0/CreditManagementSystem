using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Merchant :BaseEntity
    {
        public string Description { get; set; }
        public int TerminalNo { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<Branch> Branches { get; set; }=new List<Branch>();
    }
}
