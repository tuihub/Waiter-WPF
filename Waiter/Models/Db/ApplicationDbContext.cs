﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Waiter.Models.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AppPackageSetting> AppPackageSettings { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string workDir = Directory.GetCurrentDirectory();
            var dbFilePath = Path.Combine(workDir, GlobalContext.SystemConfig.DataDirPath, GlobalContext.SystemConfig.SqliteDbPath);
            optionsBuilder.UseSqlite($"Data Source={dbFilePath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //configurationBuilder.Conventions.Remove(typeof(ForeignKeyIndexConvention));
        }
    }
}