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

        public async Task<User> GetUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            User user;

            user = await _backnammonContextDb.Users.FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
            return user;
        }

        public async Task<User> GetUserByName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            User user;


            user = await _backnammonContextDb.Users.FirstOrDefaultAsync(u => u.UserName == username);
            return user;
        }

        public async Task<int> AddUser(User user)
        {
            int result = 0;
            if (user != null)
            {

                await _backnammonContextDb.Users.AddAsync(user);
                result = await _backnammonContextDb.SaveChangesAsync();
                return result;

            }
            else return result;
        }

        public bool UpdateUser(User user)
        {
            if (user != null)
            {

                var oldUser = GetUserByName(user.UserName);

                //  oldUser.Password = user.Password;
                //  oldUser.FirstName = user.FirstName;
                //  oldUser.LastName = user.LastName;
                //oldUser.Messages = user.Messages;Add(user);
                _backnammonContextDb.Users.Update(user);

                _backnammonContextDb.SaveChanges();

                return true;

            }
            return false;
        }

        public List<User> GetAllUsers()
        {
            List<User> listUsers = new List<User>(); ;

            listUsers = _backnammonContextDb.Users.ToList();

            return listUsers;
        }

        public async Task<bool> DeleteUser(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                User user = await GetUser(userName, password);
                if (user != null)
                {

                    _backnammonContextDb.Users.Remove(user);
                    _backnammonContextDb.SaveChanges();

                    return true;

                }
                else return false;
            }
            else return false;
        }

        public async Task<bool> SetUserOnLine(User user)
        {
            try
            {
                user.IsOnline = 1;
                UpdateUser(user);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }

        }

        public async Task<bool> SetUserOffLine(User user)
        {
            try
            {
                user.IsOnline = 0;
                UpdateUser(user);
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }

        }

    }
}

