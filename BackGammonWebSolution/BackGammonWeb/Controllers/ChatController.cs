using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackGammonDb;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackGammonWeb.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly DbManager _dbManager;
        public ChatController(DbManager dbManager)
        {
            _dbManager = dbManager;
        }

        [HttpGet("getPublicMessages")]
        public IEnumerable<Message> GetPublicMessages(int numberOfMessages)
        {
            var messages =  _dbManager.MessageRepositories.GetMessagesAsync(numberOfMessages);
            return messages;
        }
    }
}