using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class LocationDatatableModel
    {
        public string MachineID { get; set; }
        public string MachineLocation { get; set; }
        public int OpeningHour { get; set; }
        public int ClosingHour { get; set; }
        public string CurrentSupplyPercentage { get; set; }
        public string OutOfOrder { get; set; }
        public string buttons { get; set; }

        public LocationDatatableModel()
        {
            this.MachineID = null;
            this.MachineLocation = null;
            this.OpeningHour = -1;
            this.ClosingHour = -1;
            this.CurrentSupplyPercentage = null;
            this.OutOfOrder = null;
            this.buttons = null;
        }

        public LocationDatatableModel(string MachineID, string MachineLocation, int OpeningHour, int ClosingHour, string CurrentSupplyPercentage, string OutOfOrder, string buttons)
        {
            this.MachineID = MachineID;
            this.MachineLocation = MachineLocation;
            this.OpeningHour = OpeningHour;
            this.ClosingHour = ClosingHour;
            this.CurrentSupplyPercentage = CurrentSupplyPercentage;
            this.OutOfOrder = OutOfOrder;
            this.buttons = buttons;
        }
    }
}