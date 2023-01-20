using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class EncounterDAL : IEncounterDAL
    {
        private SReportsContext context;
        public EncounterDAL(SReportsContext context)
        {
            this.context = context;
        }
        public void Delete(int encounterId)
        {
            Encounter fromDb = context.Encounters.FirstOrDefault(x => x.EncounterId == encounterId);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }
            

        public List<Encounter> GetAllByEocId(int eocId)
        {
            return context.Encounters
                .Where(x => !x.IsDeleted && x.EpisodeOfCareId.Equals(eocId))
                .OrderByDescending(x => x.Period.Start)
                .ToList();
        }

        public Encounter GetById(int id)
        {
            return context.Encounters.FirstOrDefault(x => x.EncounterId == id);
        }

        public int InsertOrUpdate(Encounter encounter)
        {
            if (encounter.EncounterId == 0)
            {
                context.Encounters.Add(encounter);
            }
            else 
            {
                Encounter dbEncounter = this.GetById(encounter.EncounterId);
                dbEncounter.Class = encounter.Class;
                dbEncounter.Type = encounter.Type;
                dbEncounter.Status = encounter.Status;
                dbEncounter.ServiceType = encounter.ServiceType;
                dbEncounter.Period.Start = encounter.Period.Start;
                dbEncounter.Period.End = encounter.Period.End;

            }

            context.SaveChanges();

            return encounter.EncounterId;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.Encounters
                .Any(x => x.Status == thesaurusId || x.Class == thesaurusId || x.Type == thesaurusId || x.ServiceType == thesaurusId);
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            List<Encounter> encounters = context.Encounters.Where(x => x.Status == oldThesaurus || x.Class == oldThesaurus || x.Type == oldThesaurus || x.ServiceType == oldThesaurus).ToList();
            foreach (Encounter encounter in encounters) 
            {
                encounter.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }

            context.SaveChanges();
        }
    }
}
