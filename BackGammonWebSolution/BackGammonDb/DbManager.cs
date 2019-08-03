using BackGammonDb.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackGammonDb
{
    public class DbManager
    {
        private BacknammonContextDb _backnammonContextDb;
       
        private MessageRepositories _messageRepositories;
        public MessageRepositories MessageRepositories
        {
            get
            {
                return _messageRepositories ?? new MessageRepositories(_backnammonContextDb);
            }
        }

        private UserRepositories _userRepositories;
        public UserRepositories UserRepositories
        {
            get
            {
                return _userRepositories ?? new UserRepositories(_backnammonContextDb);
            }
        }

        public DbManager()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BacknammonContextDb>();
            var connection = @"Data Source=SQL5041.site4now.net;Initial Catalog=DB_A4A6EE_BackgamonChatDb;User Id=DB_A4A6EE_BackgamonChatDb_admin;Password=1950t03b03;";

            optionsBuilder.UseSqlServer(connection);

            _backnammonContextDb = new BacknammonContextDb(optionsBuilder.Options);

           // _terminalContextDb.Database.EnsureCreated();

        }
    }
}
