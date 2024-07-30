﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AGVSystemCommonNet6.PartsModels
{
    public partial class TaskQueue
    {
        public string Name { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string FromStation { get; set; }
        public string FromAction { get; set; }
        public string ToStation { get; set; }
        public string ToAction { get; set; }
        public string Cstid { get; set; }
        public string ReturnValue { get; set; }
        public string Priority { get; set; }
        public int? Status { get; set; }
        public string ExecuteAgv { get; set; }
        public string Mrnum { get; set; }
        public string JobNo { get; set; }
        public DateTime? ReturnDispatchTime { get; set; }
        public string ReturnDispatchStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string ExecuteTaskStartPosition { get; set; }
        public string MotorEncoder { get; set; }
        public string AreaName { get; set; }
        public string DispatchStation { get; set; }
        public string DispatchDelete { get; set; }
    }
}