using AutoMapper;
using MongoDB.Bson;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FormInstanceBLL : IFormInstanceBLL
    {
        private readonly IUserDAL userDAL;
        private readonly IPatientDAL patientDAL;
        private readonly IFormInstanceDAL formInstanceDAL;

        public FormInstanceBLL(IUserDAL userDAL, IFormInstanceDAL formInstanceDAL, IPatientDAL patientDAL)
        {
            this.userDAL = userDAL;
            this.patientDAL = patientDAL;
            this.formInstanceDAL = formInstanceDAL;
        }
        public PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn> ReloadData(FormInstanceFilterDataIn dataIn)
        {
            FormInstanceFilterData filterData = Mapper.Map<FormInstanceFilterData>(dataIn);

            PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn> result = new PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn>()
            {
                Count = (int)this.formInstanceDAL.GetAllInstancesByThesaurusCount(filterData),
                Data = LoadFormInstancesDataOut(filterData),
                DataIn = dataIn
            };            

            return result;
        }
        public string InsertOrUpdate(FormInstance form, FormInstanceStatus formInstanceStatus)
        {
            return formInstanceDAL.InsertOrUpdate(form, formInstanceStatus);
        }

        public FormInstance GetById(string id)
        {
            return formInstanceDAL.GetById(id);
        }

        public List<FormInstance> GetByIds(List<string> ids)
        {
            return formInstanceDAL.GetByIds(ids).ToList();
        }

        public void Delete(string formInstanceId, DateTime lastUpdate)
        {
            formInstanceDAL.Delete(formInstanceId, lastUpdate);
        }

        public List<FormInstanceDataOut> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId)
        {
            return Mapper.Map<List<FormInstanceDataOut>>(formInstanceDAL.GetByEpisodeOfCareId(episodeOfCareId, organizationId));
        }

        public List<FormInstanceDataOut> SearchByTitle(int episodeOfCareId, string title)
        {
            return Mapper.Map<List<FormInstanceDataOut>>(formInstanceDAL.SearchByTitle(episodeOfCareId, title));
        }

        public void SignDocument(string formInstanceId, int signedById, FormState formInstanceState)
        {
            FormInstance formInstance = GetById(formInstanceId);
            if(formInstance != null)
            {
                formInstance.FormState = formInstanceState;
                formInstanceDAL.InsertOrUpdate(formInstance, formInstance.GetCurrentFormInstanceStatus(signedById, isSigned: true));
            }
        }

        public IList<FormInstanceStatusDataOut> GetWorkflowHistory(List<FormInstanceStatus> formInstanceStatuses)
        {
            IList<FormInstanceStatusDataOut> workflowHistory = new List<FormInstanceStatusDataOut>();

            if(formInstanceStatuses != null)
            {
                List<int> createdByIds = formInstanceStatuses.Select(x => x.CreatedById).Distinct().ToList();
                Dictionary<int, User> createdByUsers = userDAL.GetAllByIds(createdByIds).ToDictionary(u => u.UserId, u => u);
                foreach(FormInstanceStatus formInstanceStatus in formInstanceStatuses.OrderByDescending(x => x.CreatedOn))
                {
                    createdByUsers.TryGetValue(formInstanceStatus.CreatedById, out User createdBy);
                    if(createdBy != null)
                    {
                        workflowHistory.Add(new FormInstanceStatusDataOut() {
                            CreatedById = createdBy.UserId,
                            CreatedByName = $"{createdBy.FirstName} {createdBy.LastName}",
                            CreatedByActiveOrganization = createdBy.UserConfig?.ActiveOrganization?.Name,
                            CreatedOn = formInstanceStatus.CreatedOn,
                            FormInstanceStatus = formInstanceStatus.Status,
                            IsSigned = formInstanceStatus.IsSigned
                        });
                    }
                }
            }

            return workflowHistory;
        }

        public DataCaptureChartUtility GetPlottableFieldsByThesaurusId(string formId, List<int> fieldThesaurusIds, List<FieldDataOut> fieldsDataOut, DateTime? DateTimeFrom, DateTime? DateTimeTo)
        {
            DataCaptureChartUtility chartUtilityDataStructure = new DataCaptureChartUtility();
            foreach (int fieldId in fieldThesaurusIds)
            {
                List<BsonDocument> bsonDocuments = formInstanceDAL.GetPlottableFieldsByThesaurusId(formId, fieldId);
                foreach(BsonDocument bsonDocument in bsonDocuments)
                {
                    BsonValue dateTimeToPlot = GetBsonValueHelper(bsonDocument, "Date") ?? GetBsonValueHelper(bsonDocument, "EntryDateTimeValue");

                    if (dateTimeToPlot.ToUniversalTime() >= DateTimeFrom.GetValueOrDefault().ToUniversalTime() &&
                         dateTimeToPlot.ToUniversalTime() <= DateTimeTo.GetValueOrDefault().ToUniversalTime())
                    {
                        BsonValue valueThesaurusIdsArray = GetBsonValueHelper(bsonDocument, "Value");
                        int? valueThesaurusId = null;

                        if (valueThesaurusIdsArray != null && valueThesaurusIdsArray is BsonArray && valueThesaurusIdsArray.AsBsonArray.Count > 0)
                            valueThesaurusId = (valueThesaurusIdsArray.AsBsonArray)[0].ToInt32();

                        var field = fieldsDataOut.FirstOrDefault(f => f.ThesaurusId == fieldId);
                        var value = (field as FieldSelectableDataOut).Values.FirstOrDefault(y => y.ThesaurusId == valueThesaurusId);


                        chartUtilityDataStructure.AddToKeyIfExists(
                            field.Label,
                            value != null ? value.NumericValue : null,
                            (long)(dateTimeToPlot.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds);
                    }
                                       
                }
            }
            return chartUtilityDataStructure;
        }

        private BsonValue GetBsonValueHelper(BsonDocument bson, string value)
        {
            BsonValue bsonValue = null;
            bson.TryGetValue(value, out bsonValue);
            return !(bsonValue is BsonNull) ? bsonValue : null ;
        }

        private List<FormInstanceTableDataOut> LoadFormInstancesDataOut(FormInstanceFilterData filterData)
        {
            List<FormInstanceTableDataOut> formInstances = Mapper.Map<List<FormInstanceTableDataOut>>(this.formInstanceDAL.GetByFormThesaurusId(filterData));
            PopulateUsersAndPatients(formInstances);
            
            return formInstances;
        }

        private void PopulateUsersAndPatients(List<FormInstanceTableDataOut> formInstances)
        {
            List<int> userIds = formInstances.Select(x => x.UserId).ToList();
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(userIds));
            List<int> patientIds = formInstances.Select(x => x.PatientId).ToList();
            List<PatientTableDataOut> patients = Mapper.Map<List<PatientTableDataOut>>(patientDAL.GetAllByIds(patientIds));

            foreach (var formInstance in formInstances)
            {
                formInstance.User = users.FirstOrDefault(x => x.Id == formInstance.UserId);
                formInstance.Patient = patients.FirstOrDefault(x => x.Id == formInstance.PatientId);
            }
        }
    }
}
