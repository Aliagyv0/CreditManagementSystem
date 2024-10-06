
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;

namespace AvtosRestoran.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin,Admin,Merchant")]
    public class BranchController : Controller
    {
        private readonly IRepository<Branch> _repository;
        private readonly IRepository<Merchant> _merchantRepository;

        public BranchController(IRepository<Branch> repository, IRepository<Merchant> merchantRepository)
        {
            _repository = repository;
            _merchantRepository = merchantRepository;
        }

        public async Task<IActionResult> Index()
        {
            var branchs = await (await _repository.GetAllAsync(x => !x.IsDeleted)).Include(x => x.Employees).Include(m=>m.Merchant).ThenInclude(x=>x.AppUser).ToListAsync();
            return View(branchs);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Merchants = await (await _merchantRepository.GetAllAsync(x => !x.IsDeleted)).Include(x=>x.AppUser).ToListAsync();


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Branch branch)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _repository.AddAsync(branch);
            await _repository.SaveAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]

        public async Task<IActionResult> Update(string id)
        {
            ViewBag.Merchants = await (await _merchantRepository.GetAllAsync(x => !x.IsDeleted)).Include(x => x.AppUser).ToListAsync();

            var Branch = await _repository.GetAsync(x => x.Id.ToString() == id);
            return View(Branch);
        }
        public async Task<IActionResult> Update(string id, Branch branch)
        {

            var uptadedBranch = await _repository.GetAsync(x => x.Id.ToString() == id);
            if (uptadedBranch == null)
            {
                return NotFound();
            }
            uptadedBranch.Name = branch.Name;
            uptadedBranch.Address = branch.Address;
            uptadedBranch.Description = branch.Description;
            uptadedBranch.MerchantId=branch.MerchantId;
            _repository.Update(uptadedBranch);
            await _repository.SaveAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var Branch = await _repository.GetAsync(c => c.Id.ToString() == id);
            if (Branch == null)
            {
                return NotFound();
            }

            Branch.IsDeleted = true;
            await _repository.SaveAsync();
            return RedirectToAction("Index");
        }
    }
}
