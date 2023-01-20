﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Initializer.PredefinedTypes
{
    public static class PredefinedTypesConstants
    {
        public static List<string> OrganizationType = new List<string>()
        {
            "Healthcare Provider",
            "Hospital Department",
            "Organizational Team",
            "Government",
            "Payer",
            "Educational Institute",
            "Religious Institution",
            "Clinical Research Sponsor",
            "Community Group",
            "Non Healthcare Business Or Corporation",
            "University Hospital",
            "Cantonal Hospital",
            "Group Practice",
            "Family Doctor Office",
            "Research Institution",
            "Spitex",
            "Governmental Body",
            "Non Profit Organization",
            "Patient Group",
            "Department",
            "Team",
            "Other"
        };

        public static List<string> EpisodeOfCareType = new List<string>()
        {
            "Home and Community Care",
            "Post Acute Care",
            "Post coordinated diabetes program",
            "Drug and alcohol rehabilitation",
            "Community-based aged care",
            "General Prevention Program of Oncological Diseases",
            "Prevention of Oncological Diseases - Municipality Program for Smoking Cesation",
            "Screening for Oncological Diseases - Colorectal Cancer Screening",
            "Screening for Oncological Diseases - Cantonal Program for Breast Cancer Screening Bern",
            "Diagnosis of Oncological Diseases and Staging Procedures - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Treatment of Oncological Diseases - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Oncological Acute Post-Treatment Care - Head and Neck Cancer Clinical Pathway of Inselspital Bern",
            "Oncological Long-Term Care and Follow-Up - Head and Neck Cancer Clinical Pathway of Inselspital Bern"
        };

        public static List<string> EncounterType = new List<string>()
        {
            "Not Applicable",
            "Annual Diabetes Mellitus Screening",
            "Bone Drilling Bone Marrow Punction In Clinic",
            "Infant Colon Screening 60 minutes",
            "Outpatient Kenacort Injection",
            "Emergency room admission (procedure)",
            "General examination of patient (procedure)",
            "Emergency Room Admission",
            "Emergency hospital admission (procedure)",
            "Encounter for check up (procedure)",
            "Encounter for symptom",
            "Encounter for problem",
            "Periodic reevaluation and management of healthy individual (procedure)",
            "Postoperative follow-up visit (procedure)",
            "Screening surveillance (regime/therapy)",
            "Gynecology service (qualifier value)",
            "Well child visit (procedure)",
            "Encounter for symptom (procedure)",
            "Discussion about treatment (procedure)",
            "Follow-up encounter",
            "Encounter for problem (procedure)",
            "Drug rehabilitation and detoxification",
            "Outpatient procedure",
            "Prenatal initial visit",
            "Prenatal visit",
            "Obstetric emergency hospital admission",
            "Postnatal visit",
            "Consultation for treatment",
            "Patient encounter procedure",
            "Urgent care clinic (environment)",
            "Admission to surgical department",
            "Follow-up visit (procedure)",
            "Non-urgent orthopedic admission",
            "Follow-up encounter (procedure)",
            "Patient-initiated encounter",
            "Asthma follow-up",
            "Emergency hospital admission for asthma",
            "Assessment of dementia (procedure)",
            "Domiciliary or rest home patient evaluation and management",
            "Hospital admission",
            "posttraumatic stress disorder",
            "Allergic disorder initial assessment",
            "Allergic disorder follow-up assessment",
            "Admission to thoracic surgery department",
            "Initial psychiatric interview with mental status and evaluation (procedure)",
            "Telephone encounter (procedure)",
            "Inpatient stay 3 days",
            "Inpatient stay (finding)",
            "Telemedicine consultation with patient",
            "Cardiac arrest",
            "Myocardial infarction",
            "Stroke"
        };

        public static List<string> EncounterStatus = new List<string>()
        {
            "Planned",
            "Arrived",
            "Triaged",
            "In progress",
            "Onleave",
            "Finished",
            "Cancelled"
        };
        
        public static List<string> EncounterClassification = new List<string>()
        {
            "Ambulatory",
            "Emergency",
            "Field",
            "Home Health",
            "Inpatient Encounter",
            "Inpatient Non Acute",
            "Observation Encounter",
            "Pre Admission",
            "Short Stay",
            "Virtual",
            "Not Applicable",
            "Inpatient Acute",
            "DATE OF ASSESSMENT (DD/MM/YYYY):",
            "Leukocyte esterase [Presence] in Urine by Test strip"
        };
        
        public static List<string> DiagnosisRole = new List<string>()
        {
            "Billing",
            "Post Op Diagnosis",
            "Pre Op Diagnosis",
            "Comorbidity Diagnosis",
            "Chief Complaint",
            "Discharge Diagnosis",
            "Admission Diagnosis",
            "Not Applicable",
            "Leukocyte esterase [Presence] in Urine by Test strip"
        };
        
        public static List<string> PatientIdentifierType = new List<string>()
        {
            "Account number",
            "Account number Creditor",
            "Account number debitor",
            "Accreditation/Certification Identifier",
            "Advanced Practice Registered Nurse number",
            "American Express",
            "American Medical Association Number",
            "Ancestor Specimen ID",
            "Anonymous identifier",
            "Bank Account Number",
            "Bank Card Number",
            "Birth Certificate",
            "Birth Certificate File Number",
            "Birth registry number",
            "Breed Registry Number",
            "Change of Name Document",
            "Citizenship Card",
            "Cost Center number",
            "Country number",
            "Death Certificate File Number",
            "Death Certificate ID",
            "Dentist license number",
            "Diner's Club card",
            "Diplomatic Passport",
            "Discover Card",
            "Doctor number",
            "Donor Registration Number",
            "Driver's Licence",
            "Drug Enforcement Administration registration number",
            "Drug Furnishing or prescriptive authority Number",
            "Employee number",
            "Facility ID",
            "Fetal Death Report File Number",
            "Fetal Death Report ID",
            "Filler Identifier",
            "General ledger number",
            "Guarantor external identifier",
            "Guarantor internal identifier",
            "Health Card Number",
            "Health Plan Identifier",
            "Indigenous/Aboriginal",
            "Insel PID",
            "Jurisdictional health number (Canada)",
            "Labor and industries number",
            "Laboratory Accession ID",
            "License number",
            "Lifelong physician number",
            "Living Subject Enterprise Number",
            "Local Registry ID",
            "Marriage Certificate",
            "Master Card",
            "Medical License number",
            "Medical Record Number",
            "Medicare/CMS (formerly HCFA)'s Universal Physician Identification numbers",
            "Medicare/CMS Performing Provider Identification Number",
            "Member Number",
            "Microchip Number",
            "Military ID number",
            "National Health Plan Identifier",
            "National Insurance Organization Identifier",
            "National Insurance Payor Identifier (Payor)",
            "National Person Identifier where the xxx is the ISO table 3166 3-character (alphabetic) country code",
            "National employer identifier",
            "National provider identifier",
            "National unique individual identifier",
            "Naturalization Certificate",
            "Nurse practitioner number",
            "Observation Instance",
            "Optometrist license number",
            "Osteopathic License number",
            "Parole Card",
            "Passport Number",
            "Patient Medicaid number",
            "Patient external identifier",
            "Patient internal identifier",
            "Patient's Medicare number",
            "Pension Number",
            "Permanent Resident Card Number",
            "Person number",
            "Pharmacist license number",
            "Physician Assistant number",
            "Placer Identifier",
            "Podiatrist license number",
            "Practitioner Medicaid number",
            "Practitioner Medicare number",
            "Primary physician office number",
            "Public Health Case Identifier",
            "Public Health Event Identifier",
            "Public Health Official ID",
            "QA number",
            "Railroad Retirement Provider",
            "Railroad Retirement number",
            "Regional registry ID",
            "Registered Nurse Number",
            "Resource identifier",
            "Secondary physician office number",
            "Shipment Tracking Number",
            "Social Security Number",
            "Social number",
            "Specimen ID",
            "Staff Enterprise Number",
            "State assigned NDBS card Identifier",
            "State license",
            "State registry ID",
            "Study Permit",
            "Subscriber Number",
            "Temporary Account Number",
            "Temporary Living Subject Number",
            "Temporary Medical Record Number",
            "Temporary Permanent Resident (Canada)",
            "Training License Number",
            "Treaty Number/ (Canada)",
            "Unique Specimen ID",
            "Unique master citizen number",
            "Universal Device Identifier",
            "Unspecified identifier",
            "VISA",
            "Visit number",
            "Visitor Permit",
            "WIC identifier",
            "Work Permit",
            "Workers' Comp Number",
            "Enitentiary/correctional institution Number",
            "Synthea identifier type",
            "Driver's License"
        };
        
        public static List<string> OrganizationIdentifierType = new List<string>()
        {
            "Accession ID",
            "Organization identifier",
            "Provider number",
            "Social Beneficiary Identifier",
            "Tax ID number"
        };

        public static List<string> AddressType = new List<string>()
        {
            "Business",
            "Home",
            "Mailing",
            "Previous"
        };

        public static List<string> Citizenship = new List<string>()
        {
            "British",
            "Serbian",
            "Swiss"
        };

        public static List<string> ReligiousAffiliationType = new List<string>()
        {
            "Adventist",
            "African Religions",
            "Afro-Caribbean Religions",
            "Agnosticism",
            "Anglican",
            "Animism",
            "Assembly of God",
            "Atheism",
            "Babi & Baha'I faiths",
            "Baptist",
            "Bon",
            "Brethren",
            "Cao Dai",
            "Celticism",
            "Christian (non-Catholic, non-specific)",
            "Christian Scientist",
            "Church of Christ",
            "Church of God",
            "Confucianism",
            "Congregational",
            "Cyberculture Religions",
            "Disciples of Christ",
            "Divination",
            "Eastern Orthodox",
            "Episcopalian",
            "Evangelical Covenant",
            "Fourth Way",
            "Free Daism",
            "Friends",
            "Full Gospel",
            "Gnosis",
            "Hinduism",
            "Humanism",
            "Independent",
            "Islam",
            "Jainism",
            "Jehovah's Witnesses",
            "Judaism",
            "Latter Day Saints",
            "Lutheran",
            "Mahayana",
            "Meditation",
            "Messianic Judaism",
            "Methodist",
            "Mitraism",
            "Native American",
            "Nazarene",
            "New Age",
            "non-Roman Catholic",
            "Occult",
            "Orthodox",
            "Paganism",
            "Pentecostal",
            "Presbyterian",
            "Process, The",
            "Protestant",
            "Protestant, No Denomination",
            "Reformed",
            "Reformed/Presbyterian",
            "Roman Catholic Church",
            "Salvation Army",
            "Satanism",
            "Scientology",
            "Shamanism",
            "Shiite (Islam)",
            "Shinto",
            "Sikism",
            "Spiritualism",
            "Sunni (Islam)",
            "Taoism",
            "Theravada",
            "Unitarian Universalist",
            "Unitarian-Universalism",
            "United Church of Christ",
            "Universal Life Church",
            "Vajrayana (Tibetan)",
            "Veda",
            "Voodoo",
            "Wicca",
            "Yaohushua",
            "Zen Buddhism",
            "Zoroastrianism"
        };
    }
}
