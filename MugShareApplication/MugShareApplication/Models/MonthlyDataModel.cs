using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    public class MonthlyDataModel
    {
        public int January { get; set; }
        public int February { get; set; }
        public int March { get; set; }
        public int April { get; set; }
        public int May { get; set; }
        public int June { get; set; }
        public int July { get; set; }
        public int August { get; set; }
        public int September { get; set; }
        public int October { get; set; }
        public int November { get; set; }
        public int December { get; set; }

        public MonthlyDataModel()
        {
            this.January = -1;
            this.February = -1;
            this.March = -1;
            this.April = -1;
            this.May = -1;
            this.June = -1;
            this.July = -1;
            this.August = -1;
            this.September = -1;
            this.October = -1;
            this.November = -1;
            this.December = -1;
        }

        public MonthlyDataModel(int January, int February, int March, int April, int May, int June,
            int July, int August, int September, int October, int November, int December)
        {
            this.January = January;
            this.February = February;
            this.March = March;
            this.April = April;
            this.May = May;
            this.June = June;
            this.July = July;
            this.August = August;
            this.September = September;
            this.October = October;
            this.November = November;
            this.December = December;
        }
    }
}