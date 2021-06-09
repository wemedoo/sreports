using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class FormDistribution : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public string VersionId { get; set; }
        public List<FormFieldDistribution> Fields { get; set; }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;

            if (this.Fields != null)
            {
                foreach (FormFieldDistribution field in this.Fields)
                {
                    field.ThesaurusId = field.ThesaurusId == oldThesaurus ? newThesaurus : field.ThesaurusId;
                    if (field.Values != null)
                    {
                        foreach (var value in field.Values)
                        {
                            value.ThesaurusId = value.ThesaurusId == oldThesaurus ? newThesaurus : value.ThesaurusId;
                        }
                    }
                    if (field.ValuesAll != null)
                    {
                        foreach (var val in field.ValuesAll)
                        {
                            if (val.Values != null)
                            {
                                foreach (var v in val.Values)
                                {
                                    v.ThesaurusId = v.ThesaurusId == oldThesaurus ? newThesaurus : v.ThesaurusId;

                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
