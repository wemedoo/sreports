using sReportsV2.Common.Extensions;
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
                        "~/Scripts/sReports/date-helper.js",
                        "~/Scripts/sReports/sReports.js"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include(                      
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.min.css",
                      "~/Content/themes/base/jquery-ui.css",
                      "~/Content/form_style_1.css",
                      "~/Content/main.css",
                      "~/Content/PagedList.css",
                      "~/Content/newMain.css")
                      );

            bundles.Add(new ScriptBundle("~/bundles/patient").Include(
                    "~/Scripts/sReports/patientTree.js",
                    "~/Scripts/sReports/formInstanceCommon.js",
                    "~/Scripts/sReports/datetimepicker.js",
                    "~/Scripts/sReports/common/custom-modal.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/patientInfo").Include(
                      "~/Scripts/sReports/patient.js",
                      "~/Scripts/sReports/datetimepicker.js",
                      "~/Scripts/sReports/telecomModal.js",
                      "~/Scripts/sReports/addressModal.js",
                      "~/Scripts/sReports/identifierModal.js"));

            bundles.Add(new ScriptBundle("~/bundles/patientGetAll").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/patientTable.js",
                      "~/Scripts/sReports/datetimepicker.js",
                      "~/Scripts/sReports/common/patientAutoComplete.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/organizationGetAll").Include(
                      "~/Scripts/sReports/organization/organizationTable.js",
                      "~/Scripts/sReports/tableCommon.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/organization").Include(
                      "~/Scripts/sReports/identifierModal.js",
                      "~/Scripts/sReports/organization/organization.js",
                      "~/Scripts/sReports/telecomModal.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/user").Include(
                   "~/Scripts/sReports/userAdministration.js",
                    "~/Scripts/sReports/changePasswordModal.js",
                    "~/Scripts/sReports/datetimepicker.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/userGetAll").Include(
                       "~/Scripts/sReports/userAdministrationTable.js",
                      "~/Scripts/sReports/tableCommon.js",
                       "~/Scripts/sReports/changePasswordModal.js",
                       "~/Scripts/sReports/user.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/administration").Include(
                   "~/Scripts/sReports/administration/administrationTable.js",
                   "~/Scripts/sReports/administration/administration.js",
                    "~/Scripts/sReports/tableCommon.js"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/administrationPredefined").Include(
                  "~/Scripts/sReports/administration/administrationPredefinedTable.js",
                  "~/Scripts/sReports/administration/administration.js",
                   "~/Scripts/sReports/tableCommon.js"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/GetAllByFormThesaurus").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/dataCaptureFormInstances.js",
                      "~/Scripts/sReports/common/userAutoComplete.js",
                      "~/Scripts/sReports/common/patientAutoComplete.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/GetAllFormDefinitions").Include(
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/dataCaptureFormDefinitions.js",
                      "~/Scripts/sReports/datetimepicker.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/encounterGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/encounter.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/formGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/designer/formDefinitionTable.js",
                    "~/Scripts/sReports/generateModal.js",
                    "~/Scripts/sReports/datetimepicker.js"

                    ));

            bundles.Add(new ScriptBundle("~/bundles/formEngine").Include(
                "~/Scripts/sReports/formEngine.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/thesaurusGetAll").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/thesaurusEntry.js",
                    "~/Scripts/sReports/thesaurusTree.js",
                    "~/Scripts/sReports/datetimepicker.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/reviewThesaurus").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/thesaurusEntry.js",
                    "~/Scripts/sReports/thesaurusTree.js",
                    "~/Scripts/sReports/reviewTree.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/formPartial").Include(
                    "~/Scripts/sReports/dynamicForm.js",
                    "~/Scripts/sReports/formInstanceCommon.js",
                    "~/Scripts/sReports/imageMapResize.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/simulator").Include(
                    "~/Scripts/sReports/tableCommon.js",
                    "~/Scripts/sReports/simulatorFormDistributionsTable.js"
                    ));

            bundles.Add(new ScriptBundle("~/bundles/digitalGuideline").Include(
                "~/Scripts/sReports/schema-json.js",
                "~/Scripts/sReports/digitalGuideline/digital-guideline.style.js",
                "~/Scripts/sReports/digitalGuideline/digital-guideline.js",
                "~/Scripts/sReports/digitalGuideline/json-editor.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/digitalGuidelineTable").Include(
                      "~/Scripts/sReports/digitalGuideline/digitalGuidelineTable.js",
                      "~/Scripts/sReports/tableCommon.js",
                      "~/Scripts/sReports/datetimepicker.js"

                      ));

            bundles.Add(new ScriptBundle("~/bundles/roleAdministration").Include(
                       "~/Scripts/sReports/roleAdministration.js",
                      "~/Scripts/sReports/tableCommon.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/dataCaptureChart").Include(
                       "~/Scripts/sReports/DataCaptureChart/chartDataCapture.js",
                       "~/Scripts/sReports/datetimepicker.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/dragAndDrop").Include(
                        "~/Scripts/sReports/image-map/jquery.imagemaps.js",
                        "~/Scripts/sReports/designer/nestable/jquery.nestable.js",
                        "~/Scripts/sReports/designer/nestable/jquery.nestable.customization.js",
                        "~/Scripts/sReports/designer/designer.js",
                        "~/Scripts/sReports/designer/validation.js",
                        "~/Scripts/sReports/designer/designer-items/filter.thesaurus.js",
                        "~/Scripts/sReports/tableCommon.js",
                        "~/Scripts/sReports/designer/designer-items/formGeneralInfo.js",
                        "~/Scripts/sReports/designer/designer-items/chapterGeneralInfo.js",
                        "~/Scripts/sReports/designer/designer-items/pageGeneralInfo.js",
                        "~/Scripts/sReports/jquery.line.js",
                        "~/Scripts/sReports/designer/designer-items/fieldsetGeneralInfo.js",
                        "~/Scripts/sReports/designer/designer-items/fields/fieldGeneralInfo.js",
                        "~/Scripts/sReports/designer/designer-items/fields/selectableField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/calculativeField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/checkboxField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/customFieldButton.js",
                        "~/Scripts/sReports/designer/designer-items/fields/numberField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/radioField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/regexField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/textField.js",
                        "~/Scripts/sReports/designer/designer-items/fields/selectField.js",
                        "~/Scripts/sReports/designer/designer-items/fieldValue.js",
                        "~/Scripts/sReports/formJson.js",
                        "~/Scripts/sReports/schema-json.js",
                        "~/Scripts/sReports/designer/consensus.js",
                        "~/Scripts/sReports/common/clinicalDomainAutocomplete.js",
                        "~/Scripts/sReports/designer/tagUserAutocomplete.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
