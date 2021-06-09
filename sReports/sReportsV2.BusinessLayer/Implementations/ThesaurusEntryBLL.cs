using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version = sReportsV2.Domain.Sql.Entities.ThesaurusEntry.Version;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Common;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Administration;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ThesaurusEntryBLL : IThesaurusEntryBLL
    {
        private readonly IUserDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        public ThesaurusEntryBLL(IUserDAL userDAL, IOrganizationDAL organizationDAL, IThesaurusDAL thesaurusDAL)
        {
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.thesaurusDAL = thesaurusDAL;
        }

        public bool ExistsThesaurusEntry(int id)
        {
            return thesaurusDAL.ExistsThesaurusEntry(id);
        }

        public ThesaurusEntry GetById(int id)
        {
            return thesaurusDAL.GetById(id);
        }

        public ThesaurusEntryDataOut GetThesaurusByFilter(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntryDataOut dataOut = null;
            if (!string.IsNullOrWhiteSpace(filterDataIn.Id.ToString()))
            {
                ThesaurusEntry thesaurus = thesaurusDAL.GetById(filterDataIn.Id.Value);
                dataOut = Mapper.Map<ThesaurusEntryDataOut>(thesaurus);
                dataOut.Codes = GetCodes(thesaurus);
            }

            return dataOut;
        }

        public List<string> GetAll(string language, string searchValue, int page)
        {
            return thesaurusDAL.GetAll(language, searchValue, page);
        }

        public ThesaurusEntryCountDataOut GetEntriesCount()
        {
            ThesaurusEntryCountDataOut result = new ThesaurusEntryCountDataOut()
            {
                Total = this.thesaurusDAL.GetAllEntriesCount(null),
                TotalUmls = this.thesaurusDAL.GetUmlsEntriesCount()
            };

            return result;
        }

        public ThesaurusEntryDataOut GetThesaurusDataOut(int id)
        {
            ThesaurusEntry thesaurus = this.thesaurusDAL.GetById(id);
            ThesaurusEntryDataOut dataOut = Mapper.Map<ThesaurusEntryDataOut>(thesaurus);

            return dataOut;
        }

        public PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> ReloadCodes(CodesFilterDataIn filterDataIn)
        {
            ThesaurusEntry thesaurus = thesaurusDAL.GetById(filterDataIn.Id.Value);
            List<O4CodeableConceptDataOut> codes = GetCodes(thesaurus)
                        .Where(x => (filterDataIn.CodeSystems == null || filterDataIn.CodeSystems.Count() == 0 || filterDataIn.CodeSystems.Contains(x.System.Value)))
                        .OrderByDescending(x => x.EntryDateTime)
                        .ToList();


            PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn> result = new PaginationDataOut<O4CodeableConceptDataOut, CodesFilterDataIn>()
            {
                Count = (int)codes.Count(),
                Data = codes.Skip((filterDataIn.Page - 1) * filterDataIn.PageSize).Take(filterDataIn.PageSize).ToList(),
                DataIn = filterDataIn
            };

            return result;
        }

        public PaginationDataOut<ThesaurusEntryDataOut, DataIn> ReloadTable(ThesaurusEntryFilterDataIn dataIn)
        {
            ThesaurusEntryFilterData filterData = Mapper.Map<ThesaurusEntryFilterData>(dataIn);
            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = new PaginationDataOut<ThesaurusEntryDataOut, DataIn>()
            {
                Count = (int)this.thesaurusDAL.GetAllEntriesCount(filterData),
                Data = Mapper.Map<List<ThesaurusEntryDataOut>>(this.thesaurusDAL.GetAll(filterData)),
                DataIn = dataIn
            };
            return result;
        }

        public PaginationDataOut<ThesaurusEntryDataOut, DataIn> GetReviewTreeDataOut(ThesaurusReviewFilterDataIn filter, ThesaurusEntry thesaurus, DTOs.User.DTO.UserCookieData userCookieData)
        {
            ThesaurusReviewFilterData filterData = Mapper.Map<ThesaurusReviewFilterData>(filter);

            List<ThesaurusEntry> similarThesauruses = thesaurusDAL.GetAllSimilar(filterData, string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.Translations.FirstOrDefault(x => x.Language == userCookieData.ActiveLanguage).PreferredTerm : filter.PreferredTerm, userCookieData.ActiveLanguage);
            List<ThesaurusEntryDataOut> list = Mapper.Map<List<ThesaurusEntryDataOut>>(similarThesauruses);

            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = new PaginationDataOut<ThesaurusEntryDataOut, DataIn>()
            {
                Count = (int)thesaurusDAL.GetAllSimilarCount(filterData, string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.Translations.FirstOrDefault(x => x.Language == userCookieData.ActiveLanguage).PreferredTerm : filter.PreferredTerm, userCookieData.ActiveLanguage),
                Data = Mapper.Map<List<ThesaurusEntryDataOut>>(similarThesauruses),
                DataIn = filter
            };

            return result;
        }

        public PaginationDataOut<ThesaurusEntryDataOut, GlobalThesaurusFilterDataIn> ReloadThesaurus(GlobalThesaurusFilterDataIn filterDataIn)
        {
            GlobalThesaurusFilter filter = Mapper.Map<GlobalThesaurusFilter>(filterDataIn);

            PaginationDataOut<ThesaurusEntryDataOut, GlobalThesaurusFilterDataIn> result = new PaginationDataOut<ThesaurusEntryDataOut, GlobalThesaurusFilterDataIn>()
            {
                Count = (int)thesaurusDAL.GetFilteredCount(filter),
                Data = Mapper.Map<List<ThesaurusEntryDataOut>>(thesaurusDAL.GetFiltered(filter)),
                DataIn = filterDataIn
            };

            return result;
        }

        public PaginationDataOut<ThesaurusEntryDataOut, AdministrationFilterDataIn> GetByAdministrationTerm(AdministrationFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            PaginationDataOut<ThesaurusEntryDataOut, AdministrationFilterDataIn> result = null;
            if (!string.IsNullOrEmpty(dataIn.PreferredTerm))
            {
                ThesaurusEntryFilterData filterData = Mapper.Map<ThesaurusEntryFilterData>(dataIn);
                result = new PaginationDataOut<ThesaurusEntryDataOut, AdministrationFilterDataIn>()
                {
                    Count = (int)this.thesaurusDAL.GetAllEntriesCount(filterData),
                    Data = Mapper.Map<List<ThesaurusEntryDataOut>>(this.thesaurusDAL.GetAll(filterData)),
                    DataIn = dataIn
                };
            }
            return result;
        }

        public void SetThesaurusVersions(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel)
        {
            if (thesaurusEntry.AdministrativeData != null && thesaurusEntry.AdministrativeData.VersionHistory != null)
            {
                viewModel.AdministrativeData = new AdministrativeDataDataOut();
                foreach (Version version in thesaurusEntry.AdministrativeData.VersionHistory)
                {
                    viewModel.AdministrativeData.VersionHistory.Add(new VersionDataOut()
                    {
                        CreatedOn = version.CreatedOn,
                        RevokedOn = version.RevokedOn,
                        Id = version.Id,
                        User = Mapper.Map<UserShortInfoDataOut>(userDAL.GetById(version.UserId)),
                        Organization = Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(version.OrganizationId)),
                        State = version.State
                    });
                }
            }
        }

        public int TryInsertOrUpdate(ThesaurusEntryDataIn thesaurusDataIn)
        {
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusDataIn);
            int result = this.thesaurusDAL.InsertOrUpdate(thesaurusEntry);
            return result;
        }

        public int TryInsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? tid)
        {
            O4CodeableConcept code = Mapper.Map<O4CodeableConcept>(codeDataIn);
            int result = this.thesaurusDAL.InsertOrUpdateCode(code, tid.Value);
            return result;
        }

        public string CreateThesaurus(ThesaurusEntryDataIn thesaurusEntryDTO, DTOs.User.DTO.UserCookieData userCookieData)
        {
            UserData userData = Mapper.Map<Common.Entities.User.UserData>(userCookieData);
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusEntryDTO);
            string result = thesaurusDAL.InsertOrUpdate(thesaurusEntry, userData);

            return result;
        }

        public void UpdateState(int thesaurusId, ThesaurusState state)
        {
            thesaurusDAL.UpdateState(thesaurusId, state);
        }

        public void TryDelete(int id)
        {
            try
            {
                thesaurusDAL.Delete(id);
            }
            catch (Exception e)
            {

            }
        }

        public void DeleteCode(int id)
        {
            thesaurusDAL.DeleteCode(id);
        }

        private List<O4CodeableConceptDataOut> GetCodes(ThesaurusEntry thesaurusEntry)
        {
            List<O4CodeableConceptDataOut> result = new List<O4CodeableConceptDataOut>();
            if (thesaurusEntry != null && thesaurusEntry.Codes != null)
            {
                foreach (O4CodeableConcept code in thesaurusEntry.Codes.Where(x => !x.IsDeleted).ToList())
                {
                    O4CodeableConceptDataOut codeDataOut = new O4CodeableConceptDataOut()
                    {
                        System = Mapper.Map<CodeSystemDataOut>(code.System),
                        Version = code.Version,
                        Code = code.Code,
                        Value = code.Value,
                        VersionPublishDate = code.VersionPublishDate,
                        Link = code.Link,
                        EntryDateTime = code.EntryDateTime,
                        Id = code.Id

                    };

                    result.Add(codeDataOut);
                }
            }

            return result;
        }
    }
}
