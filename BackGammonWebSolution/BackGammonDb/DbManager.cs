using BackGammonDb.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackGammonDb
{
    public class DbManager
    {
        private readonly BacknammonContextDb _backnammonContextDb;
        private readonly object locker = new object { };

        private readonly MessageRepositories _messageRepositories; 
         public MessageRepositories  MessageRepositories=> _messageRepositories ?? new MessageRepositories(_backnammonContextDb, locker);
           
        private readonly UserRepositories _userRepositories;
        public UserRepositories UserRepositories => _userRepositories ?? new UserRepositories(_backnammonContextDb, locker);

        public DbManager()
        {
            _backnammonContextDb = new BacknammonContextDb ();
        }
    }
}
