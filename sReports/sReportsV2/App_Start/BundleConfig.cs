using sReportsV2.Domain.Extensions;
using System;
using System.Web;
using System.Web.Optimization;

namespace sReportsV2
{
    public static class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles = Ensure.IsNotNull(bundles, nameof(bundles));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/jquery-ui-1.12.1.js",
                        "~/Scripts/umd/popper.min.js",
                        "~/Scripts/bootstrap.bundle.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/additional-methods.js",
                        "~/Scripts/sReports/sReports.js"));

            
            bundles.Add(new StyleBundle("~/Content/css")
                .Include(                      
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.min.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/form_style_1.css",
                      "~/Content/main.css",
                      "~/Content/PagedList.css")
                      );

            bundles.Add(new ScriptBundle("~/bundles/patient").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/patient.js",
                      "~/Scripts/sReports/telecomModal.js",
                      "~/Scripts/sReports/identifierModal.js"));

            bundles.Add(new ScriptBundle("~/bundles/patientGetAll").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/patientTable.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/organizationGetAll").Include(
                      "~/Scripts/sReports/organizationTable.js",
                      "~/Scripts/sReports/tableCommon.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/organization").Include(
                      "~/Scripts/sReports/identifierModal.js",
                      "~/Scripts/sReports/organization.js",
                      "~/Scripts/sReports/telecomModal.js"
                      ));
            bundles.Add(new ScriptBundle("~/bundles/user").Include(
                   "~/Scripts/sReports/tableCommon.js",
                   "~/Scripts/sReports/user.js",
                    "~/Scripts/sReports/changePasswordModal.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/userGetAll").Include(
                       "~/Scripts/sReports/userTable.js",
                      "~/Scripts/sReports/tableCommon.js",
                       "~/Scripts/sReports/changePasswordModal.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/GetAllByFormThesaurus").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/dataCaptureFormInstances.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/GetAllFormDefinitions").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/dataCaptureFormDefinitions.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/episodeOfCareGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/episodeOfCareTable.js"
                    ));


            bundles.Add(new ScriptBundle("~/bundles/encounterEocGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/encounterEoc.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/encounterGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/encounter.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/formGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/form.js",
                    "~/Scripts/sReports/generateModal.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/thesaurusGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/thesaurusEntry.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/formPartial").Include(
                    "~/Scripts/sReports/form.js",
                    "~/Scripts/sReports/imageMapResize.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/simulator").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/simulatorFormDistributionsTable.js"
                    ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
