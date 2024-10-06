using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace OnlyanKreditSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin,Merchant,Employee")]
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        public ProductController(IRepository<Product> productRepository, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager, IRepository<Branch> branchRepository, IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _branchRepository = branchRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<Product>? result = null;
            if(User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                result = await (await _productRepository.GetAllAsync(s => !s.IsDeleted)).Include(x => x.Category).ToListAsync();
            }
            else if(User.IsInRole("Merchant"))
            {
                var user = _userManager.Users.Where(x=>x.UserName == User.Identity.Name).Include(x=>x.Merchant).ThenInclude(x=>x.Branches).ThenInclude(x=>x.Products).ThenInclude(x=>x.Category).FirstOrDefault();
                result = user?.Merchant?.Branches.SelectMany(x => x.Products).ToList();
                
            }
            else
            {

                var user = _userManager.Users.Where(x => x.UserName == User.Identity.Name).Include(x => x.Employee).ThenInclude(x => x.Branch).ThenInclude(x => x.Products).ThenInclude(x=>x.Category).FirstOrDefault();

                result = user?.Employee?.Branch?.Products;

                ViewBag.BranchId = user?.Employee?.BranchId;

            }

            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> AddMerchant()
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AddEmployee([FromQuery]string branchId)
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x=>x.Depth ==2)).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromQuery] string branchId, [FromForm] Product product)
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();
            string fileName = Guid.NewGuid().ToString() + product.Image.FileName;
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/Products", fileName);
            using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
            {
                await product.Image.CopyToAsync(stream);
            }
            product.ImageUrl = fileName;
            product.BranchId = Guid.Parse(branchId);
            
            await _productRepository.AddAsync(product);

            await _productRepository.SaveAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddMerchant(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();


            string fileName = Guid.NewGuid().ToString() + product.Image.FileName;
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/Products", fileName);
            using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
            {
                await product.Image.CopyToAsync(stream);
            }
            product.ImageUrl = fileName;


            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();
            var product = await _productRepository.GetAsync(x => x.Id.ToString() == id);
            return View(product);
        }

        public async Task<IActionResult> Update(string id, Product product)
        {
            ViewBag.Categories = await (await _categoryRepository.GetAllAsync(x => x.Depth == 2)).ToListAsync();
            var entity = await _productRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            if (product.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + product.Image.FileName;

                string path = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/Products", fileName);
                using (FileStream stream = System.IO.File.Open(path, FileMode.CreateNew))
                {
                    await product.Image.CopyToAsync(stream);
                }
                entity.ImageUrl = fileName;
            }


            entity.Name = product.Name;
            entity.Description = product.Description;
            entity.Price = product.Price;
            entity.Brand = product.Brand;
            entity.Model = product.Model;
            entity.CategoryId = product.CategoryId;


            _productRepository.Update(entity);
            await _productRepository.SaveAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _productRepository.GetAsync(x => x.Id.ToString() == id && !x.IsDeleted);
            if (entity is null)
            {
                return NotFound();
            }
            entity.IsDeleted = true;
            _productRepository.Update(entity);
            await _productRepository.SaveAsync();
            return RedirectToAction("Index");

        }
    }
}
