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
        public void Delete(int encounterId, DateTime lastUpdate)
        {
            context.Encounters.FirstOrDefault(x => x.Id == encounterId).IsDeleted = true;
            context.SaveChanges();
        }
            

        public List<Encounter> GetAllByEocIdAsync(int eocId)
        {
            return context.Encounters
                .Where(x => !x.IsDeleted && x.EpisodeOfCareId.Equals(eocId))
                .OrderByDescending(x => x.Period.Start)
                .ToList();
        }

        public Encounter GetById(int id)
        {
            return context.Encounters.FirstOrDefault(x => x.Id == id);
        }

        public int Insert(Encounter encounter)
        {
            if (encounter.Id == 0)
            {
                encounter.EntryDatetime = DateTime.Now;
                context.Encounters.Add(encounter);
            }
            else 
            {
                Encounter dbEncounter = this.GetById(encounter.Id);
                dbEncounter.Class = encounter.Class;
                dbEncounter.Type = encounter.Type;
                dbEncounter.Status = encounter.Status;
                dbEncounter.ServiceType = encounter.ServiceType;
                dbEncounter.Period.Start = encounter.Period.Start;
                dbEncounter.Period.End = encounter.Period.End;

            }

            context.SaveChanges();

            return encounter.Id;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.Encounters
                .Where(x => x.Status == thesaurusId || x.Class == thesaurusId || x.Type == thesaurusId || x.ServiceType == thesaurusId)
                .Count() > 1;
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            List<Encounter> encounters = context.Encounters.Where(x => x.Status == oldThesaurus || x.Class == oldThesaurus || x.Type == oldThesaurus || x.ServiceType == oldThesaurus).ToList();
            foreach (Encounter encounter in encounters) 
            {
                encounter.ServiceType = encounter.ServiceType == oldThesaurus ? newThesaurus : encounter.ServiceType;
                encounter.Type = encounter.Type == oldThesaurus ? newThesaurus : encounter.Type;
                encounter.Class = encounter.Class == oldThesaurus ? newThesaurus : encounter.Class;
                encounter.Status = encounter.Status == oldThesaurus ? newThesaurus : encounter.Status;
            }

            context.SaveChanges();
        }
    }
}
