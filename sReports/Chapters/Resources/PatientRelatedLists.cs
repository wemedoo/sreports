using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters
{
    public class PatientRelatedLists
    {
        public static List<string> basicInfoList = new List<string>() { "Name", "Family", "Gender", "BirthDate", "MultipleBirth", "Language" };

        public static List<string> addressInfoList = new List<string>() { "City", "State", "PostalCode", "Country", "Street" };

        public static List<string> contactPersonList = new List<string>() { "ContactStreet","ContactCity" , "ContactState", "ContactPostalCode", "ContactCountry", "ContactGender", "ContactName","ContactFamily",
             "ContactTelecomCheckBox", "ContactPhoneUse", "ContactPhone", "ContactFaxUse", "ContactFax", "ContactEmailUse","ContactEmail", "ContactPagerUse",
            "ContactPager", "ContactUrlUse", "ContactUrl", "ContactSmsUse", "ContactSms", "ContactOtherUse", "ContactOther","Relationship"};

        public static List<string> telecomValues = new List<string>() {"TelecomCheckBox", "PhoneUse", "Phone", "FaxUse", "Fax", "EmailUse", "Email", "PagerUse", "Pager", "UrlUse", "Url",
                "SmsUse", "Sms", "OtherUse", "Other" };

        public static List<string> contactTelecomValues = new List<string>() {"ContactTelecomCheckBox", "ContactPhoneUse", "ContactPhone", "ContactFaxUse", "ContactFax", "ContactEmailUse","ContactEmail", "ContactPagerUse",
            "ContactPager", "ContactUrlUse", "ContactUrl", "ContactSmsUse", "ContactSms", "ContactOtherUse", "ContactOther" };

    }
}
