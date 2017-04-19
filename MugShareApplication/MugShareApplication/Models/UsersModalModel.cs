using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class UsersModalModel
    {
        public string UserKey { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool MugInUse { get; set; }
        public string DateOfRental { get; set; }
        public string TotalMugsBorrowed { get; set; }
        public string Notes { get; set; }

        public UsersModalModel()
        {
            this.UserKey = null;
            this.StudentNumber = null;
            this.FirstName = null;
            this.LastName = null;
            this.Email = null;
            this.MugInUse = false;
            this.DateOfRental = null;
            this.TotalMugsBorrowed = null;
            this.Notes = null;
        }

        public UsersModalModel(string UserKey, string StudentNumber, string FirstName, string LastName, string Email, bool MugInUse, 
            string DateOfRental, string TotalMugsBorrowed, string Notes)
        {
            this.UserKey = UserKey;
            this.StudentNumber = StudentNumber;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.MugInUse = MugInUse;
            this.DateOfRental = DateOfRental;
            this.TotalMugsBorrowed = TotalMugsBorrowed;
            this.Notes = Notes;
        }
    }
}