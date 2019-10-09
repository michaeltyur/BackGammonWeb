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

        public void SendMessage(Message message)
        {
            var userName = GetUserName();
            //_userIdProvider.GetUserId(Context.);

            if (message != null)
            {
                message.Date = DateTime.Now;

                var json = new JavaScriptSerializer().Serialize(message);

                _dbManager.MessageRepositories.AddMessage(message);

                if (message.GroupName == "public")
                {
                    Clients.All.BroadcastMessage(json);
                }
                else
                {
                    Clients.Group(message.GroupName).BroadcastMessage(json);
                }

            }
        }

        private string GetUserName()
        {
            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var userName = claimsIdentity.Claims.FirstOrDefault(u => u.Type == "UserName").Value;
            return userName;
        }
        private int GetUserID()
        {
            var claimsIdentity = (ClaimsIdentity)Context.User.Identity;
            var userID = claimsIdentity.Claims.FirstOrDefault(u => u.Type == "UserID").Value;
            return  int.Parse(userID);
        }
        public async Task<ChatInvitation> AddToGroup(int secondUserID)
        {
            ChatInvitation chatInvitation = new ChatInvitation();
            try
            {
                var userID = GetUserID();

               // var secondUser = await _dbManager.UserRepositories.GetUserByID(secondUserID);
                PrivateChat privateChat = _dbManager.UserRepositories.GetPrivateChat(userID, secondUserID);

                if (privateChat != null)
                {
                    chatInvitation.InviterID = secondUserID;
                   // chatInvitation.InviterName = secondUser.UserName;
                    chatInvitation.GroupName = privateChat.GroupName;
                    chatInvitation.Message = "Switch to chat";
                    return chatInvitation;
                }

                var secondUserConnectionId = _dbManager.UserRepositories.GetSignalRConnection(secondUserID);
                var groupName = Guid.NewGuid().ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                await Groups.AddToGroupAsync(secondUserConnectionId, groupName);


                var result = _dbManager.UserRepositories.AddPrivateChat(userID, secondUserID, groupName);
                if (!result)
                {
                    chatInvitation.Message = "The Error";
                    return chatInvitation;
                }

                chatInvitation.InviterID = userID;
                chatInvitation.InviterName = GetUserName();
                chatInvitation.GroupName = groupName;
                chatInvitation.Message = "Success: The Chat is successfully started";
                await Clients.Client(secondUserConnectionId).InviteToPrivateChat(chatInvitation);

                chatInvitation.InviterID = secondUserID;

                await Task.Run(() =>
                {
                    UpdatePrivateChatsByUser(userID);
                });
                await Task.Run(() =>
                {
                    UpdatePrivateChatsByUser(secondUserID);
                });

                return chatInvitation;
            }
            catch (Exception ex)
            {
                chatInvitation.Message = "The Error";
                return chatInvitation;
            }

        }

        public async Task<bool> ClosePrivateChat(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                try
                {
                    var privateChat = _dbManager.UserRepositories.DeletePrivateChat(groupName);
                    var userName = GetUserName();
                    int userID = GetUserID();

                    var msg = new Message();

                    msg.UserName = "auto generated";
                    msg.Content = "The Chat was closed by user";
                    msg.Date = DateTime.Now;
                    msg.GroupName = groupName;

                    var json = new JavaScriptSerializer().Serialize(msg);

                    await Clients.Group(groupName).BroadcastMessage(json);

                    await Clients.Group(groupName).PrivateChatClosed(new ChatClosing
                    {
                        CloserID = userID,
                        GroupName = groupName
                    }) ;

                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                    return true;
                }
                catch (Exception)
                {

                    return false;
                }

               
            }
            else return false;


        }

        public async void UpdatePrivateChatsByUser(int userID)
        {
            var privateChatByUsers = _dbManager.UserRepositories.GetPrivateChatsByUserID(userID);
            string signalRConnection = _dbManager.UserRepositories.GetSignalRConnection(userID);
            await  Clients.Client(signalRConnection).UpdatePrivateChatsByUser(privateChatByUsers);
        }
    }
}
