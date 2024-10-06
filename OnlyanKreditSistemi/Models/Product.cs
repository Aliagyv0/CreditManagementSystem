using OnlyanKreditSistemi.Models.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyanKreditSistemi.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public double Price { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? BranchId { get; set; }
        [NotMapped]
        public IFormFile Image { get; set; }
        public Branch? Branch { get; set; }

        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
        
    }
}
