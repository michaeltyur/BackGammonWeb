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
        public async Task<string> AddGame(int userOneID, int userTwoID)
        {

            try
            {
                Game newGame = new Game();
                var userOne = await _dbManager.UserRepositories.GetUserByID(userOneID);
                var userTwo = await _dbManager.UserRepositories.GetUserByID(userTwoID);

                if (userOne != null && userTwo != null)
                {
                    var result = await _dbManager.GameRepositories.AddGame(newGame, userOne, userTwo);
                    if (result)
                    {
                        return await Task.FromResult("The Game is started");
                    }
                    else
                    {
                        return await Task.FromResult("An error has occurred during game adding");
                    }
                }
                else
                {
                    return await Task.FromResult("Users are not found");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpGet("getGame")]
        public async Task<Game> GetGame(int userOneID, int userTwoID)
        {
            try
            {
                return await _dbManager.GameRepositories.GetGameByUsersID(userOneID, userTwoID);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet("deleteGame")]
        public async Task<string> DeleteGame(int userID)
        {
            try
            {
                var user = await _dbManager.UserRepositories.GetUserByID(userID);
                if (user == null)
                {
                    return await Task.FromResult("User is not found");
                }
                else
                {
                    var result = await _dbManager.GameRepositories.DeleteGameByUserID(userID);
                    if (result)
                    {
                        return await Task.FromResult("The game was deleted successfully");
                    }
                    else
                    {
                        return await Task.FromResult("An error has occurred during game deleting");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}