using System.ComponentModel.DataAnnotations;

namespace OnlyanKreditSistemi.Models.Account
{
    public class ForgetPasswordModel
    {
        [EmailAddress]
        public string Mail { get; set; }
    }
}
