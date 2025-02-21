using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InsuranceCompany.Domain.Entities;

namespace InsuranceCompany.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.User)
                .WithMany(u => u.Insurances)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<Insurance>(entity =>
            {
                entity.Property(i => i.TypeOfInsurance)
                    .HasConversion<string>()
                    .HasColumnType("VARCHAR(50)");

                entity.Property(i => i.Status)
                    .HasConversion<string>()
                    .HasColumnType("VARCHAR(50)");

                entity.Property(i => i.MethodOfInsurance)
                    .HasConversion<string>()
                    .HasColumnType("VARCHAR(50)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .HasColumnType("VARCHAR(40)")
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasColumnType("VARCHAR(40)")
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();

                entity.Property(e => e.Password)
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired();
            });
        }


        public DbSet<User>? Users { get; set; }

        public DbSet<Insurance>? insurances { get; set; }
    }
}
