using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class Payment:BaseEntity
    {
        public double Amount { get; set; }
        public string PaymentType { get; set; }
        public Guid LoanId { get; set; }
        public Loan Loan { get; set; }
    }
}
