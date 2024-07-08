using Microsoft.EntityFrameworkCore;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    [Index(nameof(RecordTime))]
    [Index(nameof(MaterialID))]
    [Index(nameof(ActualID))]
    [Index(nameof(SourceStation))]
    [Index(nameof(TargetStation))]
    public class clsMaterialInfo
    {
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public string MaterialID { get; set; } = "";

        public string ActualID { get; set; } = "";

        public string SourceStation { get; set; } = "";

        public string TargetStation { get; set; } = "";

        public string TaskSourceStation { get; set; } = "";

        public string TaskTargetStation { get; set; } = "";

        public MaterialIDStatus IDStatus { get; set; } = MaterialIDStatus.NG;

        public MaterialInstallStatus InstallStatus { get; set; } = MaterialInstallStatus.NG;

        public MaterialType Type { get; set; } = MaterialType.None;

        public MaterialCondition Condition { get; set; } = MaterialCondition.Add;
    }
}
