using OnlyanKreditSistemi.Models.Account;

namespace OnlyanKreditSistemi.Dtos.Merchants
{
    public class MerchantUpdateDto : UpdateModel
    {
        public string Description { get; set; }
        public int TerminalNo { get; set; }
    }
}
