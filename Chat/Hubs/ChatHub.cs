using Chat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task SendMessage(string userId, string message)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string user = _contextAccessor.HttpContext.User.Identity.Name;
                if (!string.IsNullOrWhiteSpace(appUser.ConnectionId))
                {
                    await Clients.Client(appUser.ConnectionId).SendAsync("ReceiveMessage", user, message, timestamp);
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            string connid = Context.ConnectionId;
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser? appUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);

                appUser.ConnectionId = connid;
                await _userManager.UpdateAsync(appUser);

                string userId = appUser.Id;
                await Clients.All.SendAsync("Loggin", userId);
            }

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser? appUser = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);

                appUser.ConnectionId = null;
                await _userManager.UpdateAsync(appUser);

                string userId = appUser.Id;
                await Clients.All.SendAsync("Logout", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
