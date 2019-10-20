using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Helpers;
using BackGammonWeb.Hubs;
using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BackGammonWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly DbManager _dbManager;
        IHubContext<ServerHub, ITypedHubClient> _serverHub;

        public GameController(
           DbManager dbManager,
           IHubContext<ServerHub,
           ITypedHubClient> serverHub)
        {
            _dbManager = dbManager;
            _serverHub = serverHub;
        }
        [HttpGet("addGame")]
        public string AddGame(Game  game)
        {

        }
    }
}