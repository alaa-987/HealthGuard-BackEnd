using HealthGuard.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HealthGurad.Repository.Data.Identity
{
    public class AppIDentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIDentityDbContext(DbContextOptions<AppIDentityDbContext> options) : base(options)
        {

        }
        public DbSet<AppNurse> AppNurses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Address>().ToTable("Addresses");
            builder.Entity<AppNurse>()
                   .HasMany(n => n.Appointments)
                   .WithOne(a => a.AppNurse)
                   .HasForeignKey(a => a.AppNurseId)
                   .IsRequired();
            builder.Entity<AppNurse>().ToTable("AppNurses");
           
            builder.Entity<Appointment>()
              .HasKey(a => a.Id);

            builder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.AppUserId)
                .IsRequired();


        }
    }
}
