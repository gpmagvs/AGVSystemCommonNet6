using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsGetUsableChargeStationTagResponse : clsResponseBase
    {
        public List<int> usableChargeStationTags { get; set; } = new List<int>();
        public clsGetUsableChargeStationTagResponse(bool confirm, string message, List<int> tagNumbers)
        {
            this.confirm = confirm;
            this.message = message;
            this.usableChargeStationTags = tagNumbers;
        }
    }
}
