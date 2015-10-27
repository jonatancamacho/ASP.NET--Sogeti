namespace korjornalen.Migrations
{
    using korjornalen.DAL;
    using korjornalen.Models;
    using korjornalen.Utils;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        ApplicationDbContext _context;

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            _context = context;

            SeedRoles();
            SeedRootUser();
            SeedDefaultProject();
            
            if (ConfigurationManager.AppSettings["InDevelopment"].Equals("true")) // Only seed these things if we're in development.
            {
                SeedRegularUsers(); // Create Users
                SeedCars();
                SeedProjects();
                //SeedReports(); // TODO: Fix duplication problem. Having this line creates duplicates of Car and "SSAB Kramapp"-Project rows in db, that is it's foreign key objects. See GetReportSeeds()-method
            }

            context.SaveChanges();
        }

        private void SeedDefaultProject()
        {
            _context.Projects.AddOrUpdate(new Project()
            {
                ProjectNumber = "",
                Name = "DefaultProject"
            });
        }

        private void SeedRootUser()
        {
            CreateUser(new ApplicationUser() {
                UserName = "root@root.com",
                FirstName = "root",
                LastName = "root",
                Email = "root@root.com",
                EmailConfirmed = true
            },
            "Sogeti123#", null);
        }

        private void SeedRoles()
        {
            CreateRole("SuperAdmin");
            CreateRole("Admin");
        }


        private void SeedRegularUsers()
        {
            CreateUser(new ApplicationUser()
            {
                UserName = "arne.anka@sogeti.se",
                FirstName = "Arne",
                LastName = "Anka",
                Email = "arne.anka@sogeti.se",
                AssociatedProjects = GetProjectSeeds()
            },
            "Test123#", null);


            CreateUser(new ApplicationUser()
            {
                UserName = "test.testsson@sogeti.se",
                FirstName = "Test",
                LastName = "Testsson",
                Email = "test.testsson@sogeti.se"
                
            },
            "Test123#", null);

            CreateUser(new ApplicationUser()
            {
                UserName = "a.a@sogeti.se",
                FirstName = "Admin",
                LastName = "Adminsson",
                Email = "a.a@sogeti.se",
                PreferredLanguage = "sv"

            },
            "Test123#", null);
        }

        private void SeedCars() {
            foreach (Car car in GetCarSeeds())
                _context.Cars.AddOrUpdate(c => c.RegistrationNumber, car);
        }

        private void SeedProjects()
        {
            foreach (Project project in GetProjectSeeds())
                _context.Projects.AddOrUpdate(p => p.ProjectNumber, project);
        }

        private void SeedReports()
        {
            foreach (Report report in GetReportSeeds())
                _context.Reports.AddOrUpdate(r => r.Id, report);
                
        }

        /// <summary>
        ///     Creates a new user (if it doesn't already exist) for the specified ApplicationUser-object and password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        private void CreateUser(ApplicationUser user, string password, string role)
        {
            if (!(_context.Users.Any(u => u.UserName == user.UserName)))
            {
                var store = new UserStore<ApplicationUser>(_context);
                var manager = new UserManager<ApplicationUser>(store);
                
                manager.Create(user, password);
                
                if (!String.IsNullOrEmpty(role))
                {
                    manager.AddToRole(user.Id, role);
                }
                    
            }
        }

        private void CreateRole(string name)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            roleManager.Create(new IdentityRole(name));
        }

        private ICollection<Project> GetProjectSeeds()
        {
            List<Project> projects = new List<Project>();

            projects.Add(new Project() { Id = 1, ProjectNumber = "10005353", Name = "SSAB Kramapp" });
            projects.Add(new Project() { Id = 2, ProjectNumber = "10005553", Name = "Tfvk Vägväg" });
            projects.Add(new Project() { Id = 3, ProjectNumber = "10005363", Name = "Nånapp" });
            projects.Add(new Project() { Id = 4, ProjectNumber = "10009353", Name = "BestSystem" });

            return projects;
        }

        private ICollection<Car> GetCarSeeds()
        {
            List<Car> cars = new List<Car>();

            cars.Add(new Car() { Id = 1, RegistrationNumber = "REJ 453" });
            
            return cars;
        }

        private ICollection<Report> GetReportSeeds()
        {
            List<Report> reports = new List<Report>();
            
            ApplicationUser user = _context.Users.Single(u => u.UserName == "arne.anka@sogeti.se");

            reports.Add(new Report()
            {
                Id = 1,
                OdometerStart = 6835,
                OdometerEnd = 7996,
                Date = DateTime.ParseExact("2014-11-09", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                FromLocation = "Borlänge",
                ToLocation = "Jönköping",
                Passengers = "Arne Anka",
                AssociatedProject = GetProjectSeeds().Single(p => p.Name == "SSAB Kramapp"),
                AssociatedCar = GetCarSeeds().Single(c => c.Id == 1),
                AssociatedUser = user,
                Debitable = false,
                Purpose = "Domstolsverket"
            });

            return reports;
        }
    }
}
