using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class UsersDatatableModel
    {
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TotalMugsBorrowed { get; set; }
        public string buttons { get; set; }

        public UsersDatatableModel()
        {
            this.StudentNumber = null;
            this.FirstName = null;
            this.LastName = null;
            this.TotalMugsBorrowed = null;
            this.buttons = null;
        }

        public UsersDatatableModel(string StudentNumber, string FirstName, string LastName, string TotalMugsBorrowed, string buttons)
        {
            this.StudentNumber = StudentNumber;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.TotalMugsBorrowed = TotalMugsBorrowed;
            this.buttons = buttons;
        }
    }
}