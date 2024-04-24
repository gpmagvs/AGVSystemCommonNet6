using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsResponseBase
    {
        public clsResponseBase() { }
        public clsResponseBase(bool confirm, string message)
        {
            this.confirm = confirm;
            this.message = message;
        }
        public bool confirm { get; set; } = false;
        public string message { get; set; } = "";
    }
}
