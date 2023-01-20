using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class ConsensusBLL : IConsensusBLL
    {
        private readonly IConsensusDAL consensusDAL;
        private readonly IConsensusInstanceDAL consensusInstanceDAL;
        private readonly IOutsideUserDAL outsideUserDAL;
        private readonly IUserDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IEmailSender emailSender;
        private readonly ICustomEnumDAL customEnumDAL;

        public ConsensusBLL(IConsensusDAL consensusDAL, IConsensusInstanceDAL consensusInstanceDAL, IOutsideUserDAL outsideUserDAL, IUserDAL userDAL, IOrganizationDAL organizationDAL, IFormDAL formDAL, IEmailSender emailSender, ICustomEnumDAL customEnumDAL)
        {
            this.consensusDAL = consensusDAL;
            this.outsideUserDAL = outsideUserDAL;
            this.userDAL = userDAL;
            this.organizationDAL = organizationDAL;
            this.formDAL = formDAL;
            this.consensusInstanceDAL = consensusInstanceDAL;
            this.emailSender = emailSender;
            this.customEnumDAL = customEnumDAL;
        }

        public ResourceCreatedDTO StartNewIteration(string consensusId, string formId, int creatorId)
        {
            Consensus consensus;
            if (string.IsNullOrWhiteSpace(consensusId))
            {
                consensus = CreateConsensusAndStartIteration(formId, creatorId);
            }
            else
            {
                consensus = TryStartIteration(consensusId);
            }

            return new ResourceCreatedDTO()
            {
                Id = consensus?.Id,
                LastUpdate = consensus.LastUpdate.Value.ToString("o")
            };
        }

        public ResourceCreatedDTO TerminateCurrentIteration(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().State = IterationState.Terminated;
            consensusDAL.Insert(consensus);

            SendMailToConsensusCreator(consensus, IterationState.Terminated.ToString());

            return new ResourceCreatedDTO()
            {
                Id = consensus?.Id,
                LastUpdate = consensus.LastUpdate.Value.ToString("o")
            };
        }

        public void RemindUser(List<int> usersIds, string consensusId, bool isOutsideUsers, string iterationId)
        {
            string message = $"Dear participant,<br><br>" +
                $"This is a reminder concerning the {EmailSenderNames.SoftwareName} questionnaire you have been invited to participate in.<br>" +
                $"Please follow the link below to complete your {EmailSenderNames.SoftwareName} questionnaire:";
            SendMailsToUsers(usersIds, message, consensusId, isOutsideUsers, iterationId);
        }

        public void StartConsensusFindingProcess(ConsensusFindingProcessDataIn dataIn)
        {
            Consensus consensus = consensusDAL.GetById(dataIn.ConsensusId);
            ShouldConsensusFindingProcessStop(consensus);
            consensus.Iterations.Last().State = IterationState.InProgress;
            consensusDAL.Insert(consensus);

            SendMailToConsensusCreator(consensus, "In progress");
            SendMailsToUsers(dataIn.OutsideUsersIds, dataIn.EmailMessage, dataIn.ConsensusId, true, consensus.Iterations.Last().Id);
            SendMailsToUsers(dataIn.UsersIds, dataIn.EmailMessage, dataIn.ConsensusId, false, consensus.Iterations.Last().Id);
        }

        public ConsensusDataOut ProceedIteration(ProceedConsensusDataIn proceedConsensusDataIn)
        {
            Consensus consensus = consensusDAL.GetById(proceedConsensusDataIn.ConsensusId);
            if (consensus == null)
            {

            }
            ConsensusIteration consensusIteration = consensus.GetIterationById(proceedConsensusDataIn.IterationId);
            if (consensusIteration == null)
            {

            }

            consensusIteration.State = IterationState.Design;
            consensusIteration.QuestionOccurences = proceedConsensusDataIn.QuestionOccurences.Select(x =>
            new QuestionOccurenceConfig()
            {
                Level = x.Level,
                Type = x.Type
            })
            .ToList();

            consensusDAL.Insert(consensus);

            return Mapper.Map<ConsensusDataOut>(consensus);
        }

        public ConsensusDataOut GetById(string id)
        {
            return Mapper.Map<ConsensusDataOut>(consensusDAL.GetById(id));
        }

        public void AddQuestion(ConsensusQuestionDataIn consensusQuestionDataIn)
        {
            ConsensusQuestion question = Mapper.Map<ConsensusQuestion>(consensusQuestionDataIn);
            Consensus consensus = consensusDAL.GetByFormId(consensusQuestionDataIn.FormId);
            ConsensusIteration iteration = consensus.Iterations.Last();
            iteration.Questions.Add(question);
            RepeatQuestion(consensusQuestionDataIn, iteration);
            consensusDAL.Insert(consensus);
        }

        public ResourceCreatedDTO SubmitConsensusInstance(ConsensusInstanceDataIn consensusInstanceDataIn)
        {
            ConsensusInstance consensusInstance = Mapper.Map<ConsensusInstance>(consensusInstanceDataIn);
            if (!consensusDAL.IsLastIterationFinished(consensusInstanceDataIn.ConsensusRef))
            {
                consensusInstanceDAL.InsertOrUpdate(consensusInstance);
                NotifyConsensusCreatorIfIterationIsCompleted(consensusInstance);
            }
        
            return new ResourceCreatedDTO() { Id = consensusInstance.Id };
        }

        public TrackerDataOut GetTrackerData(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<ConsensusInstance> consensusInstances = consensusInstanceDAL.GetAllByConsensusId(consensusId);
            List<User> users = userDAL.GetAllByIds(consensus.GetAllUserIds(false));
            List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers = outsideUserDAL.GetAllByIds(consensus.GetAllUserIds(true));

            return new TrackerDataOut()
            {
                ConsensusId = consensusId,
                ActiveIterationId = consensus.Iterations.Last().Id,
                Iterations = GetIterationTrackerData(consensus, consensusInstances, users, outsideUsers)
            };
        }

        public List<UserDataOut> SaveUsers(List<int> usersIds, string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            consensus.Iterations.Last().UserIds.AddRange(usersIds);
            consensus.Iterations.Last().UserIds = consensus.Iterations.Last().UserIds.Distinct().ToList();
            consensusDAL.Insert(consensus);

            List<UserDataOut> users = GetInsideUsersByConsensus(consensus);

            return users;
        }

        public ConsensusUsersDataOut GetConsensusUsers(string consensusId, int activeOrganization)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);

            return new ConsensusUsersDataOut
            {
                ConsensusOrganizationUserInfoData = GetOrganizationUserInfo(activeOrganization),
                OrganizationUsersCount = Mapper.Map<List<OrganizationUsersCountDataOut>>(organizationDAL.GetOrganizationUsersCount(null, null)),
                Users = GetInsideUsersByConsensus(consensus),
                OutsideUsers = GetOutsideUsers(consensusId)
            };
        }

        public bool CanSubmitConsensusInstance(ConsensusInstanceDataIn consensusInstance)
        {
            bool canSubmit = true;
            ConsensusIteration currentIteration = consensusDAL.GetLastIteration(consensusInstance.ConsensusRef);
            if (!consensusInstance.IsOutsideUser)
            {
                User user = userDAL.GetById(consensusInstance.UserRef);
                if(!user.UserHasPermission(PermissionNames.FindConsensus, ModuleNames.Designer) || !currentIteration.UserIds.Contains(consensusInstance.UserRef)){
                    canSubmit = false;
                }
            }

            return canSubmit;
        }

        private Consensus CreateConsensusAndStartIteration(string formId, int creatorId)
        {
            Consensus consensus = new Consensus()
            {
                FormRef = formId,
                CreatorId = creatorId
            };

            consensus.State = ConsensusFindingState.OnGoing;
            consensus.Iterations = new List<ConsensusIteration>()
                {
                    new ConsensusIteration()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserIds = new List<int>(),
                        OutsideUserIds = new List<int>(),
                        Questions = new List<ConsensusQuestion>(),
                        State = IterationState.NotStarted,
                        EntryDatetime = DateTime.Now
                    }
                };
            consensusDAL.Insert(consensus);

            return consensus;
        }

        private void SendMailsToUsers(List<int> usersIds, string emailMessage, string consensusId, bool isOutsideUsers, string iterationId)
        {
            string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            if (usersIds != null)
            {
                foreach (int userId in usersIds)
                {
                    string url = $"{domain}/FormConsensus/CreateConsensusInstance?userId={userId}&consensusId={consensusId}&isOutsideUser={isOutsideUsers}&iterationId={iterationId}";
                    string email = isOutsideUsers ? outsideUserDAL.GetById(userId).Email : userDAL.GetById(userId).Email;
                    string htmlContent = $"{emailMessage} <br><br> <a href=\"{url}\">Consensus Finding</a>";
                    Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} consensus finding process", "", htmlContent, email));
                }
            }
        }

        private void SendMailToConsensusCreator(Consensus consensus, string state)
        {
            User creator = userDAL.GetById(consensus.CreatorId);
            if(creator != null)
            {
                Form form = formDAL.GetForm(consensus.FormRef);
                ConsensusIteration iteration = consensus.Iterations.Last();
                int iterationIndex = consensus.Iterations.IndexOf(iteration);
                List<User> users = userDAL.GetAllByIds(iteration.UserIds);
                List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers = outsideUserDAL.GetAllByIds(iteration.OutsideUserIds);
                string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string mailContentIntro = $"Dear {creator.FirstName} {creator.LastName},<br><br>Consensus finding process which you created for <b>{form.Title}</b> is <b><i>{state}</i></b>.";
                string mailContentBody = $"<table border=\"1\" style=\"width: 50%;\">" +
                    $"<tr><th>Consensus created on</th><td>{consensus.EntryDatetime.ToTimeZoned(creator.UserConfig.TimeZoneOffset, DateConstants.DateFormat)}</td></tr>" +
                    $"<tr><th>Iteration order number</th><td>{iterationIndex + 1}</td></tr>" +
                    $"<tr><th>Iteration started on</th><td>{iteration.EntryDatetime.ToTimeZoned(creator.UserConfig.TimeZoneOffset, DateConstants.DateFormat)}</td></tr>" +
                    $"<tr><th>Iteration users</th><td>{string.Join(", ", users.Select(x => string.Concat(x.FirstName, " ", x.LastName)))}</td></tr>" +
                    $"<tr><th>Iteration outside users</th><td>{string.Join(", ", outsideUsers.Select(x => string.Concat(x.FirstName, " ", x.LastName)))}</td></tr>" +
                    $"<tr><th>Iteration status</th><td>{state}</td></tr></table>";
                string url = $"{domain}/Form/Edit?thesaurusId={form.ThesaurusId}&versionId={form.Version.Id}&activeTab={"consensus"}";
                string mailContentUrl = $"For more information you can visit this <a href=\"{url}\">link</a>.";
                string mailContent = $"<div>" +
                    $"{mailContentIntro}<br><br>{mailContentBody}<br><br>{mailContentUrl}<br>" +
                    $"------------------------------------------------------------------------------" +
                    $"</div>";
                Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} consensus finding process", "", mailContent, creator.Email));
            }
        }

        private Consensus TryStartIteration(string consensusId)
        {
            Consensus consensus;
            if (consensusDAL.IsLastIterationFinished(consensusId))
            {
                string id = Guid.NewGuid().ToString();
                consensus = consensusDAL.GetById(consensusId);
                consensus.Iterations.Add(new ConsensusIteration()
                {
                    Id = id,
                    Questions = new List<ConsensusQuestion>(),
                    UserIds = consensus.Iterations.Last().UserIds,
                    OutsideUserIds = consensus.Iterations.Last().OutsideUserIds,
                    State = IterationState.NotStarted,
                    EntryDatetime = DateTime.Now
                });

                consensus.SetIterationsState(id);
                consensusDAL.Insert(consensus);
            }
            else
            {
                throw new IterationNotFinishedException();
            }

            return consensus;
        }

        private List<ConsensusQuestion> GetQuestions(ConsensusQuestionDataIn consensusQuestionDataIn, List<string> ids)
        {
            List<ConsensusQuestion> result = new List<ConsensusQuestion>();
            ids.ForEach(x =>
                                         {
                                             result.Add(new ConsensusQuestion()
                                             {
                                                 Comment = consensusQuestionDataIn.Comment,
                                                 ItemRef = x,
                                                 Options = consensusQuestionDataIn.Options,
                                                 Question = consensusQuestionDataIn.Question,
                                                 Value = consensusQuestionDataIn.Value
                                             });
                                         });

            return result;
        }

        private void RepeatQuestion(ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            QuestionOccurenceConfig questionOccurenceConfig = iteration.QuestionOccurences?.FirstOrDefault(x => x.Level == consensusQuestionDataIn.Level.ToString());
            if (questionOccurenceConfig != null)
            {
                if (questionOccurenceConfig.Type == QuestionOccurenceType.Same)
                {
                    Form form = this.formDAL.GetForm(consensusQuestionDataIn.FormId);
                    if (form == null)
                    {
                        throw new EntryPointNotFoundException();
                    }

                    switch (consensusQuestionDataIn.Level)
                    {
                        case FormItemLevel.Chapter:
                            RepeatChapterQuestion(form, consensusQuestionDataIn, iteration);
                            break;

                        case FormItemLevel.Page:
                            RepeatPageQuestion(form, consensusQuestionDataIn, iteration);
                            break;

                        case FormItemLevel.FieldSet:
                            RepeatFieldSetQuestion(form, consensusQuestionDataIn, iteration);
                            break;

                        case FormItemLevel.Field:
                            RepeatFieldQuestion(form, consensusQuestionDataIn, iteration);
                            break;

                        case FormItemLevel.FieldValue:
                            RepeatFieldValuesQuestion(form, consensusQuestionDataIn, iteration);
                            break;
                    }
                }
            }
        }

        private void RepeatChapterQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var chapters = form.Chapters
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, chapters));
        }

        private void RepeatPageQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var pages = form.Chapters
                .SelectMany(x => x.Pages)
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, pages));
        }

        private void RepeatFieldSetQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fieldSets = form.GetAllFieldSets()
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fieldSets));
        }

        private void RepeatFieldQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fields = form.GetAllFields()
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fields));
        }

        private void RepeatFieldValuesQuestion(Form form, ConsensusQuestionDataIn consensusQuestionDataIn, ConsensusIteration iteration)
        {
            var fieldValues = form.GetAllFields()
                .Where(x => x is FieldSelectable)
                .Select(x => x as FieldSelectable)
                .SelectMany(x => x.Values)
                .Select(x => x.Id)
                .Where(x => !x.Equals(consensusQuestionDataIn.ItemRef))
                .ToList();
            iteration.Questions.AddRange(GetQuestions(consensusQuestionDataIn, fieldValues));
        }

        private bool ShouldConsensusFindingProcessStop(Consensus consensus)
        {
            ConsensusIteration currentConsensusIteration = consensus.Iterations.Last();
            List<QuestionOccurenceConfig> questionOccurenceConfigList = currentConsensusIteration.QuestionOccurences;
            if(questionOccurenceConfigList != null)
            {
                foreach (QuestionOccurenceConfig questionOccurenceConfig in questionOccurenceConfigList)
                {
                    QuestionOccurenceType occurenceType = questionOccurenceConfig.Type;
                    if (occurenceType == QuestionOccurenceType.Different || occurenceType == QuestionOccurenceType.Same)
                    {
                        Form form = this.formDAL.GetForm(consensus.FormRef);

                        string occurenceLevel = questionOccurenceConfig.Level;
                        switch ((FormItemLevel)Enum.Parse(typeof(FormItemLevel), occurenceLevel))
                        {
                            case FormItemLevel.Form:
                                List<string> formIds = new List<string> { form.Id };
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, formIds)) throw new ConsensusCannotStartException(FormItemLevel.Form);
                                break;
                            case FormItemLevel.Chapter:
                                List<string> chapterIds = form.Chapters.Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, chapterIds)) throw new ConsensusCannotStartException(FormItemLevel.Chapter);
                                break;

                            case FormItemLevel.Page:
                                List<string> pageIds = form.GetAllPages().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, pageIds)) throw new ConsensusCannotStartException(FormItemLevel.Page);
                                break;

                            case FormItemLevel.FieldSet:
                                List<string> fieldSetIds = form.GetAllFieldSets().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldSetIds)) throw new ConsensusCannotStartException(FormItemLevel.FieldSet);
                                break;

                            case FormItemLevel.Field:
                                List<string> fieldIds = form.GetAllFields().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldIds)) throw new ConsensusCannotStartException(FormItemLevel.Field);
                                break;

                            case FormItemLevel.FieldValue:
                                List<string> fieldValueIds = form.GetAllFieldValues().Select(x => x.Id).ToList();
                                if (!AllQuestionsCoveredFor(currentConsensusIteration, fieldValueIds)) throw new ConsensusCannotStartException(FormItemLevel.FieldValue);
                                break;
                        }
                    }
                }
            }
            
            return false;
        }

        private bool AllQuestionsCoveredFor(ConsensusIteration consensusIteration, List<string> formElementIds)
        {
            int numOfMissingQuestions = formElementIds.Where(itemRefId => !consensusIteration.Questions.Any(q => q.ItemRef.Equals(itemRefId))).Count();
            return numOfMissingQuestions == 0;
        }

        private void NotifyConsensusCreatorIfIterationIsCompleted(ConsensusInstance consensusInstance)
        {
            List<ConsensusInstance> consensusInstances = consensusInstanceDAL.GetAllByConsensusAndIteration(consensusInstance.ConsensusRef, consensusInstance.IterationId);
            Consensus consensus = consensusDAL.GetById(consensusInstance.ConsensusRef);
            ConsensusIteration consensusIteration = consensus.GetIterationById(consensusInstance.IterationId);
            bool allQuestionnairesInitialized = consensusInstances.Count == consensusIteration.OutsideUserIds.Count + consensusIteration.UserIds.Count;
            bool allQuestionnairesCompleted = consensusInstances.All(c => c.GetPercentDone() == 100.00);
            if (allQuestionnairesInitialized && allQuestionnairesCompleted)
            {
                SendMailToConsensusCreator(consensus, IterationState.Finished.ToString());
                consensusIteration.State = IterationState.Finished;
                consensusDAL.Insert(consensus);
            }
        }

        private List<IterationTrackerDataOut> GetIterationTrackerData(Consensus consensus, List<ConsensusInstance> consensusInstances, List<User> insideUsers, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers)
        {
            List<IterationTrackerDataOut> result = new List<IterationTrackerDataOut>();
            foreach (ConsensusIteration iteration in consensus.Iterations.Where(x => x.State != IterationState.NotStarted && x.State != IterationState.Design).ToList())
            {
                IterationTrackerDataOut iterationTracker = GetSingleIterationTracker(consensus.Id, iteration, insideUsers, outsideUsers, consensusInstances);
                result.Add(iterationTracker);
            }

            return result;
        }

        private IterationTrackerDataOut GetSingleIterationTracker(string consensusId, ConsensusIteration iteration, List<User> insideUsers, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers, List<ConsensusInstance> consensusInstances)
        {
            IterationTrackerDataOut iterationTracker = new IterationTrackerDataOut(iteration.Id);
            iterationTracker.State = iteration.State;

            GetSingleIterationInstancesTrackerForInsideUsers(iterationTracker, consensusId, iteration, insideUsers, consensusInstances);
            GetSingleIterationInstancesTrackerForOutsideUsers(iterationTracker, consensusId, iteration, outsideUsers, consensusInstances);

            return iterationTracker;
        }

        private void GetSingleIterationInstancesTrackerForInsideUsers(IterationTrackerDataOut iterationTracker, string consensusId, ConsensusIteration iteration, List<User> insideUsers, List<ConsensusInstance> consensusInstances)
        {
            foreach (int userId in iteration.UserIds)
            {
                User user = insideUsers.FirstOrDefault(x => x.UserId == userId);
                ConsensusInstance consensusInstance = consensusInstances.FirstOrDefault(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(user.UserId, $"{user.FirstName} {user.LastName}", false, consensusInstance));
            }
        }

        private void GetSingleIterationInstancesTrackerForOutsideUsers(IterationTrackerDataOut iterationTracker, string consensusId, ConsensusIteration iteration, List<Domain.Sql.Entities.OutsideUser.OutsideUser> outsideUsers, List<ConsensusInstance> consensusInstances)
        {
            foreach (int userId in iteration.OutsideUserIds)
            {
                Domain.Sql.Entities.OutsideUser.OutsideUser outsideUser = outsideUsers.FirstOrDefault(x => x.OutsideUserId == userId);
                ConsensusInstance consensusInstance = consensusInstances.FirstOrDefault(x => x.UserId == userId && x.ConsensusRef == consensusId.ToString() && x.IterationId == iteration.Id);
                iterationTracker.Instances.Add(GetSingleInstanceTracker(outsideUser.OutsideUserId, $"{outsideUser.FirstName} {outsideUser.LastName}", true, consensusInstance));
            }
        }

        private ConsensusInstanceTrackerDataOut GetSingleInstanceTracker(int userId, string userName, bool isOutsideUser, ConsensusInstance consensusInstance)
        {
            double percentDone = consensusInstance != null ? consensusInstance.GetPercentDone() : 0;

            return new ConsensusInstanceTrackerDataOut(userId, userName, isOutsideUser, consensusInstance?.EntryDatetime, consensusInstance?.LastUpdate, percentDone);
        }

        private List<OutsideUserDataOut> GetOutsideUsers(string consensusId)
        {
            Consensus consensus = consensusDAL.GetById(consensusId);
            List<OutsideUserDataOut> outsideUsers = Mapper.Map<List<OutsideUserDataOut>>(outsideUserDAL.GetAllByIds(consensus.Iterations.Last().OutsideUserIds));
            return outsideUsers;
        }

        private ConsensusOrganizationUserInfoDataOut GetOrganizationUserInfo(int activeOrganization)
        {
            int? countryId = organizationDAL.GetById(activeOrganization)?.Address?.CountryId;
            ConsensusOrganizationUserInfoDataOut data = new ConsensusOrganizationUserInfoDataOut()
            {
                UsersCount = (int)userDAL.GetAllCount(),
                OrganizationsCount = (int)organizationDAL.GetAllCount(),
                OrganizationsCountByState = (int)organizationDAL.GetAllEntriesCountByCountry(countryId.GetValueOrDefault())
            };
            return data;
        }

        private List<UserDataOut> GetInsideUsersByConsensus(Consensus consensus)
        {
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(consensus.Iterations.Last().UserIds));
            List<int> organizationsIds = GetOrganizationsIds(users);
            List<OrganizationDataOut> organizations = Mapper.Map<List<OrganizationDataOut>>(organizationDAL.GetByIds(organizationsIds));
            SetOrganizationsForUsers(users, organizations);

            return users;
        }

        private List<int> GetOrganizationsIds(List<UserDataOut> users)
        {
            List<int> result = new List<int>();
            foreach (var user in users)
            {
                result.AddRange(user.GetOrganizationRefs());
            }

            return result.Distinct().ToList();
        }

        private void SetOrganizationsForUsers(List<UserDataOut> users, List<OrganizationDataOut> organizations)
        {
            foreach (var user in users)
            {
                foreach (var organization in user.GetNonArchivedOrganizations())
                {
                    organization.Organization = organizations.FirstOrDefault(x => x.Id == organization.OrganizationId);
                }
            }
        }
    }
}
