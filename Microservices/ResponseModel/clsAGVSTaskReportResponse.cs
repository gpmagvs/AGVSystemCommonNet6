﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.ResponseModel
{
    public class clsAGVSTaskReportResponse : clsResponseBase
    {
        public clsAGVSTaskReportResponse()
        {
        }

        public clsAGVSTaskReportResponse(bool confirm, string message) : base(confirm, message)
        {
        }
    }
}