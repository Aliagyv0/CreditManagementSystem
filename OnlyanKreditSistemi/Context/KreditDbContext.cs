using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.Context
{
    public class KreditDbContext : IdentityDbContext<AppUser>
    {
        public KreditDbContext(DbContextOptions<KreditDbContext> options) : base(options)
        {
        }
        public KreditDbContext()
        {
            
        }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer>  Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanItem> LoanItems { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<LoanDetail> LoanDetails { get; set; }

    }
}
