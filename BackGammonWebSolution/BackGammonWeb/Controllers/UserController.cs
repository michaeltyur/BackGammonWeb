using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Helpers;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(DbManager dbManager, IOptions<AppSettings> appSettings)
        {
            _dbManager = dbManager;
            _appSettings = appSettings.Value;
        }

        [HttpGet("getAllUsers")]
        public async Task<List<User>> GetAllUsers()
        {

            var listUsers = await _dbManager.UserRepositories.GetAllUsers();
            return listUsers.Select(u => new User
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                IsOnline = u.IsOnline
            }).ToList();
        }
    }
}