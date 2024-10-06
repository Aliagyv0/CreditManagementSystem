using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Loan:BaseEntity
    {
        public string Title { get; set; }
        public double MonthlyPrice { get; set; }
        public double TotalPrice { get; set; }
        public string Terms { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public bool IsConfirmed { get; set; }
        public LoanDetail LoanDetail { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<LoanItem> LoanItems { get; set; } = new List<LoanItem>();
    }
}
