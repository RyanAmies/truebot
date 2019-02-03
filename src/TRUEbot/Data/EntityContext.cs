﻿using Microsoft.EntityFrameworkCore;
using TRUEbot.Data.Models;

namespace TRUEbot.Data
{
    public class EntityContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=truebot.db");
        }

        public virtual DbSet<Player> Players { get; set; }
    }
}