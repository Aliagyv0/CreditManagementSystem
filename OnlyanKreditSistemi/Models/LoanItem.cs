using OnlyanKreditSistemi.Models.BaseModels;

namespace OnlyanKreditSistemi.Models
{
    public class LoanItem:BaseEntity
    {
        public Guid LoanId { get; set; }
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public Loan Loan { get; set; }
        public Product Product { get; set; }
    }
}
