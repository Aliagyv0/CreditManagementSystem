using Microsoft.AspNetCore.Mvc;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Controllers
{
    public class ContactController : Controller
    {
        private readonly IRepository<Contact> _contactRepository;

        public ContactController(IRepository<Contact> contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    statusCode = 400,
                    message = "Melumatlarinizi bir daha yoxlayib yeniden gonderin"
                });
            }
            await _contactRepository.AddAsync(contact);
            await _contactRepository.SaveAsync();
            return Json(new
            {
                statusCode = 200,
                message = "Sizin message gonderildi"
            });
        }
    }
}
