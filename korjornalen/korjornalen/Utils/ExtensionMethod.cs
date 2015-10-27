using korjornalen.DAL;
using korjornalen.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace korjornalen.Utils
{
    public static class ExtensionMethod
    {

        public static bool IsNullOrEmpry(this string s)
        {
            return s == null || s.Length == 0;
        }

        /// <summary>
        /// Turns a boolean into a localized string for "Y" or "N".
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToReadable(this bool b)
        {
            return b ? Resources.AppString.YesShort : Resources.AppString.NoShort;
        }

        public static List<ApplicationUser> Filter(this IDbSet<ApplicationUser> users, string user = null, bool onlyActive = false)
        {
            List<ApplicationUser> returnList = new List<ApplicationUser>();
            if (!String.IsNullOrEmpty(user) && onlyActive)
            {
                returnList = users.Where(u => u.Email.Contains(user) && u.Active == onlyActive).ToList();
            }
            else if (onlyActive)
                returnList = users.Where(u => u.Active == true).ToList();
            else if (!String.IsNullOrEmpty(user))
                returnList = users.Where(u => u.Email.Contains(user)).ToList();
            else
                returnList = users.ToList();

            returnList.RemoveAll(u => u.Email == "root@root.com" && u.FirstName == "root" && u.LastName == "root"); // Do not display root user.

            return returnList;
        }

        public static List<Report> Sort(this List<Report> reports, string column, bool desc)
        {

            switch (column)
            {
                case "user":
                    return desc ? reports.OrderByDescending(r => r.AssociatedUser.FirstName).ToList() : reports.OrderBy(r => r.AssociatedUser.FirstName).ToList();
                case "project":
                    return desc ? reports.OrderByDescending(r => r.AssociatedProject.Name).ToList() : reports.OrderBy(r => r.AssociatedProject.Name).ToList();
                case "date":
                    return desc ? reports.OrderByDescending(r => r.Date).ToList() : reports.OrderBy(r => r.Date).ToList();
                default:
                    return reports.OrderBy(r => r.Date).ToList();
            }
        }

        public static List<Report> Filter(this List<Report> reports, string startDate, string endDate)
        {
            if (!startDate.IsNullOrEmpry() && !endDate.IsNullOrEmpry())
            {
                DateTime start = DateTime.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                reports = reports.Where(r => r.Date >= start && r.Date <= end).ToList();
            }
            return reports;
        }

        public static List<Report> Filter(this List<Report> reports, string startDate, string endDate, string user, string car, bool showUncompleted)
        {
            reports = reports.Filter(startDate, endDate);

            if (!showUncompleted)
            {
                reports = reports.Where(r => r.OdometerEnd != null).ToList();
            }

            if (!String.IsNullOrEmpty(user))
            {
                reports = reports.Where(r => r.AssociatedUser.Email == user).ToList();
            }

            if (!String.IsNullOrEmpty(car))
            {
                reports = reports.Where(r => r.AssociatedCar.RegistrationNumber == car).ToList();
            }
            
            return reports;
        }
    }
}