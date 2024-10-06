using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlyanKreditSistemi.Models;

namespace AvtosRestoran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,Merchant,Employee")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    
    }
}
