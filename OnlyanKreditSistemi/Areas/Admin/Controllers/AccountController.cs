using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlyanKreditSistemi.Models.Account;
using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            AppUser? user = await _userManager.FindByNameAsync(model.Username);

            if (user is null || !user.IsActive)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is locked. Please try again later");
                    return View(model);
                }
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(model);
            }
            if (!await _userManager.IsInRoleAsync(user, "Admin") && !await _userManager.IsInRoleAsync(user, "SuperAdmin") && !await _userManager.IsInRoleAsync(user,"Merchant") && !await _userManager.IsInRoleAsync(user,"Employee"))
            {
                BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser user = new AppUser()
            {
                Name = "SuperAdmin",
                Surname = "SuperAdmin",
                Email = "Superadmin@avtos.com",
                UserName = "SuperAdmin",
                EmailConfirmed = true,
                IsActive = true
            };
            await _userManager.CreateAsync(user, "SuperAdmin123.");
            await _userManager.AddToRolesAsync(user, new string[] { "SuperAdmin", "Admin" });
            return Ok();

        }


        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole role1 = new IdentityRole()
        //    {
        //        Name = "User"
        //    };
        //    IdentityRole role2 = new IdentityRole()
        //    {
        //        Name = "Admin"
        //    };
        //    IdentityRole role3 = new IdentityRole()
        //    {
        //        Name = "SuperAdmin"
        //    };

        //    await _roleManager.CreateAsync(role1);
        //    await _roleManager.CreateAsync(role2);
        //    await _roleManager.CreateAsync(role3);
        //    return Ok();
        //}
    }
}
