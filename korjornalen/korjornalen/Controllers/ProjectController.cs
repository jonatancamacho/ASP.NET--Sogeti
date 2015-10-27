using korjornalen.Models;
using korjornalen.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using korjornalen.ViewModels;
using System.Data.Entity;
using System.Threading.Tasks;
using Resources;

namespace korjornalen.Controllers
{

    public class ProjectController : BaseController
    {
        // GET: Project
        [HttpGet]
        public ActionResult Associate(string returnUrl, int? projectCreatedId = null )
        {
            if (projectCreatedId != null)
            {
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = AppString.ProjectCreated };
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Associate(AssociateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                Project project = _db.Projects.Single(p => p.Id == model.SelectedProjectId);
                // Not using UserManager because both objects need to be referenced from the same instance of db-context...
                // ... for Many-to-Many-relationhips to be inserted properly.
                ApplicationUser user = _db.Users.Single(u => u.Id == userId);

                project.AssociatedUsers.Add(user);

                _db.Projects.Attach(project);
                _db.Entry(project).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction("Startup", "Report", new { associatedProjectId = project.Id });
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectViewModel project, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (project.Name == "DefaultProject")
                {
                    ViewBag.ErrorMessage = AppString.ProjectNameReserved;
                    return View(project);
                }

                Project existingProject = _db.Projects.SingleOrDefault(p => p.ProjectNumber == project.ProjectNumber);
                if (existingProject != null) {
                    ViewBag.ExistingProject = new AssociateViewModel()
                    {
                        SelectedProjectId = existingProject.Id
                    };

                    ViewBag.ErrorMessage = String.Format(AppString.ProjectAlreadyExists, existingProject.Name);
                    return View(project);
                }

                Project newProject = new Project()
                {
                    ProjectNumber = project.ProjectNumber,
                    Name = project.Name
                };

                _db.Projects.Attach(newProject);
                _db.Entry(newProject).State = EntityState.Added;
                _db.SaveChanges();
                return RedirectToAction("Associate", "Project", new { projectCreatedId = newProject.Id });
            }
            return View(project);
        }

        public ActionResult Suggest(string q)
        {
            List<Project> foundProjects = _db.Projects.Where(p => p.Active == true && p.Name != "DefaultProject" && (p.Name.Contains(q) || p.ProjectNumber.Contains(q))).ToList();
            
            // Get user projects
            string userId = User.Identity.GetUserId();
            List<Project> userProjects = _db.Users.Single(u => u.Id == userId).AssociatedProjects.ToList();
            
            // Remove all projects that the user is already associated with.
            foundProjects = foundProjects.Except(userProjects).ToList();

            List<SuggestViewModel> suggestions = new List<SuggestViewModel>();

            foreach (Project item in foundProjects)
            {
                suggestions.Add(new SuggestViewModel() {
                    ProjectInfo = item.Name + " - " + item.ProjectNumber,
                    ProjectId = item.Id
                }); 
            }

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
    }
}