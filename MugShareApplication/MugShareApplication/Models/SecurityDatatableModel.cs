using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class SecurityDatatableModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string AdminPermission { get; set; }
        public string buttons { get; set; }

        public SecurityDatatableModel()
        {
            this.Username = null;
            this.Email = null;
            this.AdminPermission = null;
            this.buttons = null;
        }

        public SecurityDatatableModel(string Username, string Email, string AdminPermission, string buttons)
        {
            this.Username = Username;
            this.Email = Email;
            this.AdminPermission = AdminPermission;
            this.buttons = buttons;
        }
    }
}