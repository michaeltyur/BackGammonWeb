using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Helpers;
using BackGammonWeb.Hubs;
using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BackGammonWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly DbManager _dbManager;
        IHubContext<ServerHub, ITypedHubClient> _serverHub;

        public UserController(
            DbManager dbManager,
            IOptions<AppSettings> appSettings,
            IHubContext<ServerHub,
            ITypedHubClient> serverHub)
        {
            _dbManager = dbManager;
            _appSettings = appSettings.Value;
            _serverHub = serverHub;
            _userManager = userManager;
        }

        [HttpGet("getAllUsers")]
        public IEnumerable<User> GetAllUsers()
        {
            try
            {
                var userID = (int.Parse(User.Claims.FirstOrDefault(u => u.Type == "UserID").Value));
                var listUsers = _dbManager.UserRepositories.GetAllUsers();
                var users = listUsers.Select(u => new User
                {
                    UserID = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    IsOnline = u.IsOnline
                }).ToList();
                UpdatePrivateChatsByUser(userID);
                return users;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }        

        public async void UpdateUsers(int userID)
        {
            try
            {
                List<User> users = _dbManager.UserRepositories.GetAllUsers().ToList();
                if (users != null && users.Count > 0)
                {
                    await _serverHub.Clients.All.UpdateUsers(users);
                    UpdatePrivateChatsByUser(userID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("updateUserOnline")]
        public async void UpdateUserOnline()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
               // User.Claims["UserID"]
                var userID = _userManager.GetUserId(User);
            }
            
        }

        private async void UpdatePrivateChatsByUser(int userID)
        {
            var privateChatByUsers = _dbManager.UserRepositories.GetPrivateChatsByUserID(userID);
            string signalRConnection = _dbManager.UserRepositories.GetSignalRConnection(userID);
            await _serverHub.Clients.Client(signalRConnection).UpdatePrivateChatsByUser(privateChatByUsers);
        }
    }
}