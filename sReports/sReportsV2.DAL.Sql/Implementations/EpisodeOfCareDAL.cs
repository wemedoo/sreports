using sReportsV2.Common.Entities.User;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace sReportsV2.SqlDomain.Implementations
{
    public class EpisodeOfCareDAL : IEpisodeOfCareDAL
    {
        private SReportsContext context;
        public EpisodeOfCareDAL(SReportsContext context)
        {
            this.context = context;
        }

        public bool Delete(int eocId, DateTime lastUpdate)
        {
            EpisodeOfCare eoc = this.GetById(eocId);
            eoc.IsDeleted = true;
            context.SaveChanges();
            return true;
        }

        public List<EpisodeOfCare> GetAll(EpisodeOfCareFilter filter)
        {
            return this.GetFiltered(filter)
                 .OrderByDescending(x => x.EntryDatetime)
                 .Skip((filter.Page - 1) * filter.PageSize)
                 .Take(filter.PageSize)
                 .ToList();
        }

        public long GetAllEntriesCount(EpisodeOfCareFilter filter)
        {
            return this.GetFiltered(filter).Count();
        }

        public EpisodeOfCare GetById(int id)
        {
            return context.EpisodeOfCares.Include(x => x.WorkflowHistory).FirstOrDefault(x => x.Id == id);
        }

        public int InsertOrUpdate(EpisodeOfCare entity, UserData user)
        {
            if (entity.Id == 0)
            {
                entity.EntryDatetime = DateTime.Now;
                entity.SetWorkflow(user);
                context.EpisodeOfCares.Add(entity);
            }
            else 
            {
                EpisodeOfCare episode = this.GetById(entity.Id);
                episode.Status = entity.Status;
                episode.Type = entity.Type;
                episode.DiagnosisCondition = entity.DiagnosisCondition;
                episode.DiagnosisRole = entity.DiagnosisRole;
                episode.DiagnosisRank = entity.DiagnosisRank;
                episode.Description = entity.Description;
                episode.SetWorkflow(user);
                episode.Period.Start = entity.Period.Start;
                episode.Period.End = entity.Period.End;
            }

            context.SaveChanges();

            return entity.Id;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return context.EpisodeOfCares.Any(x => x.Type == thesaurusId || x.DiagnosisRole == thesaurusId);
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            List<EpisodeOfCare> episodes = context.EpisodeOfCares.Where(x => x.Type == oldThesaurus || x.DiagnosisRole == oldThesaurus).ToList();
            foreach (EpisodeOfCare eoc in episodes)
            {
                eoc.Type = eoc.Type == oldThesaurus ? newThesaurus : eoc.Type;
                eoc.DiagnosisRole = eoc.DiagnosisRole == oldThesaurus ? newThesaurus : eoc.DiagnosisRole;
            }

            context.SaveChanges();
        }

        private IQueryable<EpisodeOfCare> GetFiltered(EpisodeOfCareFilter filter)
        {
            IQueryable<EpisodeOfCare> filteredData = context.EpisodeOfCares.Include(x => x.WorkflowHistory).Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(filter.Description))
            {
                filteredData = filteredData.Where(x => x.Description.Contains(filter.Description));
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                filteredData = filteredData.Where(x => x.Status.ToString() == filter.Status);
            }

            if (filter.PeriodStartDate != null)
            {
                DateTime beginStartDate = filter.PeriodStartDate ?? DateTime.Now;
                DateTime endStartDate = beginStartDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.Start >= beginStartDate && x.Period.Start < endStartDate);
            }

            if (filter.PeriodEndDate != null)
            {
                DateTime beginEndDate = filter.PeriodEndDate ?? DateTime.Now;
                DateTime endEndDate = beginEndDate.AddDays(1);
                filteredData = filteredData.Where(x => x.Period.End >= beginEndDate && x.Period.End < endEndDate);
            }

            if (filter.Type != 0)
            {
                filteredData = filteredData.Where(x => x.Type.Equals(filter.Type));
            }

            if (filter.FilterByIdentifier)
            {
                filteredData = filteredData.Where(x => x.PatientId.Equals(filter.PatientId));
            }


            filteredData = filteredData.Where(x => x.OrganizationId == filter.OrganizationId);

            return filteredData;
        }
    }
}
