using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.ViewModels
{
    public class GeneralVM
    {
        public ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
