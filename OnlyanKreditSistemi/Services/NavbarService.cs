using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;
using OnlyanKreditSistemi.Services.Interfaces;
using OnlyanKreditSistemi.ViewModels;

namespace OnlyanKreditSistemi.Services
{
    public class NavbarService : INavbarService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Merchant> _merchantRepository;

        public NavbarService(IRepository<Merchant> merchantRepository, IRepository<Category> categoryRepository)
        {
            _merchantRepository = merchantRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<GeneralVM> GetNavBar()
        {
            var categories = await (await _categoryRepository.GetAllAsync(x => !x.IsDeleted && x.Depth ==0)).Include(x=>x.Children).ThenInclude(x=>x.Children).ToListAsync();

            var merchants = await (await _merchantRepository.GetAllAsync(x => !x.IsDeleted)).Include(x=>x.AppUser).Include(x=>x.Branches).ToListAsync();

            GeneralVM model = new()
            {
                Categories = categories,
                Merchants = merchants
            };
            return model;

        }
    }
}
