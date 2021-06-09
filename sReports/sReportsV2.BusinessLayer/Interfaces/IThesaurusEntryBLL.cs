using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface  IThesaurusEntryBLL 
    {
        bool ExistsThesaurusEntry(int id);
        ThesaurusEntry GetById(int id);
        ThesaurusEntryDataOut GetThesaurusByFilter(CodesFilterDataIn filterDataIn);
        List<string> GetAll(string language, string searchValue, int page);
        ThesaurusEntryCountDataOut GetEntriesCount();
        ThesaurusEntryDataOut GetThesaurusDataOut(int id);
        PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> ReloadCodes(CodesFilterDataIn filterDataIn);
        PaginationDataOut<ThesaurusEntryDataOut, DataIn> ReloadTable(ThesaurusEntryFilterDataIn dataIn);
        PaginationDataOut<ThesaurusEntryDataOut, GlobalThesaurusFilterDataIn> ReloadThesaurus(GlobalThesaurusFilterDataIn filterDataIn);
        PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, ThesaurusEntry thesaurus, DTOs.User.DTO.UserCookieData userCookieData);
        PaginationDataOut<ThesaurusEntryDataOut, AdministrationFilterDataIn> GetByAdministrationTerm(AdministrationFilterDataIn dataIn);
        void SetThesaurusVersions(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel);
        int TryInsertOrUpdate(ThesaurusEntryDataIn thesaurusEntry);
        int TryInsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? tid);
        string CreateThesaurus(ThesaurusEntryDataIn thesaurusEntryDTO, DTOs.User.DTO.UserCookieData userCookieData);
        void UpdateState(int thesaurusId, ThesaurusState state);
        void DeleteCode(int id);
        void TryDelete(int id);


    }
}
