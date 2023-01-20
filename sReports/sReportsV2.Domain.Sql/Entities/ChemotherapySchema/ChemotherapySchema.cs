using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class ChemotherapySchema : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ChemotherapySchemaId")]
        public int ChemotherapySchemaId { get; set; }
        public string Name { get; set; }
        public List<Indication> Indications { get; set; } = new List<Indication>();
        public List<LiteratureReference> LiteratureReferences { get; set; } = new List<LiteratureReference>();
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public int CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public sReportsV2.Domain.Sql.Entities.User.User Creator { get; set; }
        public int LengthOfCycle { get; set; }
        public int NumOfCycles { get; set; }
        public bool AreCoursesLimited { get; set; }
        public int NumOfLimitedCourses { get; set; }

        public void Copy(ChemotherapySchema chemotherapySchema)
        {
            CopyName(chemotherapySchema.Name);
            CopyGeneralProperties(chemotherapySchema);
            CopyRowVersion(chemotherapySchema.RowVersion);
            //CopyIndications(chemotherapySchema.Indications);
            //CopyLiteratureReference(chemotherapySchema.LiteratureReferences);
        }

        public void CopyName(string name)
        {
            this.Name = name;
        }

        public void CopyRowVersion(byte[] rowVersion)
        {
            this.RowVersion = rowVersion;
        }

        public void CopyGeneralProperties(ChemotherapySchema chemotherapySchema)
        {
            this.LengthOfCycle = chemotherapySchema.LengthOfCycle;
            this.NumOfCycles = chemotherapySchema.NumOfCycles;
            this.AreCoursesLimited = chemotherapySchema.AreCoursesLimited;
            this.NumOfLimitedCourses = chemotherapySchema.NumOfLimitedCourses;
        }

        public void CopyIndications(List<Indication> indications)
        {
            if (indications != null)
            {
                DeleteExistingRemovedIndication(indications);
                AddNewOrUpdateOldIndication(indications);
            }
        }

        private void DeleteExistingRemovedIndication(List<Indication> upcomingIndications)
        {
            foreach (var indication in Indications)
            {
                var remainingIndication = upcomingIndications.Any(x => x.IndicationId == indication.IndicationId);
                if (!remainingIndication)
                {
                    indication.IsDeleted = true;
                }
            }
        }

        private void AddNewOrUpdateOldIndication(List<Indication> upcomingIndications)
        {
            foreach (var indication in upcomingIndications)
            {
                if (indication.IndicationId == 0)
                {
                    Indications.Add(indication);
                }
                else
                {
                    var dbIndication = Indications.FirstOrDefault(x => x.IndicationId == indication.IndicationId);
                    if (dbIndication != null)
                    {
                        dbIndication.Copy(indication);
                    }
                }
            }
        }

        private void CopyLiteratureReference(List<LiteratureReference> literatureReferences)
        {
            if (literatureReferences != null)
            {
                DeleteExistingRemovedReference(literatureReferences);
                AddNewOrUpdateOldReference(literatureReferences);
            }
        }

        private void DeleteExistingRemovedReference(List<LiteratureReference> upcomingLiteratureReferences)
        {
            List<LiteratureReference> remainingLiteratureReferences = new List<LiteratureReference>();
            foreach (var literatureReference in LiteratureReferences)
            {
                var remainingIndication = upcomingLiteratureReferences.Any(x => x.LiteratureReferenceId == literatureReference.LiteratureReferenceId);
                if (remainingIndication)
                {
                    remainingLiteratureReferences.Add(literatureReference);
                }
            }
            LiteratureReferences = remainingLiteratureReferences;
        }

        private void AddNewOrUpdateOldReference(List<LiteratureReference> upcomingLiteratureReferences)
        {
            foreach (var literatureReference in upcomingLiteratureReferences)
            {
                if (literatureReference.LiteratureReferenceId == 0)
                {
                    LiteratureReferences.Add(literatureReference);
                }
                else
                {
                    var dbLiteratureReference = LiteratureReferences.FirstOrDefault(x => x.LiteratureReferenceId == literatureReference.LiteratureReferenceId);
                    dbLiteratureReference.Copy(literatureReference);
                }
            }
        }
    }
}
