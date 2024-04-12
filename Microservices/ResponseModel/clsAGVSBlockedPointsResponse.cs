using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsAGVSBlockedPointsResponse : clsResponseBase
    {
        public List<int> blockedTags { get; set; } = new List<int>();
        public clsAGVSBlockedPointsResponse(bool confirm, string message, List<int> blockedTags)
        {

            this.confirm = confirm;
            this.message = message;
            this.blockedTags = blockedTags;
        }
    }
}
