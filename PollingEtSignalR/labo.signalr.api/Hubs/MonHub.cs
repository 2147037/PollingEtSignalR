using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;
using System;

namespace labo.signalr.api.Hubs
{
    public class MonHub: Hub
    {
        ApplicationDbContext _context;
        public static List<UselessTask> _mesTasks = new List<UselessTask>();

        public static class UserHandler
        {
            public static HashSet<string> ConnectedIds = new HashSet<string>();
        }

        public MonHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            base.OnConnectedAsync();
            UserHandler.ConnectedIds.Add(Context.ConnectionId);

            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await Clients.Caller.SendAsync("TaskList", _mesTasks);
        }

        public async Task Ajouter(string text)
        {
            _mesTasks.Add(new UselessTask { Text = text, Id = _mesTasks.Count() + 1});
            await Clients.All.SendAsync("TaskList", _mesTasks);
        }
        public async Task Complete(int id)
        {
            _mesTasks.FirstOrDefault(x => x.Id == id).Completed = true;
            await Clients.All.SendAsync("TaskList", _mesTasks);

        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            base.OnDisconnectedAsync(exception);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
        }
    }
}
