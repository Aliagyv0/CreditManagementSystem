using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin,Merchant,Employee")]
    public class PaymentController : Controller
    {
        private readonly IRepository<Payment> _PaymentRepository;

        public PaymentController(IRepository<Payment> PaymentRepository)
        {
            _PaymentRepository = PaymentRepository;
        }

        public async Task<IActionResult> Index(string loanId)
        {
            var payments =  await (await _PaymentRepository.GetAllAsync(x=>!x.IsDeleted && loanId == x.LoanId.ToString()))
                .OrderByDescending(x=>x.CreateDate)
                .ToListAsync();

            return View(payments);
        }
    }
}
