using OnlyanKreditSistemi.Models.Account;

namespace OnlyanKreditSistemi.Dtos.Customers
{
    
    public class CustomerUpdateDto :UpdateModel
    {
        public string Occupation { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}
