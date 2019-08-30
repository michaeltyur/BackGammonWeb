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
        private BacknammonContextDb _backnammonContextDb;
        public UserRepositories(BacknammonContextDb backnammonContextDb)
        {
            _backnammonContextDb = backnammonContextDb;
        }

        public void Dispose()
        {
            _backnammonContextDb.Dispose();
        }

        public User GetUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            User user;

            user = _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            return  user;
        }

        public User GetUserByName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            User user;


            user =  _backnammonContextDb.Users.FirstOrDefault(u => u.UserName == username);
            return user;
        }

        public bool AddUser(User user)
        {
            int result = 0;
            if (user != null)
            {

                 _backnammonContextDb.Users.AddAsync(user);
                result =  _backnammonContextDb.SaveChanges();
                return result>0;

            }
            else return false;
        }

        public bool UpdateUser(User user)
        {
            if (user != null)
            {

                //var oldUser = GetUserByName(user.UserName);

                //  oldUser.Password = user.Password;
                //  oldUser.FirstName = user.FirstName;
                //  oldUser.LastName = user.LastName;
                //oldUser.Messages = user.Messages;Add(user);
                try
                {
                    _backnammonContextDb.Users.Update(user);

                    var result =  _backnammonContextDb.SaveChanges();

                    return result > 0;
                }
                catch (Exception)
                {

                    return false;
                }



            }
            return false;
        }

        public IEnumerable<User> GetAllUsers()
        {
            List<User> listUsers = new List<User>();

            listUsers = _backnammonContextDb.Users.ToList();
            
            return   listUsers;
        }

        public async Task<bool> DeleteUser(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                User user = GetUser(userName, password);
                if (user != null)
                {

                    _backnammonContextDb.Users.Remove(user);
                    var res = _backnammonContextDb.SaveChanges();

                    return res > 0;

                }
                else return false;
            }
            else return false;
        }

        public bool SetUserOnLine(User user)
        {
            try
            {
                 user.IsOnline = true;
                 UpdateUser(user);
                 return  UpdateUser(user);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool SetUserOffLine(User user)
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
}

