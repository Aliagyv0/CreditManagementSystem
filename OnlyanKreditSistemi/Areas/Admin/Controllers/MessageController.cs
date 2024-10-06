using OnlyanKreditSistemi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessageController : Controller
    {

        private readonly IRepository<Contact> _ContactRepo;

        public MessageController(IRepository<Contact> contactRepo)
        {
            _ContactRepo = contactRepo;
        }

        [Authorize(Roles = "SuperAdmin,Admin")] 
        public async Task<IActionResult> Index()
        {
            var result = await (await _ContactRepo.GetAllAsync(s => !s.IsDeleted)).ToListAsync();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] string id)
        {
            var message = await _ContactRepo.GetAsync(x => x.Id.ToString() == id);
            if (message == null)
            {
                return Json(new { statucCode = 404, message = "Not found." });
            }

            return Json(new { statucCode = 200, data = message });
        }

    }
}

