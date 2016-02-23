using System.Web.Optimization;

namespace Atomia.Store.Themes.Default
{
    public class BundleConfig
    {
        public static string DEFAULT_SCRIPTS_BUNDLE = "~/bundles/scripts";
        public static string DEFAULT_STYLES_BUNDLE = "~/Themes/Default/Content/css/atomia_2/style";

        public static void RegisterBundles(BundleCollection bundles)
        {
            var scriptBundle = new ScriptBundle(DEFAULT_SCRIPTS_BUNDLE)
                .Include(
                    "~/Themes/Default/Scripts/jquery-{version}.js",
                    "~/Themes/Default/Scripts/jquery.validate.js",
                    "~/Themes/Default/Scripts/MicrosoftMvcJQueryValidation.js",
                    "~/Themes/Default/Scripts/knockout-{version}.js",
                    "~/Themes/Default/Scripts/underscore.js",
                    "~/Themes/Default/Scripts/amplify.js")
                .Include(
                    "~/Themes/Default/Scripts/atomia/AtomiaValidation.js",
                    "~/Themes/Default/Scripts/atomia/atomia.utils.js",
                    "~/Themes/Default/Scripts/atomia/atomia.utils.eventhandlers.js",
                    "~/Themes/Default/Scripts/atomia/atomia.api.definitions.js",
                    "~/Themes/Default/Scripts/atomia/atomia.api.cart.js",
                    "~/Themes/Default/Scripts/atomia/atomia.api.checkout.js",
                    "~/Themes/Default/Scripts/atomia/atomia.api.domains.js",
                    "~/Themes/Default/Scripts/atomia/atomia.ko.submitvalid-binding.js",
                    "~/Themes/Default/Scripts/atomia/atomia.ko.slidevisible-binding.js",
                    "~/Themes/Default/Scripts/atomia/atomia.ko.vpsslider-binding.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.modalmixin.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.productmixin.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.notification.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.languageselector.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.cart.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.campaign.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.progress.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.domainconnection.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.domains.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.domainregistration.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.domainmove.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.productlisting.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.account.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.whois.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.paymentselector.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.paywithinvoice.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.paymentprofile.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.vatvalidation.js",
                    "~/Themes/Default/Scripts/atomia/atomia.viewmodels.vpscalculator.js");

            var styleBundle = new StyleBundle(DEFAULT_STYLES_BUNDLE)
                .Include(
                    "~/Themes/Default/Content/css/atomia_2/style.css");

            bundles.Add(scriptBundle);
            bundles.Add(styleBundle);
        }
    }
}
