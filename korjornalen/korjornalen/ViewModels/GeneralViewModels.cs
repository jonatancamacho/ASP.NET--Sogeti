using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace korjornalen.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "RegistrationNumber", ResourceType = typeof(AppString))]
        public string RegistrationNumber { get; set; }
    }

    public class AppMessage
    {
        public static readonly string Success = "success";
        public static readonly string Error = "error";
        public static readonly string Warning = "warning";
        public static readonly string Info = "info";

        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class AdminGeneralSettingsViewModel
    {
        [Display(Name = "DefaultProjectLabel", ResourceType = typeof(AppString))]
        public string ProjectNumber { get; set; }
    }
}