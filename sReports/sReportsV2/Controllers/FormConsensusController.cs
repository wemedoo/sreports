using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class FormConsensusController : FormCommonController
    {
        protected readonly IConsensusInstanceService consensusInstanceService;
        protected readonly Domain.Services.Interfaces.IConsensusDAL consensusDAL;

        private readonly IOutsideUserDAL outsideUserDAL;
        private readonly IUserDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        public ILogger Logger;


        public FormConsensusController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL,IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL, IOutsideUserDAL outsideUserDAL, IUserDAL userDAL, IOrganizationDAL organizationDAL, IConsensusDAL consensusDAL) : base(patientDAL, episodeOfCareDAL, encounterDAL,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {
            this.outsideUserDAL = outsideUserDAL;
            consensusInstanceService = new ConsensusInstanceService();
            this.consensusDAL = new Domain.Services.Implementations.ConsensusDAL();
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpGet]

        public ActionResult GetMapObject()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var obj = System.IO.File.ReadAllText($@"{path}\App_Data\countries-50m.json");
            return Content(obj);
        }
        [SReportsAuditLog]
        [SReportsAutorize]

        public ActionResult StartNewIteration(string consensusId)
        {
            if(consensusDAL.IsLastIterationFinished(consensusId))
            {
                Consensus consensus = consensusDAL.GetById(consensusId);
                consensus.Iterations.Add(new ConsensusIteration()
                {
                    Id = Guid.NewGuid().ToString(),
                    Questions = new List<ConsensusQuestion>(),
                    UserIds = consensus.Iterations.Last().UserIds,
                    OutsideUserIds = consensus.Iterations.Last().OutsideUserIds,
                    State = IterationState.NotStarted
                });

                consensus.SetIterationsState();
                consensusDAL.Insert(consensus);
                ViewBag.Consensus = Mapper.Map<ConsensusDataOut>(consensus);
                return PartialView("~/Views/Form/Consensus/ConsensusPartial.cshtml", GetFormDataOut(formDAL.GetForm(consensus.FormRef)));
            }
            else 
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Current iteration is not finished!");
            }
        }

        [SReportsAuditLog]
        [SReportsAutorize]

        public ActionResult LoadConsensusPartial(string formId)
        {
            Consensus consensus = consensusDAL.GetByFormId(formId);
            ViewBag.Consensus = Mapper.Map<ConsensusDataOut>(consensus);
            return PartialView("~/Views/Form/Consensus/ConsensusPartial.cshtml", GetFormDataOut(formDAL.GetForm(formId)));
        }

        
        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult ProceedConsensus(List<ConsensusDefinitionParameter> parameters, string formId)
        {
           var consensus = new Consensus()
            {
                FormRef = formId
            };

            consensusDAL.Insert(consensus);

            return GetConsensusTreePartialResult(formId);
        }
        

        [SReportsAuditLog]
        [HttpPost]

        public ActionResult AddQuestion(ConsensusQuestionDataIn questionDataIn, string formId, string iterationId)
        {
            consensusDAL.InsertQuestion(Mapper.Map<ConsensusQuestion>(questionDataIn), formId, iterationId);
            return GetConsensusTreePartialResult(formId);
        }


        [SReportsAuditLog]

        public ActionResult GetConsensusFormPreview(string formId)
        {
            Form form = formDAL.GetForm(formId);
            FormDataOut formDataOut = Mapper.Map<FormDataOut>(form);
            return PartialView("~/Views/Form/FormPartial.cshtml", formDataOut);
        }


        [SReportsAuditLog]

        public ActionResult GetOutsideUsers(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<OutsideUserDataOut> users = Mapper.Map<List<OutsideUserDataOut>>(outsideUserDAL.GetAllByIds(consensus.Iterations.Last().OutsideUserIds));
            return PartialView("~/Views/Form/Consensus/OutsideUsers.cshtml", users);
        }
        
        [SReportsAuditLog]

        public ActionResult GetInsideUsers(string consensusId)
        {
            return GetInsideUsersByConsensus(consensusDAL.GetById(consensusId));
        }
        
        private void SetOrganizationsForUsers(List<UserDataOut> users, List<OrganizationDataOut> organizations)
        {
            foreach (var user in users)
            {
                foreach (var organization in user.Organizations)
                {
                    organization.Organization = organizations.FirstOrDefault(x => x.Id == organization.OrganizationId);
                }
            }
        }
        

        [SReportsAuditLog]
        [HttpGet]

        public ActionResult ReloadConsensusTree(string formId)
        {
            return GetConsensusTreePartialResult(formId);
        }
        
        [SReportsAuditLog]
        [HttpGet]

        public ActionResult ReloadConsensusInstanceTree(string formId, string consensusInstanceId, string iterationId)
        {
            Form form = formDAL.GetForm(formId);
            Consensus consensus = consensusDAL.GetByFormId(formId);
            ConsensusDataOut dataOut = Mapper.Map<ConsensusDataOut>(consensus);
            dataOut.Iterations = dataOut.Iterations.Where(x => x.Id == iterationId).ToList();
            if (!string.IsNullOrWhiteSpace(consensusInstanceId))
            {
                dataOut.Iterations.FirstOrDefault(x => x.Id == iterationId).SetQuestionsValue(Mapper.Map<List<ConsensusQuestionDataOut>>(consensusInstanceService.GetById(consensusInstanceId).Questions));
            }

            ViewBag.Consensus = dataOut;

            return PartialView("~/Views/Form/Consensus/ConsensusFormTree.cshtml", GetFormDataOut(form));
        }



        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpGet]

        public ActionResult GetTrackerData(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<ConsensusInstance> consensusInstances = consensusInstanceService.GetAllByConsensusId(consensusId);
            List<User> users = userDAL.GetAllByIds(consensus.GetAllUserIds(false));
            List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers = outsideUserDAL.GetAllByIds(consensus.GetAllUserIds(true));

            List<IterationTrackerDataOut> data = GetIterationTrackerData(consensus, consensusInstances, users, outsideUsers);
            return PartialView("~/Views/Form/Consensus/TrackerPartial.cshtml", data);
        }

        private List<IterationTrackerDataOut> GetIterationTrackerData(Consensus consensus, List<ConsensusInstance> consensusInstances, List<User> users, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers)
        {
            List<IterationTrackerDataOut> result = new List<IterationTrackerDataOut>();
            foreach(ConsensusIteration iteration in consensus.Iterations.Where(x => x.State == IterationState.Finished).ToList())
            {
                IterationTrackerDataOut iterationTracker = GetSingleIterationTracker(consensus.Id, iteration, users, outsideUsers, consensusInstances);
                result.Add(iterationTracker);
            }

            return result;
        }

        private IterationTrackerDataOut GetSingleIterationTracker(string consensusId, ConsensusIteration iteration, List<User> users, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers, List<ConsensusInstance> consensusInstances) 
        {
            IterationTrackerDataOut iterationTracker = new IterationTrackerDataOut(iteration.Id);

            foreach (int userId in iteration.UserIds)
            {
                User user = users.FirstOrDefault(x => x.Id == userId);
                ConsensusInstance consensusInstance = consensusInstances.FirstOrDefault(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(user.Id, $"{user.FirstName} {user.LastName}", false, consensusInstance));

            }

            foreach (int userId in iteration.OutsideUserIds)
            {
                Domain.Sql.Entities.OutsideUser.OutsideUser outsideUser = outsideUsers.FirstOrDefault(x => x.Id == userId);
                ConsensusInstance consensusInstance = consensusInstances.FirstOrDefault(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(outsideUser.Id, $"{outsideUser.FirstName} {outsideUser.LastName}", true, consensusInstance));
            }

            return iterationTracker;
        }

        private ConsensusInstanceTrackerDataOut GetSingleInstanceTracker(int userId, string userName, bool isOutsideUser, ConsensusInstance consensusInstance) 
        {
            double percentDone = consensusInstance != null ? consensusInstance.GetPercentDone() : 0;

            return new ConsensusInstanceTrackerDataOut(userId, userName, isOutsideUser, consensusInstance?.EntryDatetime, consensusInstance?.LastUpdate, percentDone);
        }
        
        [SReportsAuditLog]
        //[SReportsAutorize]
        [HttpGet]

        public ActionResult GetOrganizationUserInfo()
        {
            string country = organizationBLL.GetOrganizationById(userCookieData.ActiveOrganization)?.Address?.Country;
            ConsensusOrganizationUserInfoDataOut data = new ConsensusOrganizationUserInfoDataOut()
            {
                UsersCount = (int)userBLL.GetAllCount(),
                OrganizationsCount = (int)organizationBLL.GetAllCount(),
                OrganizationsCountByState = !string.IsNullOrWhiteSpace(country) ? (int)organizationBLL.GetAllEntriesCountByCountry(country) : 0
            };
            return PartialView("~/Views/Form/Consensus/ConsensusOrganizationUserInfo.cshtml", data);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult GetUserHierarchy(string name, List<string> countries)
        {
            List<OrganizationUsersCountDataOut> data = Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationBLL.GetOrganizationUsersCount(name, countries));
            return PartialView("~/Views/Form/Consensus/OrganizationHierarchy.cshtml", data);
        }
        
        [SReportsAuditLog]
        [SReportsAutorize]
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
            return PartialView("~/Views/Form/Consensus/OutsideUsers.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult ReloadUsers(List<int> organizationIds)
        {
            List<OrganizationUsersDataOut> result = new List<OrganizationUsersDataOut>();

            if (organizationIds != null && organizationIds.Count > 0) 
            {
                List<OrganizationDataOut> organizations = Mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(organizationIds));
                List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByOrganizationIds(organizationIds));

                foreach (int organizationId in organizationIds)
                {
                    result.Add(new OrganizationUsersDataOut()
                    {
                        Id = organizationId,
                        Name = organizations.FirstOrDefault(x => x.Id == organizationId).Name,
                        Users = users.Where(x => x.Organizations.Any(o => o.OrganizationId == organizationId)).ToList()
                    });
                }
            }

            return PartialView("~/Views/Form/Consensus/ConsensusUsers.cshtml", result);
        }


        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult DeleteOutsideUser(int userId, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().OutsideUserIds = consensus.Iterations.Last().OutsideUserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }


        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult DeleteInsideUser(int userId, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Where(x => x != userId).ToList();
            consensusDAL.Insert(consensus);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult SaveUsers(List<int> usersIds, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds.AddRange(usersIds);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Distinct().ToList();
            consensusDAL.Insert(consensus);

            return GetInsideUsersByConsensus(consensus);
        }

        private ActionResult GetInsideUsersByConsensus(Consensus consensus) 
        {
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(consensus.Iterations.Last().UserIds));
            List<int> organizationsIds = GetOrganizationsIds(users);
            List<OrganizationDataOut> organizations = Mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(organizationsIds));
            SetOrganizationsForUsers(users, organizations);

            return PartialView("~/Views/Form/Consensus/InsideUsers.cshtml", users);
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        [HttpPost]

        public ActionResult StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn)
        {
            if (!consensusDAL.IsLastIterationFinished(dataIn.ConsensusId))
            {
                Consensus consensus = consensusDAL.GetById(dataIn.ConsensusId);
                consensus.Iterations.Last().State = IterationState.Finished;
                consensusDAL.Insert(consensus);

                SendMailsToUsers(dataIn.OutsideUsersIds, dataIn.EmailMessage, dataIn.ConsensusId, true, consensus.Iterations.Last().Id);
                SendMailsToUsers(dataIn.UsersIds, dataIn.EmailMessage, dataIn.ConsensusId, false, consensus.Iterations.Last().Id);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            else {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "You alredy finished current iteration!");
            }
        }

        

        [SReportsAuditLog]
        public ActionResult CreateConsensusInstance(int userId, string consensusId, bool isOutsideUser, string iterationId)
        {
            ViewBag.UserId = userId;
            ViewBag.ConsensusId = consensusId;
            ViewBag.IsOutsideUser = isOutsideUser;
            ConsensusInstance instance = consensusInstanceService.GetByConsensusAndUserAndIteration(consensusId.ToString(), userId, iterationId);
            Consensus consensus = consensusDAL.GetById(consensusId);
            ViewBag.Consensus = instance != null ? Mapper.Map<ConsensusDataOut>(instance) : Mapper.Map<ConsensusDataOut>(consensus);
            ViewBag.ConsensusInstanceId = instance != null ? instance.Id : string.Empty;
            ViewBag.IterationId = iterationId;
            return View("~/Views/Form/Consensus/CreateConsensusInstance.cshtml",GetFormDataOut(formDAL.GetForm(consensus.FormRef)));
        }

        [HttpPost]

        public ActionResult CreateConsensusInstance(ConsensusInstanceDataIn consensusInstance)
        {
            consensusInstanceService.InsertOrUpdate(Mapper.Map<ConsensusInstance>(consensusInstance));

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [SReportsAuditLog]
        public ActionResult RemindUser(int userId, string consensusId, bool isOutsideUser, string iterationId)
        {
            SendMailsToUsers(new List<int>() { userId }, "Reminder message", consensusId, isOutsideUser, iterationId);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        private void SendMailsToUsers(List<int> usersIds, string emailMessage, string consensusId, bool isOutsideUsers, string iterationId) 
        {
            string domain = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);

            if (usersIds != null) 
            {
                foreach (int userId in usersIds)
                {
                    string url = $"{domain}/FormConsensus/CreateConsensusInstance?userId={userId}&consensusId={consensusId}&isOutsideUser={isOutsideUsers}&iterationId={iterationId}";
                    string email = isOutsideUsers ? outsideUserDAL.GetById(userId).Email : userDAL.GetById(userId).Email;

                    Task.Run(() => EmailSender.SendAsync("sReports consensus finding process", "",
                    $"{emailMessage} </br> <a href=\"{url}\">FORM</a>", email));
                }
            }

        }

        private ActionResult GetConsensusTreePartialResult(string formId)
        {
            Form form = formDAL.GetForm(formId);
            ViewBag.Consensus = Mapper.Map<ConsensusDataOut>(consensusDAL.GetByFormId(formId));
            return PartialView("~/Views/Form/Consensus/ConsensusFormTree.cshtml", GetFormDataOut(form));

        }
        
        private List<int> GetOrganizationsIds(List<UserDataOut> users) 
        {
            List<int> result = new List<int>();
            foreach (var user in users) 
            {
                result.AddRange(user.Organizations.Select(x => x.OrganizationId));
            }

            return result.Distinct().ToList();
        }
    }
}