using OnlyanKreditSistemi.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace OnlyanKreditSistemi.Dtos.Merchants
{
    public class MerchantPostDto : RegisterModel
    {
        public string Description { get; set; }
        public int TerminalNo { get; set; }
    }
}
