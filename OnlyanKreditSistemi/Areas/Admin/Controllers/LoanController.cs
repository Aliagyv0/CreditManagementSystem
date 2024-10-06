using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin,Merchant,Employee")]
    public class LoanController : Controller
    {
        private readonly IRepository<Loan> _loanRepository;

        public LoanController(IRepository<Loan> loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<IActionResult> Index(string customerId)
        {
            var loans =  await (await _loanRepository.GetAllAsync(x=>!x.IsDeleted)).Include(x=>x.LoanDetail).Include(x=>x.Customer).ThenInclude(x=>x.AppUser).Include(x=>x.Employee).ThenInclude(x=>x.AppUser).Where(x=>x.CustomerId.ToString()==customerId)
                .ToListAsync();

            return View(loans);
        }
    }
}
