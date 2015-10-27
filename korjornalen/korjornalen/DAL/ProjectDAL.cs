using korjornalen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.DAL
{
    public class ProjectDAL
    {
        ApplicationDbContext _db;

        public ProjectDAL(ApplicationDbContext context)
        {
            _db = context;
        }

        public ICollection<Project> GetUserProjects(string userId) {
            ApplicationUser user = _db.Users.SingleOrDefault(u => u.Id == userId);

            return user.AssociatedProjects;
        }
    }
}