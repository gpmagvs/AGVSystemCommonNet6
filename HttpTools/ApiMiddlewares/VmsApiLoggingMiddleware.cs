using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.HttpTools.ApiMiddlewares
{
    public class VmsApiLoggingMiddleware : ApiLoggingMiddleware
    {
        public VmsApiLoggingMiddleware(RequestDelegate next) : base(next)
        {
        }
        public override Logger CreateLogger(HttpContext context)
        {
            if (context.Request.Path.ToString().ToLower().Contains("api/agv/"))
            {
                var AGVName = context.Request.Query["AGVName"];
                return LogManager.GetLogger($"VehicleState/{AGVName}");
            }
            else
                return base.CreateLogger(context);
        }
    }
}
