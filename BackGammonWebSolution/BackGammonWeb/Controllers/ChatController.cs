using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackGammonDb;
using BackGammonWeb.Hubs;
using BackGammonWeb.Services;
using Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BackGammonWeb.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly DbManager _dbManager;
        IHubContext<ServerHub, ITypedHubClient> _serverHub;
        public ChatController(DbManager dbManager, IHubContext<ServerHub, ITypedHubClient> serverHub)
        {
            _dbManager = dbManager;
            _serverHub = serverHub;
        }

        [HttpGet("getPublicMessages")]
        public IEnumerable<Message> GetPublicMessages(int numberOfMessages)
        {
            try
            {
                var messages = _dbManager.MessageRepositories.GetMessages(numberOfMessages,"public");
                return messages;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet("getPrivateMessages")]
        public IEnumerable<Message> GetPrivateMessages(int numberOfMessages,string groupName)
        {
            try
            {
                var messages = _dbManager.MessageRepositories.GetMessages(numberOfMessages, groupName);
                return messages;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpGet("closePrivateChat")]
        public bool ClosePrivateChat(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                var privateChat = _dbManager.UserRepositories.DeletePrivateChat(groupName);
                if (privateChat!=null)
                {
                    return true;
                }
                return false;
            }
            else return false;
            

        }
    }
}