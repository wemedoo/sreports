using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public abstract class PatientBase : EntitiesBase.Entity
    {
        public List<Identifier> Identifiers { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        [ForeignKey("MultipleBirthId")]
        public MultipleBirth MultipleBirth { get; set; }
        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }

        [Column("MultipleBirthId")]
        public int? MultipleBirthId { get; set; }
        [Column("ContactId")]
        public int? ContactId { get; set; }
        public List<Communication> Communications { get; set; }
        public List<EpisodeOfCare.EpisodeOfCare> EpisodeOfCares { get; set; }

        protected void Copy(PatientBase patient)
        {
            SetIdentifiers(patient.Identifiers);
            Gender = patient.Gender;
            BirthDate = patient.BirthDate;
            Active = patient.Active;
            SetName(patient.Name);
            if(MultipleBirth != null)
            {
                MultipleBirth.isMultipleBorn = patient.MultipleBirth.isMultipleBorn;
                MultipleBirth.Number = patient.MultipleBirth.Number;
            }
            SetComunication(patient.Communications);
            SetContactPerson(patient.Contact);
        }

        private void SetIdentifiers(List<Identifier> identifiers)
        {
            foreach (var identifier in identifiers.Where(x => x.IdentifierId == 0).ToList())
            {
                this.Identifiers.Add(identifier);
            }
        }

        private void SetName(Name name)
        {
            if (this.Name != null)
            {
                this.Name.SetName(name);
            }
            else
            {
                this.Name = name;
            }
        }

        private void SetComunication(List<Communication> communications)
        {
            foreach (var communication in communications.Where(x => x.CommunicationId != 0))
            {
                var communicationDb = this.Communications.FirstOrDefault(c => c.CommunicationId == communication.CommunicationId);
                if (communicationDb != null)
                {
                    communicationDb.Copy(communication);
                }
            }

            foreach (var communication in communications.Where(x => x.CommunicationId == 0))
            {
                this.Communications.Add(communication);
            }
        }

        private void SetContactPerson(Contact contact)
        {
            if (this.Contact == null)
            {
                this.Contact = contact;
            }
            else
            {
                this.Contact.Copy(contact);
            }
        }
    }
}
