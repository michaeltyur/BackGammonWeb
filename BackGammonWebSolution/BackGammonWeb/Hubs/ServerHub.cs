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
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ServerHub : Hub<ITypedHubClient>
    {
        IUserIdProvider _userIdProvider;
        DbManager _dbManager;
        public ServerHub(IUserIdProvider userIdProvider, DbManager dbManager)
        {
            _userIdProvider = userIdProvider;
            _dbManager = dbManager;

        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userName = GetUserName();
                _dbManager.UserRepositories.SetSignalRConnection(Context.ConnectionId, userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userName = GetUserName();
                _dbManager.UserRepositories.DeleteSignalRConnection(userName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await base.OnDisconnectedAsync(exception);
        }


        public void UpdateUsers()
        {
            try
            {
                List<User> users = _dbManager.UserRepositories.GetAllUsers().ToList();
                if (users != null && users.Count > 0)
                {
                    Clients.All.UpdateUsers(users);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SendMessage(string message)
        {
            var userName = GetUserName();
            //_userIdProvider.GetUserId(Context.);

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

        private string GetUserName()
        {
            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var userName = claimsIdentity.Claims.FirstOrDefault(u => u.Type == "UserName").Value;
            return userName;
        }

        public async Task AddToGroup(string secondUserName)
        {
            var userName = GetUserName();
            var secondUserConnectionId = _dbManager.UserRepositories.GetSignalRConnection(secondUserName);
            string groupName = $"{userName}/{secondUserName}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Groups.AddToGroupAsync(secondUserConnectionId, groupName);

            string inviterName = GetUserName();
            await Clients.Client(secondUserConnectionId).InviteToPrivateChat(inviterName);
        }
    }
}
