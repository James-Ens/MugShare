using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class SecurityModalModel
    {
        public string SecurityKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StaffCardID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool AdminPermission { get; set; }

        public SecurityModalModel()
        {
            this.SecurityKey = null;
            this.FirstName = null;
            this.LastName = null;
            this.StaffCardID = null;
            this.Username = null;
            this.Password = null;
            this.Email = null;
            this.AdminPermission = false;
        }

        public SecurityModalModel(string SecurityKey, string FirstName, string LastName, string StaffCardID,
            string Username, string Password, string Email, bool AdminPermission)
        {
            this.SecurityKey = SecurityKey;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.StaffCardID = StaffCardID;
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.AdminPermission = AdminPermission;
        }
    }
}