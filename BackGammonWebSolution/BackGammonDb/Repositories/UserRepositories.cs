using Common.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

                        return res > 0;

                    }
                    else return false;
                }
                else return false;
            }

        }

        private int SaveChanges()
        {
            return _backnammonContextDb.SaveChanges();
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

        public bool SetSignalRConnection(string connectionId, string userName )
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
                        return SaveChanges() > 0;
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
                        return SaveChanges() > 0;
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
                    if (user != null && user.SignalRConnectionID!=null)
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
    }
}

