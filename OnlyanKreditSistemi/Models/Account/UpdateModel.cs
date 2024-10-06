using System.ComponentModel.DataAnnotations;

namespace OnlyanKreditSistemi.Models.Account
{
    public class UpdateModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
               ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
