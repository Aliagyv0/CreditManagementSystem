using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.ViewModels
{
    public class HomeVM
    {
        public ICollection<Slider> Sliders { get; set; }
        public ICollection<Product> Products { get; set; }

    }

}
