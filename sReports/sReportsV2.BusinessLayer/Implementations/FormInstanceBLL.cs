using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FormInstanceBLL : IFormInstanceBLL
    {
        private readonly IUserDAL userDAL;
        private readonly HttpContextBase context;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IFormInstanceDAL formInstanceDAL;

        public FormInstanceBLL(IUserDAL userDAL, IOrganizationDAL organizationDAL, HttpContextBase context, IFormDAL formDAL, IFormInstanceDAL formInstanceDAL)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.context = context;
            this.formDAL = formDAL;
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
        public string InsertOrUpdate(FormInstance form)
        {
            return formInstanceDAL.InsertOrUpdate(form);
        }

        private List<FormInstanceTableDataOut> LoadFormInstancesDataOut(FormInstanceFilterData filterData)  
        {
            List<FormInstanceTableDataOut> formInstances = Mapper.Map<List<FormInstanceTableDataOut>>(this.formInstanceDAL.GetByFormThesaurusId(filterData));
            List<int> userIds = formInstances.Select(x => x.UserId).ToList();
            List<UserDataOut> users = Mapper.Map<List<UserDataOut>>(userDAL.GetAllByIds(userIds));
            foreach (var formInstance in formInstances) 
            {
                formInstance.User = users.FirstOrDefault(x => x.Id == formInstance.UserId);
            }

            return formInstances;
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
    }
}
