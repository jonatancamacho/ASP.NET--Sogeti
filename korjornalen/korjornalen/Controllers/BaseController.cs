using korjornalen.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using korjornalen.DAL;
using korjornalen.Models;

namespace korjornalen.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationDbContext _db = new ApplicationDbContext();
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            SelectCurrentCulture();

            return base.BeginExecuteCore(callback, state);
        }

        private void SelectCurrentCulture()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = _db.Users.SingleOrDefault(u => u.Id == userId);

            string cultureName = null;
            if (user != null)
                if ( !String.IsNullOrEmpty(user.PreferredLanguage) && CultureHelper.IsSupported(user.PreferredLanguage))
                    cultureName = user.PreferredLanguage;

            if (cultureName == null)
                cultureName = (Request.UserLanguages != null && Request.UserLanguages.Length > 0) ? Request.UserLanguages[0] : null;

            cultureName = CultureHelper.GetImplementedCulture(cultureName);

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }
    }
}