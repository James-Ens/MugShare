using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class YearlyDataModel
    {
        public string Year { get; set; }
        public int TotalMugsBorrowed { get; set; }

        public YearlyDataModel()
        {
            this.Year = null;
            this.TotalMugsBorrowed = -1;
        }

        public YearlyDataModel(string Year, int TotalMugsBorrowed)
        {
            this.Year = Year;
            this.TotalMugsBorrowed = TotalMugsBorrowed;
        }
    }
}