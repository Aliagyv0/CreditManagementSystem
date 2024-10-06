using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await (await _categoryRepository.GetAllAsync(s => !s.IsDeleted)).Include(x=>x.Parent).ToListAsync();
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = await(await _categoryRepository.GetAllAsync(s => !s.IsDeleted && s.Depth != 2)).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(s => !s.IsDeleted && s.Depth != 2)).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if(category.ParentId == null)
            {
                category.Depth = 0;
            }
            else if((await _categoryRepository.GetAsync(x=>x.Id == category.ParentId))?.ParentId == null)
            {
                category.Depth = 1;
            }
            else
            {
                category.Depth = 2;
            }

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {

            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(s => !s.IsDeleted && s.Depth != 2 && s.Id.ToString() != id)).ToListAsync();
            var entity = await _categoryRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            return View(entity);
        }

        public async Task<IActionResult> Update(string id, Category category)
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(s => !s.IsDeleted && s.Depth != 2 && s.Id.ToString() != id)).ToListAsync();
            var entity = await _categoryRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (category.ParentId == null)
            {
                category.Depth = 0;
            }
            else if ((await _categoryRepository.GetAsync(x => x.Id == category.ParentId))?.ParentId == null)
            {
                category.Depth = 1;
            }
            else
            {
                category.Depth = 2;
            }

            if (entity is null)
            {
                return NotFound();
            }

            entity.Name = category.Name;
            entity.Depth = category.Depth;
            entity.ParentId = category.ParentId;
            _categoryRepository.Update(entity);
            await _categoryRepository.SaveAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _categoryRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            entity.IsDeleted = true;
            _categoryRepository.Update(entity);
            await _categoryRepository.SaveAsync();
            return RedirectToAction("Index");

        }
    }
}
