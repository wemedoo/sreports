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
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataOut;
using sReportsV2.DTOs.DTOs.CodeSystem;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class ThesaurusEntryBLL : IThesaurusEntryBLL
    {
        private readonly IUserDAL userDAL;
        private readonly IGlobalThesaurusUserDAL globalThesaurusUserDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IThesaurusMergeDAL thesaurusMergeDAL;
        private readonly IFormDAL formService;
        private readonly IFormInstanceDAL formInstanceService;
        private readonly IFormDistributionDAL formDistributionService;
        private readonly IEncounterDAL encounterDAL;
        private readonly IEpisodeOfCareDAL episodeOfCareDAL;
        private readonly ICustomEnumDAL customEnumDAL;

        private readonly ICodeSystemDAL codeSystemDAL;
        
        public ThesaurusEntryBLL(IUserDAL userDAL, IGlobalThesaurusUserDAL globalThesaurusUserDAL, IOrganizationDAL organizationDAL, IThesaurusDAL thesaurusDAL, ICodeSystemDAL codeSystemDAL, IThesaurusMergeDAL thesaurusMergeDAL, ICustomEnumDAL customEnumDAL, IEncounterDAL encounterDAL, IEpisodeOfCareDAL episodeOfCareDAL)
        {
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.thesaurusDAL = thesaurusDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.thesaurusMergeDAL = thesaurusMergeDAL;
            this.customEnumDAL = customEnumDAL;
            this.encounterDAL = encounterDAL;
            this.episodeOfCareDAL = episodeOfCareDAL;
            this.formService = new FormDAL();
            this.formInstanceService = new FormInstanceDAL();
            this.formDistributionService = new FormDistributionDAL();
            this.globalThesaurusUserDAL = globalThesaurusUserDAL;
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
                SetUserAdministrativeData(thesaurus, dataOut);
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
                        .Where(x => (filterDataIn.CodeSystems == null || filterDataIn.CodeSystems.Count() == 0 || filterDataIn.CodeSystems.Contains(x.System.SAB)))
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
            string preferredTerm = string.IsNullOrWhiteSpace(filter.PreferredTerm) ? thesaurus.GetPreferredTermByTranslationOrDefault(userCookieData.ActiveLanguage) : filter.PreferredTerm;

            List<ThesaurusEntry> similarThesauruses = thesaurusDAL.GetAllSimilar(filterData, preferredTerm, userCookieData.ActiveLanguage);

            PaginationDataOut<ThesaurusEntryDataOut, DataIn> result = new PaginationDataOut<ThesaurusEntryDataOut, DataIn>()
            {
                Count = (int)thesaurusDAL.GetAllSimilarCount(filterData, preferredTerm, userCookieData.ActiveLanguage),
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
                        Id = version.VersionId,
                        User = Mapper.Map<UserShortInfoDataOut>(userDAL.GetById(version.UserId)),
                        Organization = Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(version.OrganizationId)),
                        State = version.State
                    });
                }
            }
        }

        public int TryInsertOrUpdate(ThesaurusEntryDataIn thesaurusDataIn, UserData userData)
        {
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusDataIn);
            if(thesaurusEntry.ThesaurusEntryId != 0)
            {
                ThesaurusEntry thesaurusEntryDb = this.thesaurusDAL.GetById(thesaurusEntry.ThesaurusEntryId);
                thesaurusEntryDb.Copy(thesaurusEntry, false);
                thesaurusEntry = thesaurusEntryDb;
            }
            UpdateAdministrativeData(thesaurusEntry, userData, thesaurusDataIn.State);
            this.thesaurusDAL.InsertOrUpdate(thesaurusEntry);
            return thesaurusEntry.ThesaurusEntryId;
        }

        public int UpdateConnectionWithOntology(ThesaurusEntryDataIn thesaurusDataIn, UserData userData)
        {
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusDataIn);
            if (thesaurusEntry.ThesaurusEntryId != 0)
            {
                ThesaurusEntry thesaurusEntryDb = this.thesaurusDAL.GetById(thesaurusEntry.ThesaurusEntryId);
                thesaurusEntryDb.CopyConnectionWithOntology(thesaurusEntry);
                thesaurusEntry = thesaurusEntryDb;
            }
            UpdateAdministrativeData(thesaurusEntry, userData, thesaurusEntry.State);
            this.thesaurusDAL.InsertOrUpdate(thesaurusEntry);
            return thesaurusEntry.ThesaurusEntryId;
        }

        public ResourceCreatedDTO TryInsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            var savedObjects = SaveCodeChanges(codeDataIn, thesaurusEntryId);
            ThesaurusEntry thesaurus = savedObjects.Item1;
            return new ResourceCreatedDTO() 
            {
                Id = thesaurus.ThesaurusEntryId.ToString(),
                RowVersion = Convert.ToBase64String(thesaurus.RowVersion)
            };
        }

        public O4CodeableConceptDataOut InsertOrUpdateCode(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            var savedObjects = SaveCodeChanges(codeDataIn, thesaurusEntryId);
            int codeId = savedObjects.Item2;
            ThesaurusEntry thesaurusEntry = thesaurusDAL.GetById(thesaurusEntryId.GetValueOrDefault());
            O4CodeableConcept code = thesaurusEntry.Codes.FirstOrDefault(x => x.O4CodeableConceptId == codeId);
            return Mapper.Map<O4CodeableConceptDataOut>(code);
        }

        public ResourceCreatedDTO CreateThesaurus(ThesaurusEntryDataIn thesaurusEntryDTO, UserData userData)
        {
            MapCodesToExistingValues(thesaurusEntryDTO);
            ThesaurusEntry thesaurusEntry = Mapper.Map<ThesaurusEntry>(thesaurusEntryDTO);
            ThesaurusEntry thesaurusEntryDb = thesaurusDAL.GetById(thesaurusEntry.ThesaurusEntryId);
            if (thesaurusEntryDb == null)
            {
                thesaurusEntryDb = thesaurusEntry;
                thesaurusEntryDb.AdministrativeData = new Domain.Sql.Entities.ThesaurusEntry.AdministrativeData(userData, thesaurusEntryDb.State);
            }   
            else
            {
                thesaurusEntryDb.Copy(thesaurusEntry);
                UpdateAdministrativeData(thesaurusEntryDb, userData, thesaurusEntry.State);
            }

            thesaurusDAL.InsertOrUpdate(thesaurusEntryDb);

            return new ResourceCreatedDTO()
            {
                Id = thesaurusEntryDb.ThesaurusEntryId.ToString(),
                RowVersion = Convert.ToBase64String(thesaurusEntryDb.RowVersion)
            };
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

        public ThesaurusEntryDataOut UpdateTranslation(ThesaurusEntryTranslationDataIn thesaurusEntryTranslationDataIn, UserData userData)
        {
            ThesaurusEntryTranslation thesaurusEntryTranslation = Mapper.Map<ThesaurusEntryTranslation>(thesaurusEntryTranslationDataIn);
            ThesaurusEntry thesaurusEntry = this.thesaurusDAL.GetById(thesaurusEntryTranslation.ThesaurusEntryId);
            thesaurusEntry.UpdateTranslation(thesaurusEntryTranslation);
            UpdateAdministrativeData(thesaurusEntry, userData, thesaurusEntry.State);
            thesaurusDAL.InsertOrUpdate(thesaurusEntry);
            ThesaurusEntryDataOut thesaurusEntryDataOut = Mapper.Map<ThesaurusEntryDataOut>(thesaurusEntry);
            return thesaurusEntryDataOut;
        }

        public ThesaurusGlobalCountDataOut GetThesaurusGlobalChartData()
        {
            ThesaurusGlobalCountDataOut result = new ThesaurusGlobalCountDataOut()
            {
                ThesaurusTotal = this.thesaurusDAL.GetFilteredCount(null),
            };

            return result;
        }

        public void InsertOrUpdateCodeSystem(CodeSystemDataIn codeSystemDataIn)
        {
            CodeSystem codeSystem = Mapper.Map<CodeSystem>(codeSystemDataIn);
            CodeSystem codeSystemDb = codeSystemDAL.GetById(codeSystemDataIn.Id);
            if (codeSystemDb == null)
            {
                codeSystemDb = codeSystem;
            }
            else
            {
                codeSystemDb.Copy(codeSystem);
            }

            codeSystemDAL.InsertOrUpdate(codeSystemDb);
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
                        Id = code.O4CodeableConceptId

                    };

                    result.Add(codeDataOut);
                }
            }

            return result;
        }

        private void MapCodesToExistingValues(ThesaurusEntryDataIn thesaurusEntry)
        {
            if (thesaurusEntry.Codes != null)
            {
                foreach (O4CodeableConceptDataIn code in thesaurusEntry.Codes.FindAll(c => c.CodeSystemId <= 0))
                {
                    CodeSystem system = codeSystemDAL.GetBySAB(code.CodeSystemAbbr);
                    if (system != null)
                    {
                        code.CodeSystemId = system.CodeSystemId;
                    }
                }
                int numOfDeleted = thesaurusEntry.Codes.RemoveAll(c => c.CodeSystemId <= 0);
            }
        }

        private void UpdateAdministrativeData(ThesaurusEntry thesaurusEntry, UserData userData, ThesaurusState? state)
        {
            thesaurusEntry.AdministrativeData = thesaurusEntry.AdministrativeData ?? new Domain.Sql.Entities.ThesaurusEntry.AdministrativeData(userData, ThesaurusState.Draft);
            thesaurusEntry.AdministrativeData.UpdateVersionHistory(userData, state);
        }

        private void SetUserAdministrativeData(ThesaurusEntry data, ThesaurusEntryDataOut dataOut)
        {
            if(data.AdministrativeData != null)
            {
                foreach(var version in data.AdministrativeData.VersionHistory)
                {
                    var versionDataOut = dataOut.AdministrativeData.VersionHistory.FirstOrDefault(v => v.Id == version.VersionId);
                    var globalUser = globalThesaurusUserDAL.GetById(version.UserId);
                    if(globalUser != null && versionDataOut != null)
                    {
                        versionDataOut.User = new UserShortInfoDataOut() { Username = globalUser.Email, FirstName = globalUser.FirstName, LastName = globalUser.LastName };
                    }
                }
            }
        }

        private Tuple<ThesaurusEntry, int> SaveCodeChanges(O4CodeableConceptDataIn codeDataIn, int? thesaurusEntryId)
        {
            O4CodeableConcept code = Mapper.Map<O4CodeableConcept>(codeDataIn);

            code.EntryDateTime = DateTime.Now;
            ThesaurusEntry thesaurus = thesaurusDAL.GetById(thesaurusEntryId.GetValueOrDefault());
            if (thesaurus != null)
            {
                if (code.O4CodeableConceptId == 0)
                {
                    thesaurus.Codes.Add(code);
                }
                else
                {
                    var dbCode = thesaurus.Codes.FirstOrDefault(x => x.O4CodeableConceptId == code.O4CodeableConceptId);
                    if (dbCode != null)
                    {
                        dbCode.Copy(code);
                    }
                }
                thesaurus.SetLastUpdate();

                thesaurusDAL.InsertOrUpdate(thesaurus);
            }

            return new Tuple<ThesaurusEntry, int>(thesaurus, code.O4CodeableConceptId);
        }
    }
}
