using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BackGammonDb.Repositories
{
    public class MessageRepositories : IDisposable
    {
        private readonly object locker;
        private BacknammonContextDb _backnammonContextDb;
        public MessageRepositories(BacknammonContextDb backnammonContextDb, object locker)
        {
            _backnammonContextDb = backnammonContextDb;
            this.locker = locker;
        }
        public void Dispose()
        {
            _backnammonContextDb.Dispose();
        }

        public bool AddMessage(Message msg)
        {
            lock (locker)
            {
                if (msg != null)
                {
                    try
                    {
                        var addedMsg = _backnammonContextDb.Messages.Add(msg);
                        var result = _backnammonContextDb.SaveChanges();

                        return true;
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
                else return false;
            }
        }

        public List<Message> GetMessages(int numberOfMessages)
        {
            lock (locker)
            {
                try
                {
                    var result = _backnammonContextDb.Messages.OrderByDescending(m => m.MessageId).Take(numberOfMessages).ToList();
                    result.Reverse();
                    return result;

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }


        }

        public void DeleteAllMessage()
        {
            try
            {
                _backnammonContextDb.Messages.RemoveRange(_backnammonContextDb.Messages);
                _backnammonContextDb.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
