using sReportsV2.DTOs.Organization;
using sReportsV2.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.Common
{
    public class UserDataViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString()
        {
            return $"{Username} ({FirstName} {LastName})";
        }
    }
}