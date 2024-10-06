using OnlyanKreditSistemi.ViewModels;

namespace OnlyanKreditSistemi.Services.Interfaces
{
    public interface INavbarService
    {
        Task<GeneralVM> GetNavBar();
    }
}
