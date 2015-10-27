using korjornalen.DAL;
using korjornalen.Models;
using korjornalen.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using korjornalen.Utils;
using Resources;

namespace korjornalen.Controllers
{
    /// <summary>
    /// The reporting flow is as follows (User perspective):
    /// 
    /// Users wants to start a new report (Drive) => Startup() - Only fields relevant to the start of a drive are entered at this point.
    /// Users has finished using the carpool car and starts the app again => Complete() - The rest of the fields are available.
    /// </summary>
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index()
        {
            ReportDAL dal = new ReportDAL(_db);

            string userId = User.Identity.GetUserId();
            if (dal.UserHasIncompleteReport(userId))
                return RedirectToAction("Complete", new { uncompletedExists = true, redirected=true });

            return RedirectToAction("Startup", "Report");
        }


        [HttpGet]
        public ActionResult Startup(bool reportCompleted = false, int? associatedProjectId = null)
        {
            Project associatedProject = null;

            if (reportCompleted)
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = AppString.ReportCompleted };
            if (associatedProjectId != null)
            {
                associatedProject = _db.Projects.Single(p => p.Id == associatedProjectId);
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = String.Format(AppString.ProjectAssociated, associatedProject.Name) };
            }
                

            string userId = User.Identity.GetUserId();

            ErrorLogger.LogString(User.IsInRole("Admin").ToString());

            // Makes sure that users finish previous reports first.
            ReportDAL rDb = new ReportDAL(_db);
            if (rDb.UserHasIncompleteReport(userId))
                return RedirectToAction("Index");

            ReportStartupViewModel model = new ReportStartupViewModel();

            try {
                PrepareProjectsForViewModel(model, associatedProject);
            } catch (Exception e) {
                // User is not logged in properly. Maybe a threading issue? (This code is a workaround)
                ErrorLogger.LogString("Could not log in. userId = " + User.Identity.GetUserId() );
                return RedirectToAction("Index", "Report"); 
            }
            
            // A user must have atleast one associated project to be able to report. Values are set in the PrepareProjectForStartup method call.
            if (model.AvailableProjects == null && model.SelectedProjectName == null)
                return RedirectToAction("Associate", "Project");

            
            PrepareCarsForViewModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Startup(ReportStartupViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReportDAL dal = new ReportDAL(_db);


                string currUserId = User.Identity.GetUserId(); // Calling GetUserId() within a Lambda-expression causes errors

                int? lastOdometerStatus = dal.GetLatestOdometerStatus(model.SelectedCarId);
                if ( lastOdometerStatus != null && model.OdometerStart < lastOdometerStatus ) {
                    ViewBag.OdometerError = String.Format(AppString.ErrorOdometerStart, lastOdometerStatus-1);
                    PrepareCarsForViewModel(model);
                    PrepareProjectsForViewModel(model, null);
                    return View(model);
                }

                Report report = new Report()
                {
                    Date = DateTime.Now,
                    OdometerStart = model.OdometerStart,
                    AssociatedCar = _db.Cars.Single(c => c.Id == model.SelectedCarId),
                    AssociatedProject = _db.Projects.Single(p => p.Id == model.SelectedProjectId),
                    AssociatedUser = _db.Users.Single(u => u.Id == currUserId),
                    FromLocation = model.FromLocation
                };

                // TODO: Perform checks on the received data to make sure that there isn't any user shenanigans going on. For example if a project id was passed that the user isn't associated with.

                _db.Reports.Add(report);
                _db.SaveChanges();

                return RedirectToAction("Complete", "Report", new { reportStarted = true });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Complete(bool uncompletedExists = false, bool reportStarted = false)
        {
            if (reportStarted)
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Success, Message = AppString.ReportStarted };
            if (uncompletedExists)
                ViewBag.AppMessage = new AppMessage() { Type = AppMessage.Warning, Message = AppString.ReportUncompletedExists };

            ReportDAL dal = new ReportDAL(_db);
            string userId = User.Identity.GetUserId();
            Report report = dal.GetLatestUncompletedReport(userId);
            string projectInfo = null;
            if (report.AssociatedProject.Name == "DefaultProject")
                projectInfo = "[ " + AppString.ProjectNotBound + " ]";
            else
                projectInfo = report.AssociatedProject.Name + " - " + report.AssociatedProject.ProjectNumber;

            ReportCompletionViewModel model = new ReportCompletionViewModel()
            {
                Date = report.Date.ToString("yyyy-MM-dd HH:mm"),
                CarRegistrationNumber = report.AssociatedCar.RegistrationNumber,
                ProjectInfo = projectInfo,
                OdometerStart = report.OdometerStart
            };


            return View(model);
        }
        
        [HttpPost]
        public ActionResult Complete(ReportCompletionViewModel model)
        {
            if (ModelState.IsValid)
            {
                ReportDAL dal = new ReportDAL(_db);

                string userId = User.Identity.GetUserId();
                Report report = dal.GetLatestUncompletedReport(userId);

                if (report.OdometerStart >= model.OdometerEnd)
                {
                    ViewBag.OdometerError = AppString.ErrorOdometerEnd;
                    return View(model);
                }
                    

                report.OdometerEnd = model.OdometerEnd;
                report.ToLocation = model.ToLocation;
                report.Passengers = model.Passengers;
                report.Debitable = model.Debitable;
                report.Purpose = model.Purpose;

                _db.Reports.Attach(report);
                _db.Entry(report).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Startup", "Report", new { reportCompleted = true });
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Uncompleted()
        {
            // TODO: Implement... duh.
            return View();
        }

        [HttpGet]
        public JsonResult LastRegisteredOdometerStatus(int carId)
        {
            ReportDAL dal = new ReportDAL(_db);

            int? oStatus = dal.GetLatestOdometerStatus(carId);

            if (oStatus != null)
                return Json(oStatus.ToString(), JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }


        /* Helper Methods below: */


        private void PrepareCarsForViewModel(ReportStartupViewModel model)
        {
            ICollection<Car> cars = _db.Cars.Where(c => c.Active).ToList();
            ReportDAL rDB = new ReportDAL(_db);
            // User doesn't need to select cars if there is only one available
            int? latestOdometerStatus = null;
            if (cars.Count == 1)
            {
                Car selectedCar = cars.FirstOrDefault();
                model.SelectedCarRegistrationNumber = selectedCar.RegistrationNumber;
                model.SelectedCarId = selectedCar.Id;

                latestOdometerStatus = rDB.GetLatestOdometerStatus(selectedCar.Id); ;
            }
            else
            {
                latestOdometerStatus = rDB.GetLatestOdometerStatus(cars.ToArray()[0].Id);
                model.AvailableCars = new SelectList(cars, "Id", "RegistrationNumber");
            }

            if (latestOdometerStatus != null)
                model.OdometerStart = (int)latestOdometerStatus;
        }

        private void PrepareProjectsForViewModel(ReportStartupViewModel model, Project associatedProject)
        {
            string userId = User.Identity.GetUserId();
            List<ProjectSelectListViewModel> selectList = new List<ProjectSelectListViewModel>();
            ProjectDAL pDb = new ProjectDAL(_db);
            List<Project> userProjects = pDb.GetUserProjects(userId).Where(p => p.Active).ToList();
            Project notProjectBound = _db.Projects.Single(p => p.Name == "DefaultProject");
            
            // Add default project first. It should always be first in the list.
            selectList.Add(new ProjectSelectListViewModel()
            {
                Id = notProjectBound.Id,
                FullInfo = "[ " + AppString.ProjectNotBound + " ]"
            });

            // Add the rest.
            foreach (Project p in userProjects)
            {
                selectList.Add(new ProjectSelectListViewModel()
                {
                    Id = p.Id,
                    FullInfo = p.FullInfo
                });
            }

            if (selectList.Count == 1)
            {
                ProjectSelectListViewModel foundProject = selectList.First();

                model.SelectedProjectName = foundProject.FullInfo;
                model.SelectedProjectId = foundProject.Id;
            }
            else
            {
                int id = -1;
                if (associatedProject != null)
                    id = associatedProject.Id;
                else
                    id = selectList.First().Id;
                model.AvailableProjects = new SelectList(selectList.ToArray(), "Id", "FullInfo", id);
            }
        }

    }
}