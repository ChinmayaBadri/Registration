using System.Web;
using System.Web.Optimization;

namespace Chinmaya.Registration.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.js",
                        "~/Scripts/Toastr/toastr.min.js",
                        "~/Scripts/Toastr/toastr.config.js",
                        "~/Scripts/jquery-{version}.js",
						"~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/Validate/jquery.validate.js",
                        "~/Scripts/Validate/jquery.validate.unobtrusive.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/Toastr/toastr.min.css",
                      "~/Content/site.css",
                      "~/Content/site.css",
					  "~/Content/DataTables/css/jquery.dataTables.css"));
        }
    }
}
