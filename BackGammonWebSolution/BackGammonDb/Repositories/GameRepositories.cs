using Common.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BackGammonDb.Repositories
{
    public class GameRepositories : IDisposable
    {
        private readonly object locker;
        private BacknammonContextDb _backnammonContextDb;
        public GameRepositories(BacknammonContextDb backnammonContextDb, object locker)
        {
            _backnammonContextDb = backnammonContextDb;
            this.locker = locker;
        }

        public void Dispose()
        {
            _backnammonContextDb.Dispose();
        }

        private async Task<bool> SaveChanges()
        {
            try
            {
                return await _backnammonContextDb.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Add to db the game according given gamers ids
        /// </summary>
        /// <param name="game">empty instance of game</param>
        /// <param name="userOne">first gamer</param>
        /// <param name="userTwo">second gamer</param>
        /// <returns></returns>
        public async Task<bool> AddGame(Game game, User userOne, User userTwo)
        {
            try
            {
                var gameFromDb = await _backnammonContextDb.Games.FirstOrDefaultAsync(g => g.GameID == game.GameID);
                if (gameFromDb != null)
                {
                    return false;
                }

                await _backnammonContextDb.Games.AddAsync(game);

                await SaveChanges();

                UserGame[] userGames = new UserGame[2];

                userGames[0] = new UserGame
                {
                    GameID = game.GameID,
                    UserID = userOne.UserID
                };

                userGames[1] = new UserGame
                {
                    GameID = game.GameID,
                    UserID = userTwo.UserID
                };

                await _backnammonContextDb.UserGames.AddAsync(userGames[0]);
                await _backnammonContextDb.UserGames.AddAsync(userGames[1]);

                return await SaveChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Game> GetGameByUsersID(int userOneID, int userTwoID)
        {
            try
            {
                var users = _backnammonContextDb.Users;
                var games = _backnammonContextDb.Games;
                var userToGames = _backnammonContextDb.UserGames;

                var gamesByUserOne = GetGamesByUserID(userOneID);

                var gamesByUserTwo = GetGamesByUserID(userTwoID);

                var unionGames = (
                    from g in gamesByUserOne
                    join g2 in gamesByUserTwo on g.GameID equals g2.GameID
                    select g);

                if (unionGames != null && unionGames.Count() > 0)
                {
                    return await unionGames.LastOrDefaultAsync();
                }

                else return await Task.FromResult(default(Game));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private IQueryable<Game> GetGamesByUserID(int userID)
        {
            var users = _backnammonContextDb.Users;
            var games = _backnammonContextDb.Games;
            var userToGames = _backnammonContextDb.UserGames;
            return (
                   from g in games
                   join ug in userToGames on g.GameID equals ug.GameID
                   join u in users on ug.UserID equals u.UserID
                   where u.UserID == userID
                   select g);
        }

        public async Task<bool> DeleteGameByUserID(int userID)
        {
            try
            {
                var gamesQuery = GetGamesByUserID(userID);//Games for delete
                var userToGames = _backnammonContextDb.UserGames;
                var userToGamesQuery = (
                    from ug in userToGames
                    join g in gamesQuery on ug.GameID equals g.GameID
                    select ug
                    );//Join tables rows for delete

                var gamesList = gamesQuery.ToList();
                var userToGamesList = userToGamesQuery.ToList();

                _backnammonContextDb.UserGames.RemoveRange(userToGamesList);

                var result = await SaveChanges();

                _backnammonContextDb.Games.RemoveRange(gamesList);

                var result2 = await SaveChanges();

                return result2;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
