using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MugShareApplication.Models
{
    /*--------------------------------------------------------------------------------------
     * Model used for true or false values during validations in controllers
     * -------------------------------------------------------------------------------------*/
    public class IsTrueOrFalse
    {
        public bool boolVal { get; set; }

        public IsTrueOrFalse()
        {
            this.boolVal = false;
        }

        public IsTrueOrFalse(bool boolVal)
        {
            this.boolVal = boolVal;
        }
    }
}