using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace korjornalen.ViewModels
{
    public class AssociateViewModel
    {
        [Display(Name = "SearchProject", ResourceType=typeof(AppString))]
        public string ProjectInfo { get; set; }
        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        public int SelectedProjectId { get; set; }
    }

    public class SuggestViewModel
    {
        public string ProjectInfo { get; set; }
        public int ProjectId { get; set; }
    }

    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [RegularExpression("^((?!DefaultProject).)*$", ErrorMessageResourceName = "ProjectNameReserved", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "Name", ResourceType = typeof(AppString))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "ProjectNumber", ResourceType = typeof(AppString))]
        public string ProjectNumber { get; set; }

        [Display(Name = "Active", ResourceType = typeof(AppString))]
        public bool IsActive { get; set; }
    }

    public class ProjectSelectListViewModel
    {
        public int Id { get; set; }
        public string FullInfo { get; set; }
    }
}