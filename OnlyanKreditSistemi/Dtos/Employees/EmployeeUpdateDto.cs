using OnlyanKreditSistemi.Models.Account;

namespace OnlyanKreditSistemi.Dtos.Employees
{
    public class EmployeeUpdateDto:UpdateModel
    {
        public string Position { get; set; }
        public string Phone { get; set; }
        public string? BranchId { get; set; }
    }
}
