using System.Web;
using System.Web.Optimization;

namespace CoeusProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/kendo").Include(
                        "~/Content/kendo/web/kendo.common.min.css",
                        "~/Content/kendo.coeus.css"));

            bundles.Add(new StyleBundle("~/Content/site")
                .Include("~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/kendo/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                "~/Scripts/kendo/kendo.all.min.js",
                "~/Scripts/kendo/kendo.aspnetmvc.min.js",
                "~/Scripts/kendo/kendo.timezones.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/site")
                .Include("~/Scripts/jsExtensions.js"));
        }
    }
}