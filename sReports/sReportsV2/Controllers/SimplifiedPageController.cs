using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.CRF.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class SimplifiedPageController : FormCommonController
    {

        public SimplifiedPageController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL, userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL)
        {

        }

        protected List<TreeJsonDataOut> GetFormTreeData(List<Form> forms)
        {
            List<Form> formsForTree = new List<Form>();
            formsForTree.AddRange(forms);
            return GetTreeJson(formsForTree);
        }

        protected List<TreeJsonDataOut> GetTreeJson(List<Form> formsData)
        {
            List<TreeJsonDataOut> result = new List<TreeJsonDataOut>();
            foreach (Form formData in formsData)
            {
                TreeJsonDataOut treeJsonDataOut = new TreeJsonDataOut();
                treeJsonDataOut.text = formData.Title;
                treeJsonDataOut.nodes = formData.Chapters.Select(x => new TreeJsonDataOut() { text = x.Title, href = $"#@c.Id" }).ToList();

                result.Add(treeJsonDataOut);
            }

            return result;
        }
    }
}