using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.User
{
    /// <summary>
    /// 用戶登入請求模型
    /// </summary>
    public class UserLoginRequest
    {
        public string Username { get; set; }    
        public string Password { get; set; }
    }
}
