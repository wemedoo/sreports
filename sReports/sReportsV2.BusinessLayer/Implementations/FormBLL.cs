﻿using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.Domain.Entities.FieldEntity;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FormBLL : IFormBLL
    {
        private readonly IUserDAL userDAL;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IThesaurusDAL thesaurusDAL;

        public FormBLL(IUserDAL userDAL, IOrganizationDAL organizationDAL, IFormDAL formDAL, IThesaurusDAL thesaurusDAL)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.formDAL = formDAL;
            this.thesaurusDAL = thesaurusDAL;
        }

        public Form GetFormByThesaurusAndLanguage(int thesaurusId, string language)
        {
            return this.formDAL.GetFormByThesaurusAndLanguage(thesaurusId, language);
        }

        public Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId)
        {
           return this.formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, organizationId, activeLanguage, versionId);
        }

        public FormDataOut GetFormDataOutById(string formId, UserCookieData userCookieData)
        {
            Form form = formDAL.GetForm(formId);
            return GetFormDataOut(form, userCookieData);
        }

        public FormDataOut GetFormDataOut(Form form, DTOs.User.DTO.UserCookieData userCookieData)
        {
            FormDataOut dataOut = Mapper.Map<FormDataOut>(form);
            dataOut.User = Mapper.Map<UserDataOut>(userCookieData);
            dataOut.Organization = Mapper.Map<OrganizationDataOut>(Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(form.OrganizationId)));
            if (form.WorkflowHistory != null)
            {
                dataOut.WorkflowHistory = new List<FormStatusDataOut>();
                foreach (FormStatus status in form.WorkflowHistory)
                {
                    dataOut.WorkflowHistory.Add(new FormStatusDataOut()
                    {
                        Created = status.Created,
                        Status = status.Status,
                        User = Mapper.Map<UserDataOut>(userDAL.GetById(status.UserId))
                    });
                }
            }
            return dataOut;
        }

        public List<FormInstancePerDomainDataOut> GetFormInstancePerDomain()
        {
            return Mapper.Map<List<FormInstancePerDomainDataOut>>(this.formDAL.GetFormInstancePerDomain());
        }
       
        public PaginationDataOut<FormDataOut, FormFilterDataIn> ReloadData(FormFilterDataIn dataIn, UserCookieData userCookieData)
        {
            FormFilterData filterData = GetFormFilterData(dataIn, userCookieData);
            PaginationDataOut<FormDataOut, FormFilterDataIn> result = new PaginationDataOut<FormDataOut, FormFilterDataIn>()
            {
                Count = (int)this.formDAL.GetAllFormsCount(filterData),
                Data = Mapper.Map<List<FormDataOut>>(this.formDAL.GetAll(filterData)),
                DataIn = dataIn
            };
            return result;
        }

        public FormDataOut SetFormDependablesAndReferrals(Form form, List<Form> referrals)
        {
            FormDataOut data = Mapper.Map<FormDataOut>(form);
            data.SetDependables();
            if (referrals != null && referrals.Count() > 0) 
            {
                data.ReferrableFields = GetReferrableFields(form, referrals);

            }

            return data;
        }

        private List<ReferralInfoDTO> GetReferrableFields(Form form, List<Form> referrals)
        {
            List<ReferralInfoDTO> result = new List<ReferralInfoDTO>();

            if (referrals != null)
            {
                result = MapReferralInfo(form.GetValuesFromReferrals(referrals));
            }
            return result;
        }

        private List<ReferralInfoDTO> MapReferralInfo(List<ReferalInfo> referrals)
        {
            List<ReferralInfoDTO> result = new List<ReferralInfoDTO>();
            
            foreach(ReferalInfo referralInfo in referrals)
            {
                User user = userDAL.GetById(referralInfo.UserId);
                Organization organization = organizationDAL.GetById(referralInfo.OrganizationId);
                
                ReferralInfoDTO referralInfoDTO = Mapper.Map<ReferralInfoDTO>(referralInfo);
                referralInfoDTO.User = user != null ? new UserDataOut { FirstName = user.FirstName, LastName = user.LastName } : new UserDataOut();
                referralInfoDTO.Organization = organization != null ? new OrganizationDataOut { Name = organization.Name } : new OrganizationDataOut();
                result.Add(referralInfoDTO);
            }

            return result;
        }

        private FormFilterData GetFormFilterData(FormFilterDataIn formDataIn, UserCookieData userCookieData)
        {
            FormFilterData result = Mapper.Map<FormFilterData>(formDataIn);
            result.OrganizationId = (userCookieData.GetActiveOrganizationData()?.Id).GetValueOrDefault();
            result.ActiveLanguage = userCookieData.ActiveLanguage;
            return result;
        }

        public Form GetForm(FormInstance formInstance, UserCookieData userCookieData)
        {
            Form form = null;
            if (!formInstance.Language.Equals(userCookieData.ActiveLanguage))
            {
                form = this.formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formInstance.ThesaurusId, (userCookieData.GetActiveOrganizationData()?.Id).GetValueOrDefault(), userCookieData.ActiveLanguage, formInstance.Version.Id);
            }
            if (form == null)
            {
                form = this.formDAL.GetForm(formInstance.FormDefinitionId);
            }


            return new Form(formInstance, form);
        }

        public FormDataOut GetFormDataOut(FormInstance formInstance, List<FormInstance> referrals, UserCookieData userCookieData)
        {
            Form form = this.GetForm(formInstance, userCookieData);
            FormDataOut data = this.SetFormDependablesAndReferrals(form, GetFormsFromReferrals(referrals));
            
            data.LastUpdate = formInstance.LastUpdate;
            data.User = Mapper.Map<UserDataOut>(userCookieData);

            return data;
        }

        public List<Form> GetFormsFromReferrals(List<FormInstance> referrals)
        {
            List<Form> forms = new List<Form>();
            foreach (FormInstance referral in referrals)
            {
                Form form = formDAL.GetForm(referral.FormDefinitionId);
                form.SetFields(referral.Fields);
                form.Id = referral.Id;
                form.UserId = referral.UserId;
                form.OrganizationId = referral.OrganizationId;
                forms.Add(form);
            }
            return forms;
        }

        public Form GetFormById(string formId)
        {
            return formDAL.GetForm(formId);
        }

        public TreeDataOut GetTreeDataOut(int thesaurusId, int thesaurusPageNum, string searchTerm, UserCookieData userCookieData = null)
        {
            var forms = formDAL.GetFilteredDocumentsByThesaurusAppeareance(thesaurusId, searchTerm, thesaurusPageNum, userCookieData?.ActiveOrganization);
            TreeDataOut result = new TreeDataOut()
            {
                Forms = Mapper.Map<List<FormTreeDataOut>>(forms),
                O4MtId = thesaurusId
            };

            foreach (FormTreeDataOut form in result.Forms)
            {
                form.ThesaurusAppearances = forms.FirstOrDefault(x => x.Id == form.Id).GetAllThesaurusIds().Where(t => t == thesaurusId).Count();
            }

            return result;
        }

        public bool TryGenerateNewLanguage(string formId, string language, UserCookieData userCookieData)
        {
            Form form = formDAL.GetForm(formId);
            if (form == null)
            {
                return false;
            }
            List<int> thesaurusList = form.GetAllThesaurusIds();
            UserData userData = Mapper.Map<UserData>(userCookieData);
            List<ThesaurusEntry> entries = thesaurusDAL.GetByIdsList(thesaurusList);
            if (entries.Count.Equals(0))
            {
                form.Id = null;
                form.Language = language;
                formDAL.InsertOrUpdate(form, userData, false);
            }
            else
            {
                form.Id = null;
                form.GenerateTranslation(entries, language);
                formDAL.InsertOrUpdate(form, userData, false);
            }

            return true;
        }

        public void DisableActiveFormsIfNewVersion(Form form, UserCookieData userCookieData)
        {
            if (!string.IsNullOrEmpty(form.Id))
            {
                Form formFromDatabase = formDAL.GetForm(form.Id);

                if (form.IsVersionChanged(formFromDatabase))
                {
                    form.Id = null;
                    form.Version.Id = Guid.NewGuid().ToString();
                    //set all common form state to disabled
                    formDAL.DisableFormsByThesaurusAndLanguageAndOrganization(formFromDatabase.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage);
                }
            }
        }

        public ResourceCreatedDTO UpdateFormState(UpdateFormStateDataIn updateFormStateDataIn, UserCookieData userCookieData)
        {
            Form form = formDAL.GetForm(updateFormStateDataIn.Id);
            if(form == null)
            {
                throw new EntryPointNotFoundException();
            }

            form.State = updateFormStateDataIn.State;
            formDAL.InsertOrUpdate(form, Mapper.Map<UserData>(userCookieData));

            return new ResourceCreatedDTO()
            {
                Id = form.Id,
                LastUpdate = form.LastUpdate.Value.ToString("o")
            };
        }

        public List<FieldDataOut> GetPlottableFields(string formId)
        {
            List<FieldDataOut> fieldsDataOut = new List<FieldDataOut>();
            List<BsonDocument> bsonFields = formDAL.GetPlottableFields( formId);

            List<Field> fields = new List<Field>();
            BsonElement fieldsDict;

            if (bsonFields != null && bsonFields.Count > 0)
            {
                foreach (var bson in bsonFields)
                {
                    if (bson.TryGetElement("Fields", out fieldsDict))
                    {
                        fields.Add(BsonSerializer.Deserialize<Field>((BsonDocument)fieldsDict.Value));
                    }
                }
            }

            fieldsDataOut = Mapper.Map<List<FieldDataOut>>(fields);
            return fieldsDataOut;
        }

    }
}
