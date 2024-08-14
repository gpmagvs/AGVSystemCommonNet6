
using AGVSystemCommonNet6.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGVSystemCommonNet6.User
{
    public class UserEntity
    {
        [PrimaryKey]
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }
        public ERole Role { get; set; }
        public string WebFunctionPermissionsJson { get; set; } = new WebFunctionViewPermissions().ToString();

        [NotMapped]
        public WebFunctionViewPermissions WebFunctionPermissions
        {
            get
            {
                if (this.WebFunctionPermissionsJson == null || this.WebFunctionPermissionsJson == "")
                {
                    return new WebFunctionViewPermissions();
                }
                return JsonConvert.DeserializeObject<WebFunctionViewPermissions>(this.WebFunctionPermissionsJson);
            }
        }
    }
}
