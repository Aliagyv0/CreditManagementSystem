using Microsoft.AspNetCore.Mvc;

namespace OnlyanKreditSistemi.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
