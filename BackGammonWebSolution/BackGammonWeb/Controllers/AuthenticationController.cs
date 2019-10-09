using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Helpers;
using BackGammonWeb.Hubs;
using BackGammonWeb.Models;
using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nancy.Json;
using Newtonsoft.Json;

namespace BackGammonWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly DbManager _dbManager;
        IHubContext<ServerHub, ITypedHubClient> _serverHub;
        private readonly UserController _userController;
        public AuthenticationController(
            DbManager dbManager,
            IOptions<AppSettings> appSettings,
            IHubContext<ServerHub, ITypedHubClient> serverHub,
            UserController userController)
        {
            _dbManager = dbManager;
            _serverHub = serverHub;
            _appSettings = appSettings.Value;
            _userController = userController;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public string Login(UserLoginMv user)
        {
            try
            {
                var response = JsonConvert.SerializeObject(new { error = "login or password is incorrect" });

                User userAuth = _dbManager.UserRepositories.GetUser(user.UserName, user.Password);

                if (userAuth != null)
                {
                    if (userAuth.IsOnline)
                    {
                        // return JsonConvert.SerializeObject(new { error=$"The User {user.UserName} is allready logged in" });
                    }
                    var tokenString = GenerateJSONWebToken(userAuth);
                    var jsonUser = new
                    {
                        userID= userAuth.UserID,
                        userName = userAuth.UserName,
                        firstName = userAuth.FirstName,
                        lastName = userAuth.LastName,
                        token = tokenString
                    };
                    response = JsonConvert.SerializeObject(jsonUser);

                    Task.Run(() =>
                    {
                        _dbManager.UserRepositories.SetUserOnLine(userAuth);
                    });

                    Task.Run(() =>
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(500));
                        _userController.UpdateUsers(userAuth.UserID);

                    });
                    


                }

                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public string Registration(User user)
        {

            if (string.IsNullOrEmpty(user.FirstName)
                || string.IsNullOrEmpty(user.LastName)
                || string.IsNullOrEmpty(user.UserName)
                || string.IsNullOrEmpty(user.Password))
            {
                return JsonConvert.SerializeObject(new { error = "one ore more parameters are missing" });
            }

            var userFromDb = _dbManager.UserRepositories.GetUserByName(user.UserName);

            if (userFromDb != null)
            {
                return JsonConvert.SerializeObject(new { error = "The name already exists" });
            }
            else
            {
                var result = _dbManager.UserRepositories.AddUser(user);
                if (result)
                {

                    return Login(new UserLoginMv
                    {
                        UserName = user.UserName,
                        Password = user.Password
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new { error = "An error occurred please try again" });
                }
            }
        }

        [HttpGet("logout")]
        public string Logout(string userName)
        {
            var response = "";

            var userFromDb = _dbManager.UserRepositories.GetUserByName(userName);
            if (userFromDb != null)
            {
                try
                {
                    _dbManager.UserRepositories.SetUserOffLine(userFromDb);

                    response = JsonConvert.SerializeObject(new { success = "The user did logged out" });

                   // UpdateUsers(userFromDb.UserID);

                    Task.Run(() =>
                    {
                        var privateChats = _dbManager.UserRepositories.DeletePrivateChatsAll(userFromDb.UserID);

                        if (privateChats != null && privateChats.Count > 0)
                        {
                            foreach (var privateChat in privateChats)
                            {
                                _serverHub.Groups.RemoveFromGroupAsync(userFromDb.SignalRConnectionID, privateChat.GroupName);
                            }

                        }

                        _userController.UpdateUsers(userFromDb.UserID);
                       // UpdatePrivateChatsByUser(userFromDb.UserID);

                    });

                }
                catch (Exception)
                {
                    response = JsonConvert.SerializeObject(new { error = "An error occurred please try again" });
                }

            }
            else
            {
                response = JsonConvert.SerializeObject(new { error = "The user not found" });
            }

            return response;
        }

        private string GenerateJSONWebToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                                new Claim("UserID", user.UserID.ToString()),
                                new Claim("UserName", user.UserName),
                                new Claim("FirstName", user.FirstName),
                                new Claim("LastName", user.LastName)
                              };

            var token = new JwtSecurityToken(
              issuer: null,
              audience: null,
              claims: claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return encodedJwt;
        }
        //private async void UpdateUsers(int userID)
        //{
        //    try
        //    {
        //        List<User> users = _dbManager.UserRepositories.GetAllUsers().ToList();
        //        if (users != null && users.Count > 0)
        //        {
        //            await _serverHub.Clients.All.UpdateUsers(users);
        //            UpdatePrivateChatsByUser(userID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}

        //public async void UpdatePrivateChatsByUser(int userID)
        //{
        //    var privateChatByUsers = _dbManager.UserRepositories.GetPrivateChatsByUserID(userID);
        //    string signalRConnection = _dbManager.UserRepositories.GetSignalRConnection(userID);
        //    await _serverHub.Clients.Client(signalRConnection).UpdatePrivateChatsByUser(privateChatByUsers);
        //}


    }
}