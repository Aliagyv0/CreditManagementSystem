using System.ComponentModel.DataAnnotations;

namespace OnlyanKreditSistemi.Models.Account
{
    public class ResetPasswordModel
    {
        public string Mail { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
