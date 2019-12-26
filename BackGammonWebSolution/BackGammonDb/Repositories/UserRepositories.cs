using Common.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public async Task <User> GetUserByID(int userID)
        {

            User user;
            if (userID<=0)
            {
                return null;
            }
            user = await _backnammonContextDb.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            return user;

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

        /// <summary>
        /// Saves change in db
        /// </summary>
        /// <returns>returns true when action successfully or false when not</returns>
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
                        // _backnammonContextDb.Users.Update(user);

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
                        // _backnammonContextDb.Users.Update(user);
                      
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

        public string GetSignalRConnection(int userID)
        {
            lock (locker)
            {
                try
                {
                    User user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserID == userID);
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

        public async Task<bool> UserOnlineUpdate(int userID)
        {
            var user = await _backnammonContextDb.Users.FirstOrDefaultAsync(u => u.UserID == userID);
            if (user != null)
            {
                var userAdditionalData = await _backnammonContextDb.UserAddInfos.FirstOrDefaultAsync(data => data.UserID == userID);
                userAdditionalData.LastVisitTime = DateTime.Now;
            }
            return SaveChanges();
        } 

        #region PrivateChat

        public bool AddPrivateChat(int firstUserID, int secondUserID, string groupName)
        {
            lock (locker)
            {
                try
                {

                    if (firstUserID > 0 && secondUserID > 0 && !string.IsNullOrEmpty(groupName))
                    {


                        int privateChatID = _backnammonContextDb.Database.ExecuteSqlCommand($"InsertPrivateChat {firstUserID}, {secondUserID}, {groupName}");
                        return privateChatID > 0;
                    }
                    else return false;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

        }

        public PrivateChat GetPrivateChat(int firstUserID, int secondUserID)
        {
            lock (locker)
            {
                try
                {
                    var privateChat = _backnammonContextDb.PrivateChats
                      .FromSql($"GetPrivateChat {firstUserID}, {secondUserID}")
                      .FirstOrDefault();


                    //var firstUser = _backnammonContextDb.Users
                    //    .Where(u => u.UserName == firstUserName)
                    //    .FirstOrDefault();

                    //var secondUser = _backnammonContextDb.Users
                    //    .Where(u => u.UserName == secondUserName)
                    //    .FirstOrDefault();

                    //var privateChat = _backnammonContextDb.PrivateChats
                    //    .Where(x => x.UserPrivateChats
                    //    .Any(upc => upc.UserID == firstUser.UserID))
                    //    .Where(x => x.UserPrivateChats
                    //    .Any(upc => upc.UserID == secondUser.UserID))
                    //    .FirstOrDefault();

                    return privateChat;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }

        public List<PrivateChat> DeletePrivateChatsAll(int userID)
        {
            lock (locker)
            {
                try
                {
                    var privateChats = _backnammonContextDb
                        .PrivateChats
                        .FromSql($"DeletePrivateChat {userID}")
                        .ToList();
                    return privateChats;
                }
                catch (Exception ex)
                {
                    throw ex;
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

        public ICollection<PrivateChatByUser> GetPrivateChatsByUserID(int userID)
        {
            lock (locker)
            {
                List<PrivateChatByUser> privateChatsPlus = new List<PrivateChatByUser>();
                var privateChats = _backnammonContextDb.PrivateChats;

                var userPrivateChats = _backnammonContextDb.UserPrivateChats;
                var users = _backnammonContextDb.Users;

                if (userID > 0)
                {
                    var userPrivateChatsByUserQuery =
                        from upc2 in _backnammonContextDb.UserPrivateChats
                        where upc2.UserID == userID
                        select upc2.PrivateChatID;

                    //    from userPrivateChats
                    var privateChatByUsersQuery =
                        from pc in privateChats
                        join upc in userPrivateChats on pc.PrivateChatID equals upc.PrivateChatID
                        join u in users on upc.UserID equals u.UserID
                        where (userPrivateChatsByUserQuery.Contains(pc.PrivateChatID))
                        && u.UserID != userID
                        select new { pc, upc, u };

                    var list = privateChatByUsersQuery.Distinct().ToList();

                    foreach (var item in list)
                    {
                        PrivateChatByUser privateChatByUser = new PrivateChatByUser
                        {
                            PrivateChatID = item.pc.PrivateChatID,
                            GroupName = item.pc.GroupName,
                            TimeCreation = item.pc.TimeCreation,
                            UserID = userID,
                            OpponentID = item.u.UserID,
                            OpponentName = item.u.UserName
                        };
                        privateChatsPlus.Add(privateChatByUser);

                    }
                    return privateChatsPlus;
                }
                else return null;
            }



        }

        public async void UpdateUserOnlineTime(int userID)
        {
            if (userID>0)
            {
                let 
            }
        }

        #endregion

      
    }
}

