using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Dtos.Customers;
using OnlyanKreditSistemi.Dtos.Employees;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        public CustomerController(IConfiguration configuration, UserManager<AppUser> userManager, IRepository<Customer> customerController)
        {
            _configuration = configuration;
            _userManager = userManager;
            _customerRepository = customerController;
        }

        [Authorize(Roles ="SuperAdmin,Admin,Merchant,Employee")]
        public async Task<IActionResult> Index()
        {
            var result = await (await _customerRepository.GetAllAsync(s => !s.IsDeleted)).Include(x => x.AppUser).ToListAsync();
            return View(result);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Add(CustomerPostDto dto)
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
                await _userManager.AddToRoleAsync(user, "Customer");

                Customer customer = new Customer()
                {
                    AppUser = user,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Occupation = dto.Occupation
                };
                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index");


        }


        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Update( string id)
        {
            var entity = await (await _customerRepository.GetAllAsync(x => x.Id.ToString() == id && !x.IsDeleted)).Include(x => x.AppUser).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }
          
            CustomerPostDto dto = new()
            {
                Surname = entity.AppUser.Surname,
                Name = entity.AppUser.Name,
                Email = entity.AppUser?.Email,
                Username = entity.AppUser?.UserName,
               Address = entity.Address,
               Occupation = entity.Occupation,
                Phone = entity.Phone,
            };

            return View(dto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> Update(string id, CustomerUpdateDto dto)
        {
            var customer = await (await _customerRepository.GetAllAsync(m => m.Id.ToString() == id)).Include(m => m.AppUser).FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound();
            }

            var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            connection.Open();
            var transaction = connection.BeginTransaction();

            AppUser? user = await _userManager.FindByEmailAsync(customer.AppUser.Email);

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
                customer.Address = dto.Address;
                customer.Phone = dto.Phone;
                customer.Occupation = dto.Occupation;
                _customerRepository.Update(customer);
                await _customerRepository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index");
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _customerRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            entity.IsDeleted = true;
            _customerRepository.Update(entity);
            await _customerRepository.SaveAsync();
            return RedirectToAction("Index");

        }
    }
}
