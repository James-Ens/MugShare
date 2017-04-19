using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class MugsModalModel
    {
        public string MugKey { get; set; }
        public string MugID { get; set; }
        public string LastBorrowedBy { get; set; }
        public bool CurrentlyInUse { get; set; }
        public string Notes { get; set; }

        public MugsModalModel()
        {
            this.MugKey = null;
            this.MugID = null;
            this.LastBorrowedBy = null;
            this.CurrentlyInUse = false;
            this.Notes = null;
        }

        public MugsModalModel(string MugKey, string MugID, string LastBorrowedBy, bool CurrentlyInUse, string Notes)
        {
            this.MugKey = MugKey;
            this.MugID = MugID;
            this.LastBorrowedBy = LastBorrowedBy;
            this.CurrentlyInUse = CurrentlyInUse;
            this.Notes = Notes;
        }
    }
}