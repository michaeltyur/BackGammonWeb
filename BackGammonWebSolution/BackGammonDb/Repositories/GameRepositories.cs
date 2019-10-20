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
            return await _backnammonContextDb.SaveChangesAsync() > 0;
        }


        public async Task<bool> AddGame(Game game)
        {
            try
            {
                var gameFromDb = await _backnammonContextDb.Games.FirstOrDefaultAsync(g => g.GameID == game.GameID);
                if (gameFromDb != null)
                {
                  return false;
                }

                await _backnammonContextDb.Games.AddAsync(game);

                return await SaveChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
