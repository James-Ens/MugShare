using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class HourlyDataModel
    {
        public string Hour { get; set; }
        public int TotalMugsBorrowed { get; set; }

        public HourlyDataModel()
        {
            this.Hour = null;
            this.TotalMugsBorrowed = -1;
        }

        public HourlyDataModel(string Hour, int TotalMugsBorrowed)
        {
            this.Hour = Hour;
            this.TotalMugsBorrowed = TotalMugsBorrowed;
        }
    }
}