using korjornalen.Models;
using korjornalen.Utils;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace korjornalen.ViewModels
{
    public class ReportStartupViewModel
    {
        [Display(Name = "Car", ResourceType = typeof(AppString))]
        public IEnumerable<SelectListItem> AvailableCars { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        public int SelectedCarId { get; set; }

        [Display(Name = "Car", ResourceType = typeof(AppString))]
        public string SelectedCarRegistrationNumber { get; set; } // Used only when there's only one Car in database.

        [Display(Name = "Project", ResourceType = typeof(AppString))]
        public IEnumerable<SelectListItem> AvailableProjects { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        public int SelectedProjectId { get; set; }
        [Display(Name = "Project", ResourceType = typeof(AppString))]
        public string SelectedProjectName { get; set; } // Used when there is only one user associated project in the database.

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        //[OdometerStart("OdometerMin", ErrorMessageResourceName = "ErrorTooLowOdometerStart", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ReportOdometerStart", ResourceType = typeof(AppString))]
        public int OdometerStart { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ReportFromLocation", ResourceType = typeof(AppString))]
        public string FromLocation { get; set; }
    }

    public class ReportCompletionViewModel
    {
        [Display(Name = "StartDate", ResourceType = typeof(AppString))]
        public string Date { get; set; }

        [Display(Name = "Car", ResourceType = typeof(AppString))]
        public string CarRegistrationNumber { get; set; }

        [Display(Name = "Project", ResourceType = typeof(AppString))]
        public string ProjectInfo { get; set; }
        
        [Display(Name = "ReportOdometerStart", ResourceType = typeof(AppString))]
        public int OdometerStart { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ReportOdometerEnd", ResourceType = typeof(AppString))]
        public int OdometerEnd { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ReportToLocation", ResourceType = typeof(AppString))]
        public string ToLocation { get; set; }

        [Display(Name = "ReportPassengers", ResourceType = typeof(AppString))]
        public string Passengers { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ReportDebitable", ResourceType = typeof(AppString))]
        public Boolean Debitable { get; set; }

        [Display(Name = "ReportPurpose", ResourceType = typeof(AppString))]
        public string Purpose { get; set; }
    }

    public class ReportUncompletedViewModel
    {

    }
}