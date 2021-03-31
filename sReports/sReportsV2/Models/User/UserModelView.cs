using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sReportsV2.Models.User
{
    public class UserModelView
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public Uri ReturnUrl { get; set; }
    }
}