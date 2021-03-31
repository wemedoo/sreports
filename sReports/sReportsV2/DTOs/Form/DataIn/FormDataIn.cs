using sReportsV2.Domain.Enums;
using sReportsV2.DTOs.DocumentProperties.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormDataIn
    {
        public string Id { get; set; }
        public FormAboutDataIn About { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public List<FormChapterDataIn> Chapters { get; set; } = new List<FormChapterDataIn>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }
        public string ThesaurusId { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DocumentPropertiesDataIn DocumentProperties { get; set; }
        public FormEpisodeOfCareDataDataIn EpisodeOfCare { get; set; }
        public bool DisablePatientData { get; set; }

        public List<FieldDataIn> GetAllFields()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                   .SelectMany(set => set.Fields
                                    )
                                )
                            )
                        ).ToList();
        }

        public List<string> ValidateFieldsIds()
        {
            List<string> listAllFieldIds = new List<string>();
            List<string> listDuplicateIds = new List<string>();
            foreach (FieldDataIn field in this.GetAllFields())
            {
                if (listAllFieldIds.Contains(field.Id))
                    listDuplicateIds.Add(field.Id);

                listAllFieldIds.Add(field.Id);
            }

            return listDuplicateIds;
        }

    }
}