using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.SMS
{
    public class SMSHelper
    {

        public SMSHelper() { 
        }

        private HttpHelper http;

        public string SMSIP { get; set; }
        public int Port { get; set; } = 8002;

        public string AP { get; set; } = "FAC_FAB_AMCAGV_SMS";

        public Dictionary<string, clsSMSSponsorInfo> ZonesInfo { get; set; } = new Dictionary<string, clsSMSSponsorInfo>
        {
            { "F14P12", new clsSMSSponsorInfo{
                    ZoneName = "F14P12",
                   SMSIP = "10.11.120.60",
                   Phones = new List<string>(){ "7342038","7142297","7342356"}
                }
            },
            { "F15P12", new clsSMSSponsorInfo{
                    ZoneName = "F15P12",
                   SMSIP = "10.11.200.59",
                   Phones = new List<string>(){ "7258284","734356"}
                }
            },
            { "F18P5", new clsSMSSponsorInfo{
                    ZoneName = "F18P5",
                   SMSIP = "10.11.136.60",
                   Phones = new List<string>(){ "728876","7342356"}
                }
            }
        };

        public string BaseUrl
        {
            get
            {
                return $"http://{SMSIP}:{Port}";
            }
        }

        /// <summary>
        /// 發送簡訊
        /// </summary>
        /// <param name="zone_name">廠區名稱</param>
        /// <param name="agv_name">AGV名稱</param>
        /// <param name="message_en">訊息(英文)</param>
        /// <param name="message_zh">訊息(中文)</param>
        /// <returns></returns>
        public async Task<(bool success, string response_string)> SendSMS(string zone_name, string agv_name, string message_en, string message_zh)
        {
            try
            {
                if (!ZonesInfo.TryGetValue(zone_name, out var SponsorInfo))
                {
                    return (false, "Zone Name Not Defined");
                }
                SMSIP = SponsorInfo.SMSIP;
                http = new HttpHelper(BaseUrl);
                clsSMSData data = new clsSMSData()
                {
                    Message = $"{zone_name}{agv_name}{message_en}{message_zh}",
                    Phone = SponsorInfo.Phones
                };
                return await http.PostAsync("/SMS", data);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
