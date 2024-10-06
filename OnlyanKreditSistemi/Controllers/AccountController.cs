using OnlyanKreditSistemi.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Services.Interfaces;
using OnlyanKreditSistemi.Dtos.Customers;
using Microsoft.Data.SqlClient;
using OnlyanKreditSistemi.Repository.Interfaces;
namespace OnlyanKreditSistemi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Customer> _customerRepository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMailService mailService, IHttpContextAccessor contextAccessor, IConfiguration configuration, IRepository<Customer> customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _customerRepository = customerRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerPostDto dto)
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
                AppUser user = new()
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    UserName = dto.Username,
                    IsActive = true
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
                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action("VerifyEmail", controller: "Account", values: new
                {
                    token,
                    mail = user.Email,

                }, protocol: Request.Scheme);

                await _mailService.SendMail("aliaea@code.edu.az", user.Email, "Verification Email", link);



                Customer customer = new()
                {
                    AppUser = user,
                    Phone = dto.Phone,
                    Address = dto.Address,
                    Occupation = dto.Occupation,
                };
                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveAsync();

                transaction.Commit();

            }
            catch (Exception)
            {
                transaction.Rollback();

            }
            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            AppUser? user = await _userManager.FindByNameAsync(model.Username);


            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(model.Username);
                if (user is null)
                {
                    ModelState.AddModelError(" ", "Username or password is incorrect");
                    return View(model);
                }
            }

            if (!await _userManager.IsInRoleAsync(user, "Customer"))
            {
                ModelState.AddModelError(" ", "You cannot login in this page");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);


            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(" ", "Your account is locked. Please try again later");
                    return View(model);
                }
                ModelState.AddModelError(" ", "Username or password is incorrect");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> VerifyEmail(string token, string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user is null)
            {
                return NotFound();
            }
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("index", "home");
        }
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
            if (user is null)
            {
               return NotFound();
            }
            UpdateModel model = new()
            {
                Username = user.UserName,
                Surname = user.Surname,
                Name = user.Name,
                Email = user.Email
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Update(UpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
            if (user is null)
            {
                return NotFound();
            }
            user.Email = model.Email;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.UserName = model.Username;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);


                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

            }

            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index", "home");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Mail))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(model.Mail);
            if (user is null)
            {
                return NotFound();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var link = Url.Action(action: "ResetPassword", controller: "Account", values: new
            {
                token,
                mail = model.Mail
            }, protocol: Request.Scheme);

            await _mailService.SendMail("aliaea@code.edu.az", model.Mail, "Reset Password", link);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]

        public async Task<IActionResult> ResetPassword(string token, string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user is null)
            {
                return NotFound();
            }

            ResetPasswordModel model = new ResetPasswordModel()
            {
                Mail = mail,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Mail);
            if (user is null)
            {
                return NotFound();
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }
            return RedirectToAction("Login", "account");
        }
    }
}
