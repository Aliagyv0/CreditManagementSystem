using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public int Depth { get; set; }
        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; }=new List<Category>();
        public ICollection<Product> Product { get; set; }=new List<Product>();
    }
}
