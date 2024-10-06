using OnlyanKreditSistemi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OnlyanKreditSistemi.Models;

namespace OnlyanKreditSistemi.Hubs;

public class ChatHub : Hub
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;

    public ChatHub(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task SendMessage(string userId, string message)
    {
        AppUser user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var receiverId = (await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name))
                .Id;

            if (!string.IsNullOrWhiteSpace(user.ConnectionId))
            {
                await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", receiverId, message);
            }
        }
    }

    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;

        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            AppUser? appUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            appUser.ConnectionId = connectionId;
            await _userManager.UpdateAsync(appUser);
            await Clients.All.SendAsync("Login", appUser.Id);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            AppUser? appUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            appUser.ConnectionId = null;
            await _userManager.UpdateAsync(appUser);
            await Clients.All.SendAsync("Logout", appUser.Id);
        }
        await base.OnDisconnectedAsync(exception);
    }
}