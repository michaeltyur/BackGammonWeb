using Common.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGammonDb.Repositories
{
    public class UserRepositories : IDisposable
    {
        private readonly object locker;
        private BacknammonContextDb _backnammonContextDb;
        public UserRepositories(BacknammonContextDb backnammonContextDb, object locker)
        {
            _backnammonContextDb = backnammonContextDb;
            this.locker = locker;
        }

        public void Dispose()
        {
            _backnammonContextDb.Dispose();
        }

        public User GetUser(string username, string password)
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return null;
                }
                User user;
                user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
                return user;
            }

        }

        public User GetUserByName(string username)
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(username))
                {
                    return null;
                }
                User user;
                user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == username);
                return user;
            }

        }

        public bool AddUser(User user)
        {
            lock (locker)
            {
                int result = 0;
                if (user != null)
                {

                    _backnammonContextDb.Users.AddAsync(user);
                    result = _backnammonContextDb.SaveChanges();
                    return result > 0;

                }
                else return false;
            }

        }

        public bool UpdateUser(User user)
        {
            lock (locker)
            {
                if (user != null)
                {
                    try
                    {
                        _backnammonContextDb.Users.Update(user);

                        var result = _backnammonContextDb.SaveChanges();

                        return result > 0;
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                }
                return false;
            }

        }

        public IEnumerable<User> GetAllUsers()
        {
            lock (locker)
            {
                List<User> listUsers = new List<User>();

                listUsers = _backnammonContextDb.Users.ToList();

                return listUsers;
            }

        }

        public bool DeleteUser(string userName, string password)
        {
            lock (locker)
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    User user = GetUser(userName, password);
                    if (user != null)
                    {

                        _backnammonContextDb.Users.Remove(user);
                        var res = SaveChanges();

                        return res;

                    }
                    else return false;
                }
                else return false;
            }

        }

        private bool SaveChanges()
        {
            return _backnammonContextDb.SaveChanges() > 0;
        }

        public bool SetUserOnLine(User user)
        {
            lock (locker)
            {
                try
                {
                    user.IsOnline = true;
                    UpdateUser(user);
                    return UpdateUser(user);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool SetUserOffLine(User user)
        {
            lock (locker)
            {
                try
                {
                    user.IsOnline = false;
                    var result = UpdateUser(user);
                    return result;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool SetSignalRConnection(string connectionId, string userName)
        {
            lock (locker)
            {
                try
                {
                    User user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        user.SignalRConnectionID = connectionId;
                        _backnammonContextDb.Users.Update(user);
                        return SaveChanges();
                    }
                    else return false;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }

        public bool DeleteSignalRConnection(string userName)
        {
            lock (locker)
            {
                try
                {
                    User user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user != null)
                    {
                        user.SignalRConnectionID = null;
                        _backnammonContextDb.Users.Update(user);
                        return SaveChanges();
                    }
                    else return false;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

        }

        public string GetSignalRConnection(string userName)
        {
            lock (locker)
            {
                try
                {
                    User user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == userName);
                    if (user != null && user.SignalRConnectionID != null)
                    {
                        return user.SignalRConnectionID;
                    }
                    else return string.Empty;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        #region PrivateChat

        public bool AddPrivateChat(string firstUserName, string secondUserName, string groupName)
        {
            lock (locker)
            {
                try
                {
                    User firstUser = _backnammonContextDb.Users.FirstOrDefault(user => user.UserName == firstUserName);
                    User secondUser = _backnammonContextDb.Users.FirstOrDefault(user => user.UserName == secondUserName);
                    if (firstUser != null && secondUser != null && !string.IsNullOrEmpty(groupName))
                    {

                        PrivateChat privateChat = new PrivateChat {
                            GroupName = groupName,
                            TimeCreation = DateTime.Now,
                            Users=new List<User>()
                            {
                                firstUser,
                                secondUser
                            }
                        };
                        _backnammonContextDb.PrivateChats.Add(privateChat);
                        return SaveChanges();
                    }
                    else return false;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

        }

        public PrivateChat GetPrivateChat(string firstUserName, string secondUserName)
        {
            lock (locker)
            {
                try
                {
                    //var privateChat = _backnammonContextDb.PrivateChats
                    //  .FromSql($"exec GetPrivateChat @UserOneName={firstUserName}, @UserTwoName={secondUserName}")
                    //  .FirstOrDefault();


                    var firstUser = _backnammonContextDb.Users.Where(u => u.UserName == firstUserName).FirstOrDefault();

                    firstUser.Collection(u=>u.)
                    var secondUser = _backnammonContextDb.Users.Where(u => u.UserName == secondUserName).FirstOrDefault();
                    var privateChat = _backnammonContextDb.PrivateChats
                        .Where(x=>x.Users.Any(upc=> upc.UserID == firstUser.UserID).Select(upch => upch. == secondUser))

                    return privateChat;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public List<PrivateChat> DeletePrivateChatsAll(string userName)
        {
            lock (locker)
            {
                try
                {
                    var privateChats = _backnammonContextDb
                        .PrivateChats
                        .FromSql("exec DeletePrivateChat @UserName", new SqlParameter("UserName", userName))
                        .ToList();
                    return privateChats;
                }
                catch (Exception ex)
                {
                    return null;

                }

            }
        }

        public PrivateChat DeletePrivateChat(string groupName)
        {
            lock (locker)
            {
                try
                {
                    var privateChat = _backnammonContextDb.PrivateChats.FirstOrDefault(chat => chat.GroupName == groupName);
                    if (privateChat == null) return null;

                    _backnammonContextDb.PrivateChats.Remove(privateChat);
                    var result = SaveChanges();
                    if (result)
                    {
                        return privateChat;
                    }
                    else return null;

                }
                catch (Exception ex)
                {
                    return null;

                }

            }
        }

        #endregion
    }
}

