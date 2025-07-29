using Microsoft.AspNetCore.SignalR;

namespace SmartWear.Hubs
{
    public class ChatHub : Hub
    {
        // Broadcast tin nhắn đến tất cả client
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
