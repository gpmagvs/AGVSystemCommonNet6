﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AGVSystemCommonNet6.PartsModels
{
    public partial class AlarmResetLog
    {
        public DateTime? OccurTime { get; set; }
        public string AlarmCode { get; set; }
        public string ResetType { get; set; }
        public string CauseBy { get; set; }
        public string EventCode { get; set; }
        public string CommentFromAgv { get; set; }
        public string CommentByEventCodeTable { get; set; }
        public string Agvname { get; set; }
        public string Position { get; set; }
        public DateTime? ResetTime { get; set; }
    }
}