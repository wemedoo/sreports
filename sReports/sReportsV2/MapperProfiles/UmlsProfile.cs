using AutoMapper;
using sReportsV2.Models.Umls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UmlsClient.UMLSClasses;
using UMLSClient.UMLSClasses;

namespace sReportsV2.MapperProfiles
{
    public class UmlsProfile : Profile
    {
        public UmlsProfile()
        {
            CreateMap<SearchResult, SearchResultViewModel>();

            CreateMap<ConceptSearchResult, ConceptSearchResultViewModel>();

            CreateMap(typeof(UMLSSearchResult), typeof(UmlsViewModel<>));

            CreateMap<ConceptDefinition, ConceptDefinitionViewModel>();

            CreateMap(typeof(UMLSConceptDefinition), typeof(UmlsViewModel<>));

            CreateMap(typeof(UMLSAtomResult), typeof(UmlsViewModel<>));

            CreateMap<Atom, AtomViewModel>();

        }
    }
}