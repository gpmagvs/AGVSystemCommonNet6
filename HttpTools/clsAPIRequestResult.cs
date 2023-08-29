using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.HttpTools
{
    public class clsAPIRequestResult
    {
        public bool Success => ReturnCode == 0;
        public string Message { get; set; }
        public int ReturnCode { get; set; } = -1;
    }
}
