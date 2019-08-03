using System;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;

namespace BackGammonWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DbManager _dbManager;
        public AuthenticationController(DbManager dbManager)
        {
            _dbManager = dbManager;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<string> Login(UserLoginMv user)
        {
            var userAuth = await _dbManager.UserRepositories.GetUser(user.UserName, user.Password);
            var jsonUser = new
            {
                userName = userAuth.UserName,
                firstName= userAuth.FirstName,
                lastName= userAuth.LastName,
                token=""
            };


            //  var signingKey = Convert.FromBase64String(_configuration["Jwt:SigningSecret"]);
            // var expiryDuration = int.Parse(_configuration["Jwt:ExpiryDuration"]);

            return JsonConvert.SerializeObject(jsonUser);
        }


    }
}