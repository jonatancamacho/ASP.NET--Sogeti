using korjornalen.DAL;
using korjornalen.Models;
using korjornalen.Utils;
using korjornalen.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace korjornalen.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult General()
        {
            Project defaultProject = _db.Projects.Single(p => p.Name == "DefaultProject");

            AdminGeneralSettingsViewModel model = new AdminGeneralSettingsViewModel()
            {
                ProjectNumber = defaultProject.ProjectNumber
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult General(AdminGeneralSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project defaultProject = _db.Projects.Single(p => p.Name == "DefaultProject");

                defaultProject.ProjectNumber = model.ProjectNumber;

                _db.Projects.Attach(defaultProject);
                _db.Entry(defaultProject).State = EntityState.Modified;
                _db.SaveChanges();

                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = AppString.ChangesSaved };
            }
            else
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Error, Message = AppString.ErrorFormNotFilledCorrectly};

            return View(model);
        }
        
        [HttpGet]
        public ActionResult Users()
        {   
            List<UserFrontEndSafe> users = new List<UserFrontEndSafe>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            
            foreach (ApplicationUser user in _db.Users.Filter(null, false))
            {
                bool isAdmin = userManager.IsInRole(user.Id, "Admin");

                users.Add(new UserFrontEndSafe()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.FirstName + " " + user.LastName,
                    IsAdmin = isAdmin,
                    IsActive = user.Active
                });
            }

            return View(users);
        }

        [HttpGet]
        public ActionResult Projects()
        {
            return View(_db.Projects.Where(p => p.Name != "DefaultProject").ToList());
        }

        [HttpGet]
        public ActionResult Cars()
        {

            return View(_db.Cars.ToList());
        }

        [HttpPost]
        public ActionResult CreateCar(CarViewModel car)
        {
            if (ModelState.IsValid)
            {
                if (_db.Cars.Any(c => c.RegistrationNumber == car.RegistrationNumber))
                    ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Error, Message = String.Format(AppString.CarAlreadyExists, car.RegistrationNumber )};
                else
                {
                    _db.Cars.Add(new Car() { RegistrationNumber = car.RegistrationNumber });
                    _db.SaveChanges();
                    ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = AppString.CarCreated };
                }
            }
            else
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Error, Message = AppString.ErrorFormNotFilledCorrectly };

            return View("Cars", _db.Cars.ToList());
        }

        [HttpPost]
        public JsonResult ChangeStatus(string type, string column, string id, bool status)
        {
            switch (type)
            {
                case "project":
                    return ChangeProjectActiveStatus(column, Int32.Parse(id), status);
                case "car":
                    return ChangeCarActiveStatus(column, Int32.Parse(id), status);
                case "user":
                    return ChangeUserStatus(column, id, status);
                default:
                    return Json(new AppMessage() { Type = AppMessage.Error, Message = AppString.BadJsonRequest });
            }
        }

        private JsonResult ChangeProjectActiveStatus(string column, int projectId, bool active)
        {
            Project project = _db.Projects.Single(p => p.Id == projectId);
            project.Active = active;

            _db.Projects.Attach(project);
            _db.Entry(project).State = EntityState.Modified;
            int rows = _db.SaveChanges();

            if (rows > 0)
                return Json(new AppMessage() { Type = AppMessage.Success, Message = String.Format(AppString.ProjectIsNow, project.Name, active ? AppString.Active.ToLower() : AppString.Inactive.ToLower()) });
            else
                return Json(new AppMessage() { Type = AppMessage.Error, Message = String.Format(AppString.UpdateForTypeFailed, AppString.Project) });
        }

        private JsonResult ChangeCarActiveStatus(string column, int carId, bool active)
        {
            Car car = _db.Cars.Single(c => c.Id == carId);
            car.Active = active;

            _db.Cars.Attach(car);
            _db.Entry(car).State = EntityState.Modified;
            int rows = _db.SaveChanges();

            if (rows > 0)
                return Json(new AppMessage() { Type = AppMessage.Success, Message = String.Format(AppString.CarIsNow, car.RegistrationNumber, active ? AppString.Active.ToLower() : AppString.Inactive.ToLower()) });
            else
                return Json(new AppMessage() { Type = AppMessage.Error, Message = String.Format(AppString.UpdateForTypeFailed, AppString.Car) });
        }

        private JsonResult ChangeUserStatus(string column, string userId, bool status) {
            switch (column)
            {
                case "admin":
                    //if (User.IsInRole("SuperAdmin")) // Only root can add and revoke admins-privileges.
                        return ChangeUserAdminStatus(userId, status);
                    break;
                case "active":
                    return ChangeUserActiveStatus(userId, status);
            }
            return Json(new AppMessage() { Type = AppMessage.Error, Message = AppString.BadJsonRequest });
        }

        private JsonResult ChangeUserActiveStatus(string userId, bool active)
        {
            ApplicationUser user = _db.Users.Single(u => u.Id == userId);
            user.Active = active;

            _db.Users.Attach(user);
            _db.Entry(user).State = EntityState.Modified;
            int rows = _db.SaveChanges();

            if (rows > 0)
                return Json(new AppMessage() { Type = AppMessage.Success, Message = String.Format(AppString.UserIsNow, user.Email, active ? AppString.Active.ToLower() : AppString.Inactive.ToLower()) });
            else
                return Json(new AppMessage() { Type = AppMessage.Error, Message = String.Format(AppString.UpdateForTypeFailed, AppString.User) });
        }

        private JsonResult ChangeUserAdminStatus(string userId, bool admin)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            ApplicationUser user = userManager.FindById(userId);
            IdentityResult result = null;
            
            if (admin)
                result = userManager.AddToRole(userId, "Admin");
            else
                result = userManager.RemoveFromRoles(userId, "Admin");

            string errors = "";

            foreach (string er in result.Errors)
            {
                errors += er + " ";
            }

            if (result.Succeeded)
                return Json(new AppMessage() { Type = AppMessage.Success, Message = String.Format(AppString.UserIsNow, user.Email, admin ? AppString.PartialIsNowAdmin : AppString.PartialIsNoLongerAdmin) });
            else
                return Json(new AppMessage() { Type = AppMessage.Error, Message = String.Format(AppString.UpdateForTypeFailed, AppString.User)  + " " + errors  });
        }
    }
}