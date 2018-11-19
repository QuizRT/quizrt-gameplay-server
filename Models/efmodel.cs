using Microsoft.EntityFrameworkCore;
using System;
namespace gameplay_back.Models
{
    public class efmodel: DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Questions> questions {get; set;}
        public DbSet<Options> options{get; set;}
        public DbSet<Game> game{get; set;}

        public efmodel(DbContextOptions<efmodel> options): base(options){
            this.Database.EnsureCreated(); 
        }

         protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Filename=./notes5.db");
        }
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlServer(@"SqlServer=localhost\SQLEXPRESS;Database=Users;Trusted_Connection=True;");
        // }    // Not Needed Anymore, Relevant Changes Done In Startup.cs

    // protected override void OnModelCreating(ModelBuilder modelBuilder){
    //         modelBuilder.Entity<>(Question).HasMany(n => n.checklist).WithOne().HasForeignKey(c => c.id);
    //         modelBuilder.Entity<Notes>().HasOne(n=>n.label).WithMany().HasForeignKey(l=> l.id);
    //     } 
    }
}