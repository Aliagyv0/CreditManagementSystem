using OnlyanKreditSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.Controllers;

public class ChatController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public ChatController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "Admin"))
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Customer");
            var users = (await _userManager.GetUsersInRoleAsync("Customer"))
                .Where(x => adminUsers.Contains(x))
                .ToList();
            return View(users);
        }
        else
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var admins = (await _userManager.GetUsersInRoleAsync("Admin"))
                .Where(x => adminUsers.Contains(x))
                .ToList();
            return View(admins);
        }

    }
}