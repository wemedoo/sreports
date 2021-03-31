using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IThesaurusEntryService
    {
        ThesaurusEntry GetById(string id);
        bool ExistsThesaurusEntry(string id);
        List<ThesaurusEntry> GetAll(ThesaurusEntryFilterData filterData);
        long GetAllEntriesCount(ThesaurusEntryFilterData filterData);
        string InsertOrUpdate(ThesaurusEntry thesaurusEntry, UserData user);
        bool Delete(string thesaurusEntryId, DateTime lastUpdate);
        bool ExistsThesaurusEntryByO4MtId(string o4MtId);
        ThesaurusEntry GetByO4MtIdId(string o4MtId);
        List<ThesaurusEntry> GetByIdsList(List<string> thesaurusList);
        long GetUmlsEntriesCount(); 
        void InsertThesaurusEntryWithId(ThesaurusEntry thesaurusEntry, UserData user);
    }
}
