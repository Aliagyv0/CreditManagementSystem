using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvtosRestoran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]

    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var admins = users.Where(user => _userManager.IsInRoleAsync(user, "Admin").Result)
                .ToList();

            return View(admins);
        }
        [HttpGet]

        public IActionResult Add()
        {
        return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Add(RegisterModel model)
        {
            AppUser user = new()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed=true,
                IsActive=true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            await _userManager.AddToRoleAsync(user, "Admin");
            
           

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
          var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DoActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
           await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
    }
}
