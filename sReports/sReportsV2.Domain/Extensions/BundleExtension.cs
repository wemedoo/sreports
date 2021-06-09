using Hl7.Fhir.Model;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Extensions
{
    public static class BundleExtension
    {
        public static void SetDiagnosticReportIntoBundle(this Bundle patientBundle, FieldSet set)
        {
            set = Ensure.IsNotNull(set, nameof(set));

            patientBundle = Ensure.IsNotNull(patientBundle, nameof(patientBundle));

            DiagnosticReport report = new DiagnosticReport();
            int test = 0;
            foreach (Field field in set.Fields)
            {
                if (field.FhirType != null && field.FhirType.Equals(ResourceTypes.Observation))
                {
                    test++;
                    report.Result.Add(new ResourceReference(patientBundle.Entry[test].FullUrl));
                }
}

            if (set.FhirType != ResourceTypes.Patient && report.Result.Count != 0)
            {
                patientBundle.AddResourceEntry(report, report.ResourceType.ToString());
            }
        }

        public static void SetBundleElementsIntoBundle(this Bundle patientBundle, List<DomainResource> bundleElements)
        {
            bundleElements = Ensure.IsNotNull(bundleElements, nameof(bundleElements));

            foreach (var element in bundleElements)
            {
                if (element.ResourceType == ResourceType.Observation)
                {
                    Observation obs = element as Observation;
                    patientBundle.AddResourceEntry(element, obs.Identifier[0].System);
                }
                else
                {
                    patientBundle.AddResourceEntry(element, element.ResourceType.ToString());
                }
            }
        }

    }
}
