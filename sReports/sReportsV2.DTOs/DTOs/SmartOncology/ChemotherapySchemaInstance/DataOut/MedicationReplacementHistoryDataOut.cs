using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class MedicationReplacementHistoryDataOut
    {
        public MedicationInstancePreviewDataOut MedicationInstance { get; set; }
        public List<MedicationReplacementDataOut> MedicationReplacements { get; set; }
        public List<MedicationReplacementDataOut> GroupEntriesByDate()
        {
            List<MedicationReplacementDataOut> result = new List<MedicationReplacementDataOut>();
            foreach (var groupedByDate in MedicationReplacements.GroupBy(x => x.EntryDatetime))
            {
                var numberOfGroupedElements = groupedByDate.Count();
                bool isEvenNumberOfElements = numberOfGroupedElements % 2 == 0;
                int middle = numberOfGroupedElements / 2;
                var groupedElementsToAdd = new List<MedicationReplacementDataOut>();
                var groupedElements = groupedByDate.ToList();
                for(int i = 0; i < numberOfGroupedElements; i++)
                {
                    MedicationReplacementDataOut medicationReplacement = groupedElements[i];
                    groupedElementsToAdd.Add(new MedicationReplacementDataOut()
                    {
                        ReplaceMedication = medicationReplacement.ReplaceMedication,
                        ReplaceMedicationId = medicationReplacement.ReplaceMedicationId,
                        Creator = medicationReplacement.Creator,
                        EntryDatetime = medicationReplacement.EntryDatetime,
                        IsStartReplacement = i == 0
                    });
                }
                var middleElement = groupedElements[middle];
                if (isEvenNumberOfElements)
                {
                    var middleElementToAdd = new MedicationReplacementDataOut()
                    {
                        ReplaceMedication = middleElement.ReplaceMedication,
                        ReplaceWithMedication = middleElement.ReplaceWithMedication,
                        ReplaceWithMedicationId = middleElement.ReplaceWithMedicationId,
                        Creator = middleElement.Creator,
                        EntryDatetime = middleElement.EntryDatetime,
                    };
                    groupedElementsToAdd.Insert(middle, middleElementToAdd);
                }
                else
                {
                    var middleElementToAdd = groupedElementsToAdd[middle];
                    middleElementToAdd.ReplaceWithMedication = middleElement.ReplaceWithMedication;
                    middleElementToAdd.ReplaceWithMedicationId = middleElement.ReplaceWithMedicationId;
                }
                result.AddRange(groupedElementsToAdd);
            }
            return result;
        }
    }
}
