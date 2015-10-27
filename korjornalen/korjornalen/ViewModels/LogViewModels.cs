using korjornalen.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace korjornalen.ViewModels
{
    public class LogViewModel
    {
        public Report Report { get; set; }
        public double? NumberOfKm { get { 
            return (Report.OdometerEnd - Report.OdometerStart) / 10;
        }}
        public string allPassengers { get { return Report.AssociatedUser.FirstName + " " + Report.AssociatedUser.LastName + ", " + Report.Passengers; } }
        public string dateFormated { get { return Report.Date.ToString("yyyy-MM-dd"); } }
    }


    public class ExportExcelViewModel
    {
        [Display(Name = "Odometer (start)")]
        public int OdometerStart { get; set; }

        [Display(Name = "Odometer (end)")]
        public int OdometerEnd { get; set; }

        [Display(Name = "Date")]
        public string Date { get; set; }

        [Display(Name = "From")]
        public string From { get; set; }

        [Display(Name = "To")]
        public string To { get; set; }

        [Display(Name = "Travelers")]
        public string Travelers { get; set; }

        [Display(Name = "Sub project no")]
        public string SubProjectNumber { get; set; }

        [Display(Name = "Deb Y/N")]
        public string Debitable { get; set; }

        [Display(Name = "No of km")]
        public double? NumberOfKm { get; set; }

        [Display(Name = "Purpose/Company")]
        public string Purpose { get; set; }
    }
}