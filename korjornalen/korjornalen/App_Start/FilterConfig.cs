using System.Web;
using System.Web.Mvc;

namespace korjornalen
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Everything in the app requires login. Hence we do a global AuthorizeFilter
            filters.Add(new AuthorizeAttribute());
        }
    }
}
