using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.ViewModels
{
    public class ShopVM
    {
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
