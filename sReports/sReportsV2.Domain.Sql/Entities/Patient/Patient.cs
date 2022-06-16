using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Patient : EntitiesBase.Entity
    {
        public int Id { get; set; }
        public List<Identifier> Identifiers { get; set; }
        public bool Active { get; set; }
        public Name Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public Address Addresss { get; set; }
        public MultipleBirth MultipleB { get; set; }
        public Contact ContactPerson { get; set; }
        public List<Telecom> Telecoms { get; set; }
        public List<Communication> Communications { get; set; }

        public List<EpisodeOfCare.EpisodeOfCare> EpisodeOfCares { get; set; }

        
        public void SetAddress(Address address)
        {
            if (this.Addresss != null)
            {
                this.Addresss.SetAddress(address);
            }
            else 
            {
                this.Addresss = address;
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

        public void SetGenderFromString(string gender)
        {
            switch (gender)
            {
                case "Male":
                    this.Gender = Gender.Male; break;
                case "Female":
                    this.Gender = Gender.Female; break;
                case "Other":
                    this.Gender = Gender.Other; break;
                case "Unknown":
                    this.Gender = Gender.Unknown; break;
                default:
                    this.Gender = Gender.Unknown; break;
            }
        }

        public void Copy(Patient patient)
        {
            SetIdentifiers(patient.Identifiers);
            Gender = patient.Gender;
            BirthDate = patient.BirthDate;
            SetName(patient.Name);
            SetAddress(patient.Addresss);
            MultipleB.isMultipleBorn = patient.MultipleB.isMultipleBorn;
            MultipleB.Number = patient.MultipleB.Number;
            SetTelecoms(patient.Telecoms);
            SetComunication(patient.Communications);
            SetContactPerson(patient.ContactPerson);
        }

        private void SetIdentifiers(List<Identifier> identifiers)
        {
            foreach (var identifier in identifiers.Where(x => x.Id == 0).ToList())
            {
                this.Identifiers.Add(identifier);
            }
        }

        private void SetTelecoms(List<Telecom> telecoms)
        {
            foreach (var telecom in telecoms.Where(x => x.Id == 0).ToList())
            {
                this.Telecoms.Add(telecom);
            }
        }

        private void SetComunication(List<Communication> communications)
        {
            foreach (var communication in communications.Where(x => x.Id == 0))
            {
                this.Communications.Add(communication);
            }
        }

        private void SetContactPerson(Contact contact)
        {
            if(this.ContactPerson == null)
            {
                this.ContactPerson = contact;
            }
            else
            {
                this.ContactPerson.Copy(contact);   
            }
        }
    }
}
