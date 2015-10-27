using korjornalen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.DAL
{
    public class ReportDAL
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public ReportDAL(ApplicationDbContext context)
        {
            _db = context;
        }

        public int? GetLatestOdometerStatus(int carId)
        {
            Report report = _db.Reports.Where(r => r.AssociatedCar.Id == carId)
               .OrderByDescending(r => r.Date)
               .FirstOrDefault();

            if (report == null) // Makes sure we actually found one, before trying to extract OdometerEnd
                return null;

            return report.OdometerEnd;
        }

        public bool UserHasIncompleteReport(string userId)
        {
            Report report = GetLatestUncompletedReport(userId);

            if (report == null)
                return false;

            return report.OdometerEnd == null;
        }

        public Report GetLatestUncompletedReport(string userId)
        {
            return _db.Reports
                   .Where(r => r.AssociatedUser.Id == userId && r.OdometerEnd == null)
                   .OrderByDescending(r => r.Date)
                   .FirstOrDefault();
        }
    }
}