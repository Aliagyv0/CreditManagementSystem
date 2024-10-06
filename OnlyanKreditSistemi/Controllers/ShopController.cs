using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;
using OnlyanKreditSistemi.ViewModels;

namespace OnlyanKreditSistemi.Controllers
{
    public class ShopController : Controller
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<LoanItem> _loanItemRepository;
        private readonly IRepository<Loan> _loanRepository;
        private readonly IRepository<LoanDetail> _loanDetailRepository;

        public ShopController(IRepository<Product> repository, IRepository<Customer> customerRepository, IRepository<LoanItem> loanItemRepository, IRepository<LoanDetail> loanDetailRepository, IRepository<Loan> loanRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _loanItemRepository = loanItemRepository;
            _loanDetailRepository = loanDetailRepository;
            _loanRepository = loanRepository;
        }

        public async Task<IActionResult> Index(string? categoryId)
        {
            var products = await _repository.GetAllAsync(x => !x.IsDeleted);

            if(categoryId != null)
            {
                products = products.Include(x => x.Category).ThenInclude(c => c.Parent).ThenInclude(p => p.Parent).Where(x => x.CategoryId.ToString() == categoryId || x.Category.ParentId.ToString() == categoryId
                || x.Category.Parent.ParentId.ToString() == categoryId);
            }

            ShopVM model = new()
            {
                Products = await products.ToListAsync(),
            };
            return View(model);
        }
        public async Task<IActionResult> Detail(string id)
        {
            Product? product = await(await _repository.GetAllAsync(x=>x.Id.ToString() == id && !x.IsDeleted)).Include(x=>x.Category).ThenInclude(c=>c.Parent).ThenInclude(p=>p.Parent).FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> Basket()
        {
            var customer = await(await _customerRepository.GetAllAsync(x => !x.IsDeleted)).Include(x => x.AppUser).Where(x => x.AppUser.UserName == User.Identity.Name).Include(x => x.Loans).ThenInclude(x => x.LoanItems).ThenInclude(x=>x.Product).ThenInclude(x=>x.Category).FirstOrDefaultAsync();

            if (customer == null)
                return BadRequest();

            var basket = customer.Loans.Where(x => !x.IsConfirmed).FirstOrDefault();
            if (basket == null)
                return RedirectToAction("index", "home");

            return View(basket);
        }

        [HttpPost]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> Basket(int month)
        {
            var customer = await (await _customerRepository.GetAllAsync(x => !x.IsDeleted)).Include(x => x.AppUser).Where(x => x.AppUser.UserName == User.Identity.Name).Include(x => x.Loans).ThenInclude(x=>x.LoanDetail).Include(x=>x.Loans).ThenInclude(x=>x.LoanItems).ThenInclude(x=>x.Product).ThenInclude(x=>x.Category).FirstOrDefaultAsync();

            if (customer == null) return BadRequest();


            var basket = customer.Loans.Where(x => !x.IsConfirmed).FirstOrDefault();
            if (basket == null) return BadRequest();

            if (!(month == 3 || month == 6 || month == 12 || month == 18))
            {
                ModelState.AddModelError(" ", "Please select corrrect month");
                return View(basket);
            }

            basket.IsConfirmed = true;
            basket.LoanDetail.Mounths = month;
            basket.MonthlyPrice = (double)basket.TotalPrice / month;
            _loanRepository.Update(basket);
            _loanDetailRepository.Update(basket.LoanDetail);
            await  _loanRepository.SaveAsync(); 
            return RedirectToAction("index","home");

        }

        [Authorize(Roles ="Basket")]
        public async Task<IActionResult> DeleteBasketItem(string id)
        {
            var basketItem = await ( await _loanItemRepository.GetAllAsync(x => x.Id.ToString() == id))
                .Include(x=>x.Loan).ThenInclude(x=>x.LoanDetail).FirstOrDefaultAsync();
            if(basketItem == null) { return NotFound(); }


            basketItem.Loan.TotalPrice -= basketItem.Price;
            basketItem.Loan.LoanDetail.CurrentAmount -= basketItem.Price;

            _loanRepository.Update(basketItem.Loan);
            _loanDetailRepository.Update(basketItem.Loan.LoanDetail);
            _loanItemRepository.Delete(basketItem);
            await _loanItemRepository.SaveAsync();
            return RedirectToAction("basket");
        }
    }
}
