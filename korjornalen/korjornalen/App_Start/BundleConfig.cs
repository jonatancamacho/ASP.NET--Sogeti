using System.Web;
using System.Web.Optimization;

namespace korjornalen
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css-shared").Include(
                      "~/Content/Styles/Shared/bootstrap.css",
                      "~/Content/Styles/Shared/site.css"));

            bundles.Add(new StyleBundle("~/Content/css-nonauth").Include(
                      "~/Content/Styles/NonAuth/NonAuth.css"));

            bundles.Add(new ScriptBundle("~/Content/script-log").Include(
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js",
                "~/Scripts/typeahead.bundle.js",
                "~/Content/Scripts/Log.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-datetimepicker").Include(
                "~/Content/Styles/Shared/bootstrap-datetimepicker.css"));

            bundles.Add(new ScriptBundle("~/Content/script-project").Include(
                "~/Scripts/typeahead.bundle.js",
                "~/Content/Scripts/Associate.js"));

            bundles.Add(new ScriptBundle("~/Content/script-adminlists").Include(
                "~/Content/Scripts/AdminLists.js"));

            bundles.Add(new StyleBundle("~/Content/css-typeahead").Include(
                "~/Content/Styles/Shared/typeahead.css"));

            bundles.Add(new ScriptBundle("~/Content/script-report").Include(
                "~/Content/Scripts/Report.js"));
        }
    }
}
