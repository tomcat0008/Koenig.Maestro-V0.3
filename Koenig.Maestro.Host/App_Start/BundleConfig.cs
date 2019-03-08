using System.Web;
using System.Web.Optimization;
using System.Web.Optimization.React;

namespace Koenig.Maestro.Host
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            BundleTable.EnableOptimizations = true;
            /*
            bundles.Add(new BabelBundle("~/Scripts/react").Include
                (
                    "~/Scripts/src/maestro.jsx"
                    
                ));
            
            bundles.Add(new ScriptBundle("~/Scripts/dist").Include(
                                "~/Scripts/jquery-3.3.1.js",
                                "~/Scripts/bootstrap.bundle.js",
                                "~/Scripts/react.development.js",
                                "~/Scripts/react-dom.development.js"
                ));
            */
            StyleBundle styleBundle = new StyleBundle("~/Content/dist");
            styleBundle.Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css");

            bundles.Add(styleBundle);

        }
    }
}