using BackGammonDb;
using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackGammonWeb.Hubs
{
    [Authorize]
    public class ServerHub : Hub<ITypedHubClient>
    {
        IUserIdProvider _userIdProvider;
        DbManager _dbManager;
        public ServerHub(IUserIdProvider userIdProvider, DbManager dbManager)
        {
            _userIdProvider = userIdProvider;
             _dbManager=dbManager;

        }

        public void UpdateUsers(List<User> users)
        {
            if (users != null && users.Count > 0)
            {
                Clients.All.UpdateUsers(users);
            }
        }

        public void SendMessage(string message)
        {
            //var userName = User .  Identity.Name;
            //_userIdProvider.GetUserId(Context.);
            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var userName = claimsIdentity.Claims.FirstOrDefault(u=>u.Type=="UserName").Value;
            if (!string.IsNullOrEmpty(message))
            {
                Message newMessage = new Message
                {
                    Content = message,
                    Date = DateTime.Now,
                    UserName = userName
                };

                var json = new JavaScriptSerializer().Serialize(newMessage);

                _dbManager.MessageRepositories.AddMessage(newMessage);


                Clients.All.BroadcastMessage(json);
            }
        }
    }
}
