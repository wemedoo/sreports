using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public partial class ThesaurusEntryBLL : IThesaurusEntryBLL
    {
        private const string MergeCodes = "Codes";
        private const string MergeDefinitions = "Definition";
        private const string MergeAbbreviations = "Abbreviations";
        private const string MergeSynonyms = "Synonyms";
        private const string MergeTranslations = "Translations";

        public void TakeBoth(int currentId)
        {
            thesaurusDAL.UpdateState(currentId, ThesaurusState.Production);
        }

        public void MergeThesauruses(int sourceId, int targetId, List<string> valuesForMerge, UserData userData)
        {
            ThesaurusEntry sourceThesaurus = thesaurusDAL.GetById(sourceId);
            ThesaurusEntry targetThesaurus = thesaurusDAL.GetById(targetId);

            if (targetThesaurus == null)
            {
                throw new NullReferenceException($"Target thesaurus with given id (id=${targetId}) does not exist");
            }
            MergeValues(sourceThesaurus, targetThesaurus, valuesForMerge, userData);

            ThesaurusMerge thesaurusMerge = new ThesaurusMerge()
            {
                State = ThesaurusMergeState.Pending,
                NewThesaurus = targetId,
                OldThesaurus = sourceId
            };
            thesaurusMergeDAL.InsertOrUpdate(thesaurusMerge);
        }

        public int Merge(UserData userData)
        {
            int enumsEntriesUpdated = 0;
            foreach (ThesaurusMerge thesaurusMerge in thesaurusMergeDAL.GetAllByState(ThesaurusMergeState.Pending))
            {
                enumsEntriesUpdated += Replace(thesaurusMerge, userData);
                TryDelete(thesaurusMerge.OldThesaurus);
            }

            return enumsEntriesUpdated;
        }

        private void MergeValues(ThesaurusEntry sourceThesaurus, ThesaurusEntry targetThesaurus, List<string> valuesForMerge, UserData userData)
        {
            if (valuesForMerge != null)
            {
                UpdateMergeListIfThereAreDependentActions(valuesForMerge);
                foreach (string value in valuesForMerge)
                {
                    switch (value)
                    {
                        case MergeTranslations:
                            targetThesaurus.MergeTranslations(sourceThesaurus);
                            break;
                        case MergeCodes:
                            targetThesaurus.MergeCodes(sourceThesaurus);
                            break;
                        case MergeSynonyms:
                            targetThesaurus.MergeSynonyms(sourceThesaurus);
                            break;
                        case MergeAbbreviations:
                            targetThesaurus.MergeAbbreviations(sourceThesaurus);
                            break;
                        case MergeDefinitions:
                            targetThesaurus.MergeDefinitions(sourceThesaurus);
                            break;
                        default:
                            break;
                    }
                }

                CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(targetThesaurus), userData);
            }
        }

        private void UpdateMergeListIfThereAreDependentActions(List<string> valuesForMerge)
        {
            if (valuesForMerge.Any(v => v.Equals(MergeTranslations)))
            {
                valuesForMerge.Remove(MergeSynonyms);
                valuesForMerge.Remove(MergeAbbreviations); 
                valuesForMerge.Remove(MergeDefinitions);
            }
        }

        private int Replace(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            thesaurusMerge.FailedCollections = new List<string>();
            thesaurusMerge.CompletedCollections = new List<string>();

            ReplaceFromForm(thesaurusMerge, userData);
            ReplaceFromFormInstance(thesaurusMerge);
            ReplaceFromFormDistribution(thesaurusMerge);
            int enumsEntriesUpdated = ReplaceFromEnum(thesaurusMerge);
            ReplaceFromEncounter(thesaurusMerge);
            ReplaceFromEpisodeOfCare(thesaurusMerge);

            thesaurusMerge.State = thesaurusMerge.FailedCollections.Count == 0 ? ThesaurusMergeState.Completed : ThesaurusMergeState.NotCompleted;
            thesaurusMerge.SetLastUpdate();

            return enumsEntriesUpdated;
        }
        private void ReplaceFromForm(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            foreach (Form form in formService.GetAll(null))
            {
                form.ReplaceThesauruses(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
                formService.InsertOrUpdate(form, userData);
            }

            if (formService.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormDAL));

            }
        }

        private void ReplaceFromFormInstance(ThesaurusMerge thesaurusMerge)
        {
            formInstanceService.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (formInstanceService.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormInstanceDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormInstanceDAL));
            }
        }

        private void ReplaceFromFormDistribution(ThesaurusMerge thesaurusMerge)
        {
            foreach (FormDistribution formDistribution in formDistributionService.GetAll())
            {
                formDistribution.ReplaceThesauruses(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
                formDistributionService.InsertOrUpdate(formDistribution);
            }

            if (formDistributionService.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormDistributionDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormDistributionDAL));
            }
        }

        private int ReplaceFromEnum(ThesaurusMerge thesaurusMerge)
        {
            int entriesUpdated = customEnumDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
            if (customEnumDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(CustomEnumDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(CustomEnumDAL));
            }

            return entriesUpdated;
        }

        private void ReplaceFromEncounter(ThesaurusMerge thesaurusMerge)
        {
            encounterDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (encounterDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(encounterDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(encounterDAL));
            }
        }

        private void ReplaceFromEpisodeOfCare(ThesaurusMerge thesaurusMerge)
        {
            episodeOfCareDAL.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);

            if (episodeOfCareDAL.ThesaurusExist(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(episodeOfCareDAL));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(episodeOfCareDAL));
            }
        }
    }
}
