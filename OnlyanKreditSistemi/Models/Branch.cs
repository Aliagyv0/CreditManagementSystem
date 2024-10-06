using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
