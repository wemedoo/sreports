using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IThesaurusDAL
    {
        void InsertOrUpdate(ThesaurusEntry thesaurus);
        ThesaurusEntry GetById(int id);
        IQueryable<ThesaurusEntry> GetFilteredQuery(GlobalThesaurusFilter filterDataIn);
        List<ThesaurusEntry> GetFiltered(GlobalThesaurusFilter filterDataIn);
        int GetFilteredCount(GlobalThesaurusFilter filterDataIn);
        void DeleteCode(int id);
        int GetAllCount();
        void InsertMany(List<ThesaurusEntry> thesauruses);
        List<int> GetLastBulkInserted(int size);
        List<int> GetBulkInserted(int size);
        int GetAllEntriesCount(ThesaurusEntryFilterData filterData);
        long GetUmlsEntriesCount();
        List<ThesaurusEntry> GetAll(ThesaurusEntryFilterData filterData);
        bool ExistsThesaurusEntry(int id);
        void Delete(int id);
        List<ThesaurusEntry> GetAllSimilar(ThesaurusReviewFilterData filter, string preferredTerm, string language);
        long GetAllSimilarCount(ThesaurusReviewFilterData filter, string preferredTerm, string language);
        void UpdateState(int thesaurusId, ThesaurusState state);
        List<ThesaurusEntry> GetByIdsList(List<int> thesaurusList);
        List<string> GetAll(string language, string searchValue, int page);
        O4CodeableConcept GetCodeById(int id);
        int GetIdByPreferredTerm(string prefferedTerm);
    }
}
