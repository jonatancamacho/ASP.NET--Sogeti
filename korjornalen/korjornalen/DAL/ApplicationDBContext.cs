using korjornalen.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Linq;
using System.Web;

namespace korjornalen.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Report> Reports { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Makes sure our table names are in singular (Report) rather than plural (Reports)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Sets up the many-to-many relationship for our ApplicationUsers and Projects, using the table name "UserProjects".
            modelBuilder.Entity<ApplicationUser>().HasMany<Project>(u => u.AssociatedProjects).WithMany(p => p.AssociatedUsers).Map(up => { up.ToTable("UserProjects"); });

            modelBuilder.Entity<Project>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Project>().Property(p => p.ProjectNumber).IsRequired();
            modelBuilder.Entity<Project>().Ignore(p => p.FullInfo);

            // Sets up foreign keys.
            modelBuilder.Entity<Report>().HasRequired(r => r.AssociatedCar).WithMany().HasForeignKey(r => r.CarId);
            modelBuilder.Entity<Report>().HasRequired(r => r.AssociatedProject).WithMany().HasForeignKey(r => r.ProjectId);
            modelBuilder.Entity<Report>().HasRequired(r => r.AssociatedUser).WithMany().HasForeignKey(r => r.UserId);

            // Without these three lines, making changes to the modelBuilder gave me errors.
            // Referenced http://stackoverflow.com/questions/19913447/user-in-entity-type-mvc5-ef6
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
        }
    }
}