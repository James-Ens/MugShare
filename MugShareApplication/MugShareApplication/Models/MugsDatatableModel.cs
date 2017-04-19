using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class MugsDatatableModel
    {
        public string MugID { get; set; }
        public string CurrentlyInUse { get; set; }
        public string buttons { get; set; }

        public MugsDatatableModel()
        {
            this.MugID = null;
            this.CurrentlyInUse = null;
            this.buttons = null;
        }

        public MugsDatatableModel(string MugID, string CurrentlyInUse, string buttons)
        {
            this.MugID = MugID;
            this.CurrentlyInUse = CurrentlyInUse;
            this.buttons = buttons;
        }
    }
}