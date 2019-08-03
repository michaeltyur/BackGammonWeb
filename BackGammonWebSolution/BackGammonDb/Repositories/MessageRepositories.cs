using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGammonDb.Repositories
{
    public class MessageRepositories : IDisposable
    {
        private BacknammonContextDb _backnammonContextDb;
        public MessageRepositories(BacknammonContextDb backnammonContextDb)
        {
            _backnammonContextDb = backnammonContextDb;
        }
        public void Dispose()
        {
            _backnammonContextDb.Dispose();
        }

        public async Task<bool> AddMessage(Message msg)
        {
            if (msg != null)
            {
                    var user = await _backnammonContextDb.Users.FindAsync(msg.UserName);

                        var count = _backnammonContextDb.Messages.Count();
                        if (count > 1000)
                        {
                            DeleteAllMessage();
                        }
                        msg.MessageId = _backnammonContextDb.Messages.Count();
                _backnammonContextDb.Messages.Add(msg);
                _backnammonContextDb.SaveChanges();
                return await Task.FromResult(true);
            }
            else return await Task.FromResult(false) ;
        }

        public void DeleteAllMessage()
        {
            _backnammonContextDb.Messages.RemoveRange(_backnammonContextDb.Messages);
            _backnammonContextDb.SaveChanges();
        }
    }
}
