using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class LoanDetail : BaseEntity
    {
        public double CurrentAmount { get; set; }
        public int Mounths { get; set; }
        public Guid LoanId { get; set; }
        public Loan Loan { get; set; }
    }
}
