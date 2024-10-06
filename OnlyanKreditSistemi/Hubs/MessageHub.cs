using Microsoft.AspNetCore.SignalR;

namespace OnlyanKreditSistemi.Hubs
{
    public class MessageHub :Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",message);
        }
    }
}
