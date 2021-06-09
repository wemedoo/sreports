using AutoMapper;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public partial class ThesaurusEntryController : BaseController
    {

        [SReportsAutorize]
        public ActionResult TakeBoth(int currentId)
        {
            thesaurusEntryBLL.UpdateState(currentId, ThesaurusState.Production);
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [SReportsAutorize]
        public ActionResult MergeThesauruses(int currentId, int targetId, List<string> valuesForMerge)
        {
            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurus = thesaurusEntryBLL.GetById(currentId);

            sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry mergedThesaurus = thesaurusEntryBLL.GetById(targetId);
            if (mergedThesaurus == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            MergeValues(thesaurus, mergedThesaurus, valuesForMerge);


            ThesaurusMerge thesaurusMerge = new ThesaurusMerge()
            {
                State = ThesaurusMergeState.Pending,
                NewThesaurus = targetId,
                OldThesaurus = currentId
            };


            thesaurusMergeService.InsertOrUpdate(thesaurusMerge);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void MergeValues(sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry thesaurusForMerge, sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry targetThesaurus, List<string> valuesForMerge)
        {
            if (valuesForMerge != null)
            {
                foreach (string value in valuesForMerge)
                {
                    switch (value)
                    {
                        case "Translations":
                            targetThesaurus.MergeTranslations(thesaurusForMerge);
                            break;
                        case "Codes":
                            targetThesaurus.MergeCodes(thesaurusForMerge);
                            break;
                        case "Synonyms":
                            targetThesaurus.MergeSynonyms(thesaurusForMerge);
                            break;
                        case "Abbreviations":
                            targetThesaurus.MergeAbbreviations(thesaurusForMerge);
                            break;
                        default:
                            // code block
                            break;
                    }
                }

                thesaurusEntryBLL.CreateThesaurus(Mapper.Map<ThesaurusEntryDataIn>(targetThesaurus), userCookieData);
            }

        }




        public ActionResult Merge()
        {
            foreach (ThesaurusMerge thesaurusMerge in thesaurusMergeService.GetAllByState(ThesaurusMergeState.Pending))
            {
                Replace(thesaurusMerge);
                thesaurusEntryBLL.TryDelete(thesaurusMerge.OldThesaurus);
            }

            //SingletonDataContainer.Instance.RefreshSingleton();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void Replace(ThesaurusMerge thesaurusMerge)
        {
            thesaurusMerge.FailedCollections = new List<string>();
            thesaurusMerge.CompletedCollections = new List<string>();

            ReplaceFromForm(thesaurusMerge);
            ReplaceFromFormInstance(thesaurusMerge);
            ReplaceFromFormDistribution(thesaurusMerge);
            //ReplaceFromEnum(enumService, thesaurusMerge);
            ReplaceFromEncounter(thesaurusMerge);
            ReplaceFromEpisodeOfCare(thesaurusMerge);

            thesaurusMerge.State = thesaurusMerge.FailedCollections.Count == 0 ? ThesaurusMergeState.Completed : ThesaurusMergeState.NotComplited;

        }
        private void ReplaceFromForm(ThesaurusMerge thesaurusMerge)
        {
            foreach (Form form in formService.GetAll(null))
            {
                form.ReplaceThesauruses(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
                formService.InsertOrUpdate(form, Mapper.Map<UserData>(userCookieData));
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

            if (formInstanceService.ExistThesaurus(thesaurusMerge.OldThesaurus))
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

            if (formDistributionService.ExistThesaurus(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(nameof(FormDistributionService));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(FormDistributionService));
            }
        }

        /*private void ReplaceFromEnum(IEnumService service, ThesaurusMerge thesaurusMerge)
        {
            service.UpdateManyWithThesaurus(thesaurusMerge.OldThesaurus, thesaurusMerge.NewThesaurus);
            if (service.ExistThesaurus(thesaurusMerge.OldThesaurus))
            {
                thesaurusMerge.FailedCollections.Add(service.ToString().Split('.').Last());
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(service.ToString().Split('.').Last());
            }
        }*/
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
                thesaurusMerge.FailedCollections.Add(nameof(EpisodeOfCareService));
            }
            else
            {
                thesaurusMerge.CompletedCollections.Add(nameof(EpisodeOfCareService));

            }
        }
    }
}