using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Context;
using OnlyanKreditSistemi.Dtos.Merchants;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class MerchantController : Controller
    {
        private readonly IRepository<Merchant> _repository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;

        public MerchantController(IRepository<Merchant> repository, UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _repository = repository;
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
         var merchants = await (await _repository.GetAllAsync(x=>!x.IsDeleted)).Include(x=>x.AppUser).Include(m=>m.Branches).ToListAsync();

            return View(merchants);
        }
        [HttpGet]
        public IActionResult Add()
        {

        return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(MerchantPostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                AppUser user = new AppUser()
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    EmailConfirmed = true,
                    IsActive = true,
                    UserName = dto.Username,
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(dto);
                }
                await _userManager.AddToRoleAsync(user, "Merchant");

                Merchant merchant = new Merchant()
                {
                    AppUser = user,
                    Description = dto.Description,
                    TerminalNo = dto.TerminalNo,
                };
                await _repository.AddAsync(merchant);
                await _repository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {

           var merchant = await (await _repository.GetAllAsync(m=>m.Id.ToString() == id)).Include(m=>m.AppUser).FirstOrDefaultAsync();
            if (merchant == null) {
                return NotFound();
                }

            MerchantUpdateDto dto = new()
            {
                Surname = merchant.AppUser.Surname,
                Name = merchant.AppUser.Name,
                Email = merchant.AppUser.Email,
                Username = merchant.AppUser.UserName,
                Description = merchant.Description,
                TerminalNo = merchant.TerminalNo
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id,  MerchantUpdateDto dto)
        {
            var merchant = await (await _repository.GetAllAsync(m => m.Id.ToString() == id)).Include(m => m.AppUser).FirstOrDefaultAsync();
            if (merchant == null)
            {
                return NotFound();
            }

            var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            connection.Open();
            var transaction = connection.BeginTransaction();

            AppUser? user = await _userManager.FindByEmailAsync(merchant.AppUser.Email);

            try
            {
                user.Email = dto.Email;
                user.Name = dto.Name;
                user.Surname = dto.Surname;
                user.UserName = dto.Username;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(dto);
                }

                if (!string.IsNullOrWhiteSpace(dto.Password) && !string.IsNullOrWhiteSpace(dto.CurrentPassword))
                {
                    result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.Password);


                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(dto);
                    }

                }

                await _signInManager.SignInAsync(user, true);

                merchant.Description = dto.Description;
                merchant.TerminalNo = dto.TerminalNo;
                _repository.Update(merchant);
                await _repository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var merchant = await _repository.GetAsync(x => x.Id.ToString() == id);
            if (merchant == null)
            {
                return NotFound();  
            }
            merchant.IsDeleted = true;
           _repository.Update(merchant);
            await _repository.SaveAsync();
            return RedirectToAction("index");
        }
    }
}
