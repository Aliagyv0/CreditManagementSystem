using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlyanKreditSistemi.Dtos.Employees;
using OnlyanKreditSistemi.Dtos.Merchants;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,Merchant")]
    public class EmployeeController : Controller
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager; 
        private readonly IRepository<Branch> _branchRepository;
        public EmployeeController(IRepository<Employee> employeeRepository, IConfiguration configuration, UserManager<AppUser> userManager, IRepository<Branch> branchRepository)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
            _userManager = userManager;
            _branchRepository = branchRepository;
        }

        public async Task<IActionResult> Index(string branchId)
        {
            var result = await (await _employeeRepository.GetAllAsync(s => !s.IsDeleted && s.BranchId.ToString() == branchId)).Include(x=>x.AppUser).ToListAsync();
            return View(result);
        }
        [HttpGet]
        public IActionResult Add(string branchId)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromQuery]string branchId,[FromForm]EmployeePostDto dto)
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
                await _userManager.AddToRoleAsync(user, "Employee");

                Employee employee = new Employee()
                {
                    AppUser = user,
                    Position = dto.Position,
                    Phone = dto.Phone,
                    BranchId = Guid.Parse(branchId)
                };
                await _employeeRepository.AddAsync(employee);
                await _employeeRepository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index", new { branchId = branchId });

        }
        [HttpGet]
        public async Task<IActionResult> Update(string branchId ,string id)
        {      
            var entity = await ( await _employeeRepository.GetAllAsync(x => x.Id.ToString() == id && !x.IsDeleted)).Include(x=>x.Branch).ThenInclude(x=>x.Merchant).Include(x=>x.AppUser).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }
            ViewBag.Branches = await (await _branchRepository.GetAllAsync(x => x.MerchantId == entity.Branch.MerchantId)).ToListAsync();


            EmployeeUpdateDto dto = new()
            {
                Surname = entity.AppUser.Surname,
                Name = entity.AppUser.Name,
                Email = entity.AppUser?.Email,
                Username = entity.AppUser?.UserName,
                BranchId = entity.BranchId.ToString(),
                Position = entity.Position,
                Phone = entity.Phone,
            };

            return View(dto);
        }

        public async Task<IActionResult> Update(string branchId,string id, EmployeeUpdateDto dto)
        {
            var employee = await (await _employeeRepository.GetAllAsync(m => m.Id.ToString() == id)).Include(m => m.AppUser).FirstOrDefaultAsync();
            if (employee == null)
            {
                return NotFound();
            }

            var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            connection.Open();
            var transaction = connection.BeginTransaction();

            AppUser? user = await _userManager.FindByEmailAsync(employee.AppUser.Email);

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
                employee.Position = dto.Position;
                employee.Phone = dto.Phone;
                employee.BranchId =Guid.Parse(dto.BranchId);
                _employeeRepository.Update(employee);
                await _employeeRepository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index", new { branchId = branchId });
        }
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _employeeRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            entity.IsDeleted = true;
            _employeeRepository.Update(entity);
            await _employeeRepository.SaveAsync();
            return RedirectToAction("Index");

        }
    }
}
