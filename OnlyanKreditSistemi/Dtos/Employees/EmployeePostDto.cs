using OnlyanKreditSistemi.Models.Account;

namespace OnlyanKreditSistemi.Dtos.Employees
{
    public class EmployeePostDto : RegisterModel
    {
        public string Position { get; set; }
        public string Phone { get; set; }
        public string? BranchId { get; set; }
    }
}
