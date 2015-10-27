using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using korjornalen.DAL;
using korjornalen.Models;
using korjornalen.ViewModels;
using korjornalen.Utils;
using System.Data.Entity.Infrastructure;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;
using System.Globalization;

namespace korjornalen.Controllers
{
	public class LogController : BaseController
	{
		// GET: Log
		public ActionResult Index(string orderBy)
		{
			List<LogViewModel> LogList = new List<LogViewModel>();

			foreach (Report item in _db.Reports.OrderByDescending(r => r.Date))
			{
				LogViewModel LVM = new LogViewModel();
				LVM.Report = item;
				LogList.Add(LVM);
			}

			ViewData["LogList"] = LogList;

			return View();

		}

		[HttpGet]
		public ActionResult FilterAndSort(string sort, bool desc, string startDate, string endDate, string user, string car, bool showUncompleted)
		{
            List<Report> reports = _db.Reports.ToList().Filter(startDate, endDate, user, car, showUncompleted);
            reports = reports.Sort(sort, desc); // Sort() is an extension method, see Utils->ExtensionMethod->Sort

			List<LogViewModel> LogList = new List<LogViewModel>();

			foreach (Report item in reports)
			{
				LogViewModel LVM = new LogViewModel();
				LVM.Report = item;
				LogList.Add(LVM);
			}

			return PartialView("_LogTable", LogList);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Report report = _db.Reports.Find(id);
			if (report == null)
			{
				return HttpNotFound();              
			}
			return View(report);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Report report)
		{
			if (ModelState.IsValid)
			{
				_db.Reports.Attach(report);
				_db.Entry(report).State = EntityState.Modified;
				_db.SaveChanges();

				return RedirectToAction("Index");
			}
			return View();
		}
	   
		public ActionResult CreateNew()
		{

			return View();
		}

	    [HttpPost]
	    [ValidateAntiForgeryToken]
		public ActionResult CreateNew(Report report)
		{
			if (ModelState.IsValid) // NÅGOT GALET HÄR :)
			{
				_db.Reports.Attach(report); 
				_db.Entry(report).State = EntityState.Modified;
				_db.SaveChanges();

				return RedirectToAction("Index");
			
			}
			return View();
		}

		public ActionResult Delete(Report report)
		{
			_db.Reports.Attach(report);
			_db.Reports.Remove(report);
			_db.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpGet]
		public void ExportData(string startDate, string endDate)
		{
		    DateTime start = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
		    DateTime end = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

		    GridView gv = new GridView();
		    gv.DataSource = GetExportReadyList(start, end);
		    gv.DataBind();
			
		    Response.ClearContent();
		    Response.Buffer = true;
		    Response.AddHeader("content-disposition", "attachment; filename=Reports.xls");
            Response.ContentType = "application/vnd.ms-excel";
		    Response.Charset = "";
		    StringWriter sw = new StringWriter();
		    HtmlTextWriter htw = new HtmlTextWriter(sw);
		    gv.RenderControl(htw);
		    Response.Output.Write(sw.ToString());
		    Response.Flush();
		    Response.End();
		}

		public JsonResult Cars()
		{
			return Json(_db.Cars.Where(c => c.Active).ToList(), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Users()
		{
			List<UserFrontEndSafe> safeUsersList = new List<UserFrontEndSafe>();

            // Using a viewmodel because we don't want to send sensitive user data with JSon to front end scripts.
			foreach (ApplicationUser item in _db.Users.Where(u => u.Active).ToList())
			{
				safeUsersList.Add(new UserFrontEndSafe() {
					Email = item.Email,
					Name = item.FirstName + " " + item.LastName,
					Id = item.Id
				});
			}

			return Json(safeUsersList, JsonRequestBehavior.AllowGet);
		}

		public List<ExportExcelViewModel> GetExportReadyList(DateTime start, DateTime end)
		{
			List<ExportExcelViewModel> reports = new List<ExportExcelViewModel>();

            List<Report> reportList = _db.Reports.Where(r => r.OdometerEnd != null && r.Date >= start && r.Date <= end).ToList();

			foreach (Report report in reportList)
			{
				reports.Add(new ExportExcelViewModel()
				{
					OdometerStart = report.OdometerStart,
					OdometerEnd = (int)report.OdometerEnd,
					Date = report.Date.ToString("yyyy-MM-dd"),
					From = report.FromLocation,
					To = report.ToLocation,
					Travelers = ExtractTravelers(report),
					SubProjectNumber = report.AssociatedProject.ProjectNumber,
					Debitable = report.Debitable.ToReadable(),
					NumberOfKm = (report.OdometerEnd - report.OdometerStart) / 10,
					Purpose = report.Purpose
				});
			}
			return reports;
		}


		/// <summary>
		/// Used to get a commaseparated list of travelers.
		/// </summary>
		/// <param name="report"></param>
		/// <returns></returns>
		private string ExtractTravelers(Report report)
		{
			string travelers = report.AssociatedUser.FirstName + " " + report.AssociatedUser.LastName;

			if (!(report.Passengers == null || report.Passengers == ""))
			{
				travelers += ", " + report.Passengers; 
			}

			return travelers;
		}
	}
}
