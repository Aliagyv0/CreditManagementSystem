using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;
using OnlyanKreditSistemi.ViewModels;

namespace OnlyanKreditSistemi.Controllers
{
    [Authorize(Roles ="Customer")]
    public class LoanController : Controller
    {
        private readonly IRepository<Loan> _repository;
        private readonly IRepository<LoanItem> _loanItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Customer> _customerRepository;

        public LoanController(IRepository<Loan> repository, IRepository<Customer> customerRepository, IRepository<Product> productRepository, IRepository<LoanItem> loanItemRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _loanItemRepository = loanItemRepository;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
 
            var loans = await (await _repository.GetAllAsync(x=>!x.IsDeleted)).Include(x=>x.Customer).ThenInclude(x=>x.AppUser).Include(x=>x.LoanDetail).Where(x=>x.Customer.AppUser.UserName== User.Identity.Name).Where(x=>x.IsConfirmed).ToListAsync();

            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string productId,int count)
        {
            var customer = await(await _customerRepository.GetAllAsync(x=>!x.IsDeleted)).Include(x=>x.AppUser).Where(x=>x.AppUser.UserName == User.Identity.Name).Include(x=>x.Loans).ThenInclude(x=>x.LoanDetail).FirstOrDefaultAsync();

            if (customer == null) 
                return BadRequest();

            var basket = customer.Loans.Where(x=>!x.IsConfirmed).FirstOrDefault();

            var product = await _productRepository.GetAsync(x => x.Id.ToString() == productId);

            if(product == null)
                return BadRequest();

            var price = count * product.Price;

            if (basket is not null)
            {
                //aktiv basketi olanda
                var loanitem = await _loanItemRepository.GetAsync(x => x.ProductId == product.Id && x.LoanId == basket.Id);
                if(loanitem is null)
                {

                    //elave edecik
                    LoanItem loanItem = new()
                    {
                        Count = count,
                        CreateDate = DateTime.Now,
                        ProductId = Guid.Parse(productId),
                        Price = price,
                        Loan = basket,
                    };
                    await _loanItemRepository.AddAsync(loanItem);

                    basket.LoanDetail.CurrentAmount += price;
                    
                    basket.TotalPrice += price;
                    _repository.Update(basket);
                    //problem ola biler
                    await _loanItemRepository.SaveAsync();
                }
                else
                {
                    //sayini artiraciq
                    loanitem.Count += count;
                    loanitem.Price += price;
                    basket.TotalPrice += price;
                    basket.LoanDetail.CurrentAmount +=price;
                    _loanItemRepository.Update(loanitem);
                    await _loanItemRepository.SaveAsync();
                    _repository.Update(basket);
                    await _loanItemRepository.SaveAsync();
                }

            }
            else
            {
                //basketi olmuyanda

                Loan loan = new Loan()
                {
                    CreateDate = DateTime.Now,
                    Customer = customer,
                    IsConfirmed = false,
                    Terms = "test",
                    Title = "Basket",
                    MonthlyPrice = 0,
                    LoanItems = new List<LoanItem>()
                    {
                        new LoanItem()
                        {
                            Count = count,
                            CreateDate = DateTime.Now,
                            ProductId = Guid.Parse(productId),
                            Price = price,
                        }
                    },
                    TotalPrice = price,
                    LoanDetail = new LoanDetail()
                    {
                        CurrentAmount = price
                    }
                };
               await _repository.AddAsync(loan);
                await _repository.SaveAsync();  

            }



            return RedirectToAction("basket", "shop");
        }

        [HttpGet] 
        public async Task<IActionResult> Payment(string loanId)
        {
            var loan = await(await _repository.GetAllAsync(x => x.Id.ToString() == loanId)).Include(x => x.LoanDetail).FirstOrDefaultAsync();

            if (loan == null || loan.LoanDetail.CurrentAmount <= 0)
                return NotFound();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string loanId,double amount)
        {
            if (amount <= 0) 
                return BadRequest();

           var loan = await (await _repository.GetAllAsync(x=>x.Id.ToString() == loanId)).Include(x=>x.LoanDetail).FirstOrDefaultAsync();

            if (loan == null || loan.LoanDetail.CurrentAmount<=0)
                return NotFound();

            if(amount > loan.LoanDetail.CurrentAmount)
            {
                TempData["message"] = $"Please pay equal or less than ${loan.LoanDetail.CurrentAmount}";
                return View();
            }
                

            Models.Payment payment = new Models.Payment()
            {
                Amount = amount,
                CreateDate = DateTime.Now,
                Loan = loan,
                PaymentType = "Credit Card",

            };

            loan.LoanDetail.CurrentAmount -= amount;
            loan.Payments.Add(payment);
            _repository.Update(loan);
            await _repository.SaveAsync();
            return RedirectToAction("index", "loan");

        }
    }
}
