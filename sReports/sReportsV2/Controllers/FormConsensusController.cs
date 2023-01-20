using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Singleton;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class FormConsensusController : FormCommonController
    {
        protected readonly IConsensusInstanceDAL consensusInstanceDAL;
        protected readonly IConsensusBLL consensusBLL;
        protected readonly IConsensusDAL consensusDAL;

        private readonly IOutsideUserDAL outsideUserDAL;
        private readonly IUserDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        public ILogger Logger;

        public FormConsensusController(IPatientDAL patientDAL, 
            IEpisodeOfCareDAL episodeOfCareDAL,
            IEncounterDAL encounterDAL, 
            IUserBLL userBLL, 
            IOrganizationBLL organizationBLL, 
            ICustomEnumBLL customEnumBLL, 
            IFormInstanceBLL formInstanceBLL, 
            IFormBLL formBLL, 
            IOutsideUserDAL outsideUserDAL, 
            IUserDAL userDAL, 
            IOrganizationDAL organizationDAL,
            IConsensusDAL consensusDAL,
            IConsensusInstanceDAL consensusInstanceDAL,
            IConsensusBLL consensusBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
            this.outsideUserDAL = outsideUserDAL;
            this.consensusDAL = consensusDAL;
            this.consensusInstanceDAL = consensusInstanceDAL;
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.consensusBLL = consensusBLL;
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpGet]
        public ActionResult GetMapObject()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var obj = System.IO.File.ReadAllText($@"{path}\App_Data\countries-50m.json");
            return Content(obj);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult StartNewIteration(string consensusId, string formId)
        {
            ResourceCreatedDTO resourceCreatedDTO = consensusBLL.StartNewIteration(consensusId, formId, userCookieData.Id);

            return new JsonResult()
            {
                Data = resourceCreatedDTO,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult TerminateCurrentIteration(string consensusId)
        {
            ResourceCreatedDTO resourceCreatedDTO = consensusBLL.TerminateCurrentIteration(consensusId);
           
            return new JsonResult()
            {
                Data = resourceCreatedDTO,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult LoadConsensusPartial(string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId)));
            return PartialView("~/Views/Form/Consensus/ConsensusPartial.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }

        [SReportsAuditLog]
        public ActionResult GetQuestionnairePartial(string formId, string showQuestionnaireType)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId)))
            {
                ShowQuestionnaireType = string.IsNullOrWhiteSpace(showQuestionnaireType) ? null : showQuestionnaireType
            };
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusQuestionnairePartial.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetConsensusUsersPartial(string consensusId)
        {
            ConsensusUsersDataOut data = consensusBLL.GetConsensusUsers(consensusId, userCookieData.ActiveOrganization);
            SetLastIterationStateViewBag(consensusId);
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsersPartial.cshtml", data);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult ProceedConsensus(ProceedConsensusDataIn proceedConsensusDataIn)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.ProceedIteration(proceedConsensusDataIn));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", formBLL.GetFormDataOutById(proceedConsensusDataIn.FormId, userCookieData));
        }

        public ActionResult ReloadConsensusTree(string consensusId, string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(consensusBLL.GetById(consensusId));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", formBLL.GetFormDataOutById(formId, userCookieData));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        public ActionResult AddQuestion(ConsensusQuestionDataIn questionDataIn)
        {
            Ensure.IsNotNull(questionDataIn, nameof(questionDataIn));
            consensusBLL.AddQuestion(questionDataIn);
            return GetConsensusTreePartial(questionDataIn.FormId);
        }

        [SReportsAuditLog]
        public ActionResult GetConsensusFormPreview(string formId)
        {
            Form form = formDAL.GetForm(formId);
            FormDataOut formDataOut = Mapper.Map<FormDataOut>(form);
            SetReadOnlyAndDisabledViewBag(true);
            SetViewBagAndMakeResetAndNeSectionHidden();
            return PartialView("~/Views/Form/FormPartial.cshtml", formDataOut);
        }

        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ReloadConsensusTree(string formId)
        {
            return GetConsensusTreePartial(formId);
        }
        
        [SReportsAuditLog]
        [HttpGet]
        public ActionResult ReloadConsensusInstanceTree(string formId, string consensusInstanceId, string iterationId, string questionnaireViewType)
        {
            ConsensusDataOut consensus = Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId));
            consensus.Iterations = consensus.Iterations.Where(x => x.Id == iterationId).ToList();

            if (!string.IsNullOrWhiteSpace(consensusInstanceId))
            {
                consensus.GetIterationById(iterationId).SetQuestionsValue(Mapper.Map<List<ConsensusQuestionDataOut>>(consensusInstanceDAL.GetById(consensusInstanceId).Questions));
            }
            
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut()
            {
                Consensus = consensus,
                ViewType = questionnaireViewType ?? "View"
            };
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        [HttpGet]
        public ActionResult GetTrackerData(string consensusId)
        {
            return PartialView("~/Views/Form/Consensus/Tracker/ConsensusTrackerPartial.cshtml", consensusBLL.GetTrackerData(consensusId));
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult GetUserHierarchy(string name, List<string> countries)
        {
            List<OrganizationUsersCountDataOut> data = Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(name, MapCountriesToCustomEnum(countries)));
            return PartialView("~/Views/Form/Consensus/Users/OrganizationHierarchy.cshtml", data);
        }
        
        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult CreateOutsideUser(OutsideUserDataIn userDataIn)
        {
            int userId = outsideUserDAL.InsertOrUpdate(Mapper.Map<Domain.Sql.Entities.OutsideUser.OutsideUser>(userDataIn));
            Consensus consensus = consensusDAL.GetById(userDataIn.ConsensusRef);
            if (!consensus.Iterations.Last().OutsideUserIds.Contains(userId)) 
            {
                consensus.Iterations.Last().OutsideUserIds.Add(userId);
            }

            consensusDAL.Insert(consensus);

            List<OutsideUserDataOut> users = Mapper.Map<List<OutsideUserDataOut>>(outsideUserDAL.GetAllByIds(consensus.Iterations.Last().OutsideUserIds));
            SetLastIterationStateViewBag(consensus.Id);
            return PartialView("~/Views/Form/Consensus/Users/OutsideUsers.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult ReloadUsers(List<int> organizationIds, string consensusId)
        {
            List<OrganizationUsersDataOut> result = new List<OrganizationUsersDataOut>();

            if (organizationIds != null) 
            {
                result = organizationBLL.GetUsersByOrganizationsIds(organizationIds);
            }
            SetLastIterationStateViewBag(consensusId);
            return PartialView("~/Views/Form/Consensus/Users/ConsensusUsers.cshtml", result);
        }


        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult DeleteOutsideUser(int userId, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().OutsideUserIds = consensus.Iterations.Last().OutsideUserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }


        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult DeleteInsideUser(int userId, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult SaveUsers(List<int> usersIds, string consensusId)
        {
            List<UserDataOut> users = consensusBLL.SaveUsers(usersIds, consensusId);
            SetLastIterationStateViewBag(consensusId);
            return PartialView("~/Views/Form/Consensus/Users/InsideUsers.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAuthorize(Permission = PermissionNames.FindConsensus, Module = ModuleNames.Designer)]
        [HttpPost]
        public ActionResult StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn)
        {
            if (consensusDAL.CanStartConsensusFindingProcess(dataIn.ConsensusId))
            {
                this.consensusBLL.StartConsensusFindingProcess(dataIn);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, Resources.TextLanguage.CF_iteration_stop);
            }
        }

        [SReportsAuditLog]
        public ActionResult CreateConsensusInstance(int userId, string consensusId, bool isOutsideUser, string iterationId)
        {
            return ShowQuestionnaire(userId, consensusId, isOutsideUser, iterationId, "Create", "~/Views/Form/Consensus/Instance/CreateConsensusInstance.cshtml");
        }

        [SReportsAuditLog]
        public ActionResult ShowUserQuestionnaire(int userId, string consensusId, bool isOutsideUser, string iterationId)
        {
            return ShowQuestionnaire(userId, consensusId, isOutsideUser, iterationId, "View", "~/Views/Form/Consensus/Instance/ShowUserQuestionnaire.cshtml");
        }

        [HttpPost]
        public ActionResult CreateConsensusInstance(ConsensusInstanceDataIn consensusInstance)
        {
            if (consensusBLL.CanSubmitConsensusInstance(consensusInstance))
            {
                ResourceCreatedDTO resourceCreatedDTO = consensusBLL.SubmitConsensusInstance(consensusInstance);

                return new JsonResult()
                {
                    Data = resourceCreatedDTO,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                SignOut();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
            }

        }

        [SReportsAuditLog]
        public ActionResult RemindUser(int userId, string consensusId, bool isOutsideUser, string iterationId)
        {
            consensusBLL.RemindUser(new List<int>() { userId }, consensusId, isOutsideUser, iterationId);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        private ActionResult GetConsensusTreePartial(string formId)
        {
            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut(Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId)));
            return PartialView("~/Views/Form/Consensus/Questionnaire/ConsensusFormTree.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }
        
        private ActionResult ShowQuestionnaire(int userId, string consensusId, bool isOutsideUser, string iterationId, string viewType, string viewName)
        {
            ConsensusInstance instance = consensusInstanceDAL.GetByConsensusAndUserAndIteration(consensusId.ToString(), userId, iterationId);
            Consensus consensus = consensusDAL.GetById(consensusId);

            ViewBag.ConsensusQuestionnaire = new ConsensusQuestionnaireDataOut()
            {
                Consensus = instance != null ? Mapper.Map<ConsensusDataOut>(instance) : Mapper.Map<ConsensusDataOut>(consensus),
                ConsensusInstanceId = instance != null ? instance.Id : string.Empty,
                IterationId = iterationId,
                IterationState = consensus.GetIterationById(iterationId).State ?? IterationState.NotStarted,
                UserId = userId,
                IsOutsideUser = isOutsideUser,
                ShowQuestionnaireType = "ConsensusInstance",
                Completed = instance != null ? instance.IsCompleted() : false,
                ViewType = viewType
            };

            return View(viewName, GetFormDataOut(formDAL.GetForm(consensus.FormRef)));
        }

        private void SetLastIterationStateViewBag(string consensusId)
        {
            ViewBag.CanEditConsensusUsers = consensusDAL.GetLastIterationState(consensusId) == IterationState.Design;
        }

        private Dictionary<int, string> MapCountriesToCustomEnum(List<string> countryNames)
        {
            Dictionary<int, string> countries = new Dictionary<int, string>();
            if (countryNames != null)
            {
                foreach (string countryName in countryNames)
                {
                    int customEnumId = SingletonDataContainer.Instance.GEtCustomEnumId(countryName, CustomEnumType.Country);
                    if (customEnumId > 0)
                    {
                        countries.Add(customEnumId, countryName);
                    }
                }
            }
            
            return countries;
        }
    }
}