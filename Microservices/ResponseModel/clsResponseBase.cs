using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public abstract class clsResponseBase
    {
        public bool confirm { get; set; } = false;
        public string message { get; set; } = "";
    }
}
