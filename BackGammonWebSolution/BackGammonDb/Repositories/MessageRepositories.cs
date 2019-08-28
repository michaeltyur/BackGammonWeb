using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackGammonDb.Repositories
{
    public class MessageRepositories : IDisposable
    {
        private static readonly Object locker = new Object();
        Semaphore semaphoreObject = new Semaphore(initialCount: 1, maximumCount: 1);
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
                try
                {
                    // var user =  _backnammonContextDb.Users.FirstOrDefault(u=>u.UserName == msg.UserName);

                    //var count = _backnammonContextDb.Messages.Count();
                    //if (count > 1000)
                    //{
                    //    DeleteAllMessage();
                    //}
                    //msg.MessageId = _backnammonContextDb.Messages.Count();
                    semaphoreObject.WaitOne();
                    var addedMsg = await _backnammonContextDb.Messages.AddAsync(msg);
                    var result = await _backnammonContextDb.SaveChangesAsync();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally
                {
                    semaphoreObject.Release();
                }

            }
            else return await Task.FromResult(false);



        }

        public List<Message> GetMessagesAsync(int numberOfMessages)
        {
            lock (locker)
            {
                try
                {
                    var result = _backnammonContextDb.Messages.OrderByDescending(m => m.MessageId).Take(numberOfMessages).ToList();
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
            finally
            {
                semaphoreObject.Release();
            }
        }
    }
}
