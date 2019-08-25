using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackGammonWeb.Hubs
{
    public class ServerHub : Hub<ITypedHubClient>
    {
        public ServerHub()
        {

        }

        public void UpdateUsers(List<User> users)
        {
            if (users != null && users.Count > 0)
            {
                Clients.All.UpdateUsers(users);
            }
        }

        public void SendMessage(object message)
        {
            if (message!=null)
            {
                Clients.All.BroadcastMessage(message);
            }
        }
    }
}
