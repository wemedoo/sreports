using Chapters.Resources;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters
{
    public class PatientParser
    {
        public List<string> identifierList;
        private List<IdentifierType> identifierTypes;
        public FormChapter patientChapter { get; set; }

        public PatientParser(List<IdentifierType> identifierTypes)
        {
            identifierList = Enum.GetNames(typeof(TypeOfIdentifier)).ToList();
            this.identifierTypes = identifierTypes;
        }

        public PatientEntity ParsePatientChapter(FormChapter chapter)
        {
            patientChapter = chapter;
            PatientEntity result = ParseIntoPatient();

            if (result != null)
            {
                ConvertPatientIdentifier(result);
            }
            
            return result;
        }

        public PatientEntity ParseIntoPatient()
        {
            PatientEntity patient = null;
            if (patientChapter != null)
            {
                List<Field> basicInfoFields = patientChapter.GetFieldsByList(PatientRelatedLists.basicInfoList);
                List<Field> identifiersFields = patientChapter.GetFieldsByList(identifierList);
                List<Field> addressInfoFields = patientChapter.GetFieldsByList(PatientRelatedLists.addressInfoList);
                List<Field> contactPersonFields = patientChapter.GetFieldsByList(PatientRelatedLists.contactPersonList);
                List<Field> telecomFields = patientChapter.GetFieldsByList(PatientRelatedLists.telecomValues);

                patient = ParseBasicIntoPatient(basicInfoFields);
                patient.Telecoms = GetTelecomsForPatient(telecomFields);
                patient.Identifiers = GetListIdentifiers(identifiersFields);
                patient.Addresss = ParseIntoPatientAddress(addressInfoFields);
                patient.ContactPerson = ParseIntoContactPerson(contactPersonFields);
            }

            return patient;
        }

        public List<sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity> GetListIdentifiers(List<Field> identifiersFields)
        {
            List<sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity> identifiers = new List<sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity>();
            sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity identifier = ParseIntoIdentifier(identifiersFields);
            if (identifier != null)
            {
                identifiers.Add(identifier);
            }

            return identifiers;
        }
        public void ConvertPatientIdentifier(PatientEntity patient)
        {
            IdentifierType type = identifierTypes.FirstOrDefault(x => patient.Identifiers != null && patient.Identifiers.Count() > 0 && x.Name.Equals(patient.Identifiers[0].System));
            if(type != null)
            {
                patient.Identifiers[0].System = type.O4MtId;
            }
        }

        private PatientEntity ParseBasicIntoPatient(List<Field> basicInfoList)
        {
            PatientEntity patientEntity = new PatientEntity();
            patientEntity.Name = new Name()
            {
                Family =!string.IsNullOrWhiteSpace(basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Family).Value?[0]) ? basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Family).Value?[0] : PdfParserType.Unknown,
                Given =!string.IsNullOrWhiteSpace(basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Name).Value?[0]) ? basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Name).Value?[0] : PdfParserType.Unknown
            };

            var patientField = basicInfoList.FirstOrDefault(x => x.FhirType ==  PdfParserType.BirthDate);

            if (!string.IsNullOrEmpty(patientField.Value?[0]))
            {
                patientEntity.BirthDate = Convert.ToDateTime(patientField.Value?[0]);
            }

            patientEntity.Communications = new List<Communication>();
            Communication communication = new Communication();
            communication.Language = basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Language)?.Value?[0];
            communication.Preferred = true;

            if (!string.IsNullOrEmpty(communication.Language))
            {
                patientEntity.Communications.Add(communication);
            }

            patientEntity.Active = true;
            patientEntity.SetGenderFromString(basicInfoList.FirstOrDefault(x => x.FhirType == PdfParserType.Gender)?.Value?[0]);

            return patientEntity;
        }

        private sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity ParseIntoIdentifier(List<Field> identifierFields)
        {
            sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity identifier = null;
            string field = identifierFields.FirstOrDefault(x => x.FhirType == PdfParserType.IdentifierName)?.Value?[0];
            string value = identifierFields.FirstOrDefault(x => x.FhirType == PdfParserType.IdentifierValue)?.Value?[0];
            string use = identifierFields.FirstOrDefault(x => x.FhirType == PdfParserType.IdentifierUse)?.Value?[0];

            if (!string.IsNullOrWhiteSpace(field) && !string.IsNullOrWhiteSpace(value))
            {
                identifier = new sReportsV2.Domain.Entities.OrganizationEntities.IdentifierEntity
                {
                    System = field,
                    Value = value,
                    Use = use
                };
            }

            return identifier;
        }

        private Address ParseIntoPatientAddress(List<Field> addressFields)
        {
            Address address = new Address
            {
                City = addressFields.FirstOrDefault(x => x.FhirType == PdfParserType.City)?.Value?[0],
                State = addressFields.FirstOrDefault(x => x.FhirType == PdfParserType.State)?.Value?[0],
                PostalCode = addressFields.FirstOrDefault(x => x.FhirType == PdfParserType.PostalCode)?.Value?[0],
                Country = addressFields.FirstOrDefault(x => x.FhirType == PdfParserType.Country)?.Value?[0],
                Street = addressFields.FirstOrDefault(x => x.FhirType == PdfParserType.Street)?.Value?[0]

            };

            return address;
        }

        private List<Telecom> GetTelecomsForContact(List<Field> contactFields)
        {
            List<Telecom> Telecoms = new List<Telecom>();
            SetTelecom(nameof(TelecomSystemType.Phone), PdfParserType.ContactPhone, PdfParserType.ContactPhoneUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Email), PdfParserType.ContactEmail, PdfParserType.ContactEmailUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Url), PdfParserType.ContactUrl, PdfParserType.ContactUrlUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Fax), PdfParserType.ContactFax, PdfParserType.ContactFaxUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Sms), PdfParserType.ContactSms, PdfParserType.ContactSmsUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Other), PdfParserType.ContactOther, PdfParserType.ContactOtherUse, contactFields, Telecoms);
            SetTelecom(nameof(TelecomSystemType.Pager), PdfParserType.ContactPager, PdfParserType.ContactPagerUse, contactFields, Telecoms);

            return Telecoms;
        }

        private void SetTelecom(string system, string value, string use, List<Field> telecomsOptions, List<Telecom> telecoms)
        {
            var phoneUse = telecomsOptions.FirstOrDefault(x => x.FhirType == use)?.Value?[0];
            var phone = telecomsOptions.FirstOrDefault(x => x.FhirType == value)?.Value?[0];

            if (!string.IsNullOrEmpty(phoneUse) && !string.IsNullOrEmpty(phone))
            {
                telecoms.Add(new Telecom(system, phone, phoneUse));
            }
        }

        private List<Telecom> GetTelecomsForPatient(List<Field> telecomsOptions)
        {
            List<Telecom> telecoms = new List<Telecom>();
            SetTelecom(nameof(TelecomSystemType.Phone), PdfParserType.Phone, PdfParserType.PhoneUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Email), PdfParserType.Email, PdfParserType.EmailUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Url), PdfParserType.Url, PdfParserType.UrlUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Fax), PdfParserType.Fax, PdfParserType.FaxUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Sms), PdfParserType.Sms, PdfParserType.SmsUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Other), PdfParserType.Other, PdfParserType.OtherUse, telecomsOptions, telecoms);
            SetTelecom(nameof(TelecomSystemType.Pager), PdfParserType.Pager, PdfParserType.PagerUse, telecomsOptions, telecoms);

            return telecoms;
        }

        private Contact ParseIntoContactPerson(List<Field> contactFields)
        {
            Address contactAddress = new Address
            {
                City = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactCity)?.Value?[0],
                State = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactState)?.Value?[0],
                PostalCode = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactPostalCode)?.Value?[0],
                Country = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactCountry)?.Value?[0],
                Street = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactStreet)?.Value?[0]
            };

            Contact contactPerson = new Contact
            {
                Address = contactAddress,
                Gender = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactGender).Value?[0],
                Name = new Name(contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactName).Value?[0], contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.ContactFamily)?.Value?[0] ?? ""),
                Telecoms = new List<Telecom>()
            };

            List<Field> telecomFields = contactFields.Where(x => PatientRelatedLists.contactTelecomValues.Contains(x.FhirType)).ToList();
            contactPerson.Telecoms = GetTelecomsForContact(telecomFields);
            contactPerson.Relationship = contactFields.FirstOrDefault(x => x.FhirType == PdfParserType.Relationship).Value?[0];

            return contactPerson;
        }
    }
}
