using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SliderController : Controller
    {
        private readonly IRepository<Slider> _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(IRepository<Slider> repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var Sliders = await (await _repository.GetAllAsync(x => !x.IsDeleted)).ToListAsync();
            return View(Sliders);
        }
        [HttpGet]
        public  IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Slider Slider)
        {
            
            string fileName = Guid.NewGuid().ToString() + Slider.Image.FileName;
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/Sliders", fileName);
            using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
            {
                await Slider.Image.CopyToAsync(stream);
            }
            Slider.ImageUrl = fileName;
            Slider.IsActive = true;
            await _repository.AddAsync(Slider);
            await _repository.SaveAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]

        public async Task<IActionResult> Update(string id)
        {
            var Slider = await _repository.GetAsync(x => x.Id.ToString() == id);
            return View(Slider);
        }
        public async Task<IActionResult> Update(string id, Slider Slider)
        {
            var uptadedSlider = await _repository.GetAsync(x => x.Id.ToString() == id);
            if (uptadedSlider == null)
            {
                return NotFound();
            }
            if (Slider.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Slider.Image.FileName;

                string path = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/Sliders", fileName);
                using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
                {
                    await Slider.Image.CopyToAsync(stream);
                }
                uptadedSlider.ImageUrl = fileName;
            }
            uptadedSlider.Description = Slider.Description;
            uptadedSlider.Title = Slider.Title;
            _repository.Update(uptadedSlider);
            await _repository.SaveAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var Slider = await _repository.GetAsync(c => c.Id.ToString() == id);
            if (Slider == null)
            {
                return NotFound();
            }

            Slider.IsDeleted = true;
            await _repository.SaveAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DoActive(string id)
        {
            var slider = await _repository.GetAsync(c => c.Id.ToString() == id);
            if (slider == null)
                return NotFound();

            slider.IsActive = !slider.IsActive;

            _repository.Update(slider);
            await _repository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
