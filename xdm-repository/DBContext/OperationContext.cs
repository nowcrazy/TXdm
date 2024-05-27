using System;
using Microsoft.EntityFrameworkCore;
using xdm_model.Models;

namespace xdm_repository.DBContext
{
    public class OperationContext : DbContext
    {
        public OperationContext(DbContextOptions<OperationContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  ******   加上以下代码 即可   *******
            base.OnModelCreating(modelBuilder);
            //类与实际不同时可更改名字（与表相同）
            //modelBuilder.Entity<sys_user>().ToTable("sys_user");
        }
        public DbSet<sys_user> sys_user { get; set; }
    }
}

