using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;
using OnlyanKreditSistemi.Repository.Interfaces;
using OnlyanKreditSistemi.ViewModels;
using System.Diagnostics;

namespace OnlyanKreditSistemi.Controllers
{
    public class HomeController : Controller
    {
       
        private readonly IRepository<Slider> _sliderRepository;
        private readonly IRepository<Product> _productRepository;
  
        
        public HomeController(IRepository<Slider> sliderRepository, IRepository<Product> productRepository)
        {
          
            _sliderRepository = sliderRepository;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {

            var sliders = await (await _sliderRepository.GetAllAsync(x => !x.IsDeleted && x.IsActive)).ToListAsync();

            var products = await (await _productRepository.GetAllAsync(x => !x.IsDeleted)).ToListAsync();



            HomeVM vmModel = new()
            {
                Sliders = sliders,
                Products = products,
            };
            return View(vmModel);


        }


    }
}
