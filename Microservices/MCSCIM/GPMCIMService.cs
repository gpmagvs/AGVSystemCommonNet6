﻿using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.MCS
{
    public class GPMCIMService
    {
        public static string HostUrl => "http://localhost:5400";
        private static HttpHelper _CIMhttp;

        private static HttpHelper CIMhttp
        {
            get
            {
                if (_CIMhttp == null)
                {
                    _CIMhttp = new HttpHelper(HostUrl);
                }
                return _CIMhttp;
            }
        }
        public static async Task<clsCIMResponse> ChangePortTypeOfEq(int tagID, int portType)
        {
            clsCIMResponse response = await CIMhttp.PostAsync<clsCIMResponse, object>($"/api/porttype_change?eqTag={tagID}&portType={portType}", null, timeout: 20);
            return response;
        }
        public class clsCIMResponse
        {
            public int code { get; set; } = 0;
            public string message { get; set; } = "";
            public clsCIMResponse(int return_code, string message = "")
            {
                code = return_code;
                this.message = message;
            }
        }
    }
}