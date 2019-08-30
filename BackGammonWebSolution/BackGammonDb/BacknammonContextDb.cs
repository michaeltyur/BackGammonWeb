using Microsoft.EntityFrameworkCore;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackGammonDb
{
    public class BacknammonContextDb : DbContext
    {
        public BacknammonContextDb() : base()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BacknammonContextDb>();

            var connection = @"Data Source=SQL5041.site4now.net;Initial Catalog=DB_A4A6EE_BackgamonChatDb;User Id=DB_A4A6EE_BackgamonChatDb_admin;Password=1950t03b03;";

            optionsBuilder.UseSqlServer(connection);

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = @"Data Source=SQL5041.site4now.net;Initial Catalog=DB_A4A6EE_BackgamonChatDb;User Id=DB_A4A6EE_BackgamonChatDb_admin;Password=1950t03b03;";

            optionsBuilder.UseSqlServer(connection);
        }
    }
}
