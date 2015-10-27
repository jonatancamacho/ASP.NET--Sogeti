using Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace korjornalen.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "ErrorRequired", ErrorMessageResourceType = typeof(AppString))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AppString))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(AppString))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+(@sogeti\.com|@sogeti\.se)$", ErrorMessage = "Registration limited to Sogeti.com and Sogeti.se")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "FirstName", ResourceType = typeof(AppString))]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName", ResourceType = typeof(AppString))]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "ErrorStringTooShort", ErrorMessageResourceType = typeof(AppString), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AppString))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "RegisterConfirmPassword", ResourceType = typeof(AppString))]
        [Compare("Password", ErrorMessageResourceName = "ErrorPasswordMismatch", ErrorMessageResourceType = typeof(AppString))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "ErrorStringTooShort", ErrorMessageResourceType = typeof(AppString), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(AppString))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(AppString))]
        [Compare("Password", ErrorMessageResourceName = "ErrorPasswordMismatch", ErrorMessageResourceType = typeof(AppString))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        public string Email { get; set; }
    }

    public class UserFrontEndSafe
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Email", ResourceType = typeof(AppString))]
        public string Email { get; set; }
        [Display(Name = "Admin", ResourceType=typeof(AppString))]
        public bool IsAdmin { get; set; }
        [Display(Name = "Active", ResourceType = typeof(AppString))]
        public bool IsActive { get; set; }
    }
}
