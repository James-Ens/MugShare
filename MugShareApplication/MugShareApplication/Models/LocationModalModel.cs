using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class LocationModalModel
    {
        public string MachineKey { get; set; }
        public string MachineID { get; set; }
        public string MachineLocation { get; set; }
        public int OpeningHour { get; set; }
        public int ClosingHour { get; set; }
        public string CurrentSupply { get; set; }
        public string TotalCapacity { get; set; }
        public string TotalMugsDispensed { get; set; }
        public bool OutOfOrder { get; set; }
        public string Notes { get; set; }

        public LocationModalModel()
        {
            this.MachineKey = null;
            this.MachineID = null;
            this.MachineLocation = null;
            this.OpeningHour = -1;
            this.ClosingHour = -1;
            this.CurrentSupply = null;
            this.TotalCapacity = null;
            this.TotalMugsDispensed = null;
            this.OutOfOrder = false;
            this.Notes = null;
        }

        public LocationModalModel(string MachineKey, string MachineID, string MachineLocation, int OpeningHour, int ClosingHour,
            string CurrentSupply, string TotalCapacity, string TotalMugsDispensed, bool OutOfOrder, string Notes)
        {
            this.MachineKey = MachineKey;
            this.MachineID = MachineID;
            this.MachineLocation = MachineLocation;
            this.OpeningHour = OpeningHour;
            this.ClosingHour = ClosingHour;
            this.CurrentSupply = CurrentSupply;
            this.TotalCapacity = TotalCapacity;
            this.TotalMugsDispensed = TotalMugsDispensed;
            this.OutOfOrder = OutOfOrder;
            this.Notes = Notes;
        }
    }
}