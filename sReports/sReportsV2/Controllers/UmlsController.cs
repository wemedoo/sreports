using AutoMapper;
using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Umls;
using sReportsV2.Models.Umls;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using UMLSClient.Client;
using UMLSClient.UMLSClasses;

namespace sReportsV2.Controllers
{
    public class UmlsController : BaseController
    {

        [Authorize]
        public ActionResult Search(UmlsDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            Client umlsClient = new Client();
            var result = Mapper.Map<UmlsViewModel<SearchResultViewModel>>(umlsClient.GetSearchResult(dataIn.SearchTerm, dataIn.PageSize, dataIn.Page));
            return PartialView("ConceptsRows",result);
        }

        [Authorize]
        public ActionResult GetDefinitions(string id)
        {
            Client client = new Client();
            var result = Mapper.Map<UmlsViewModel<List<ConceptDefinitionViewModel>>>(client.GetConceptDefinition(id));
            if (result == null)
            {
                result = new UmlsViewModel<List<ConceptDefinitionViewModel>>()
                {
                    Result = new List<ConceptDefinitionViewModel>()
                };
            }

            return PartialView("DefinitionsList",result);
        }

        [Authorize]
        public ActionResult GetAtoms(string id)
        {
            Client client = new Client();
            var result = Mapper.Map<UmlsViewModel<List<AtomViewModel>>>(client.GetAtomsResult(id));
            if(result == null)
            {
                result = new UmlsViewModel<List<AtomViewModel>>()
                {
                    Result = new List<AtomViewModel>()
                };
            }
            return PartialView("AtomsList",result);
        }
    }
}