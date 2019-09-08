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
        private string GetUserID()
        {
            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var userName = claimsIdentity.Claims.FirstOrDefault(u => u.Type == "UserID").Value;
            return userName;
        }
        public async Task<ChatInvitation> AddToGroup(string secondUserName)
        {
            ChatInvitation chatInvitation = new ChatInvitation();
            try
            {
                var userName = GetUserName();
                PrivateChat privateChat = _dbManager.UserRepositories.GetPrivateChat(userName, secondUserName);

                if (privateChat != null)
                {
                    chatInvitation.GroupName = privateChat.GroupName;
                    chatInvitation.Message = "Switch to chat";
                    return chatInvitation;
                }

                var secondUserConnectionId = _dbManager.UserRepositories.GetSignalRConnection(secondUserName);
                var groupName = Guid.NewGuid().ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Groups.AddToGroupAsync(secondUserConnectionId, groupName);


                var result = _dbManager.UserRepositories.AddPrivateChat(userName, secondUserName, groupName);

                await Clients.Client(secondUserConnectionId).InviteToPrivateChat(userName);

                chatInvitation.InvaterName = userName;
                chatInvitation.GroupName = groupName;
                chatInvitation.Message = "Success: The Chat is successfully started";
                return chatInvitation;
            }
            catch (Exception ex)
            {
                chatInvitation.Message = "The Error";
                return  chatInvitation;
            }

        }
    }
}
