﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AGVSystemCommonNet6.PartsModels
{
    public partial class PartsAGVS_InfoContext : DbContext
    {
        public PartsAGVS_InfoContext()
        {
        }

        public PartsAGVS_InfoContext(DbContextOptions<PartsAGVS_InfoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AgvchargeStation> AgvchargeStations { get; set; }
        public virtual DbSet<Agvinfo> Agvinfos { get; set; }
        public virtual DbSet<AgvmaintainTable> AgvmaintainTables { get; set; }
        public virtual DbSet<AlarmResetLog> AlarmResetLogs { get; set; }
        public virtual DbSet<CartInfo> CartInfos { get; set; }
        public virtual DbSet<CartMaintainTable> CartMaintainTables { get; set; }
        public virtual DbSet<ChargeStationStatus> ChargeStationStatuses { get; set; }
        public virtual DbSet<Dispatch> Dispatches { get; set; }
        public virtual DbSet<EarthquakeLog> EarthquakeLogs { get; set; }
        public virtual DbSet<ElevatorInfo> ElevatorInfos { get; set; }
        public virtual DbSet<ElevatorWithAgvinfo> ElevatorWithAgvinfos { get; set; }
        public virtual DbSet<FireAdaminfo> FireAdaminfos { get; set; }
        public virtual DbSet<FireAlarmLog> FireAlarmLogs { get; set; }
        public virtual DbSet<Hidlog> Hidlogs { get; set; }
        public virtual DbSet<MaterialStationInfo> MaterialStationInfos { get; set; }
        public virtual DbSet<MaterialWip> MaterialWips { get; set; }
        public virtual DbSet<PhoneParam> PhoneParams { get; set; }
        public virtual DbSet<RackInfo> RackInfos { get; set; }
        public virtual DbSet<RunStatus> RunStatuses { get; set; }
        public virtual DbSet<StationGroup> StationGroups { get; set; }
        public virtual DbSet<StopRegion> StopRegions { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<TaskQueue> TaskQueues { get; set; }
        public virtual DbSet<TrafficManager> TrafficManagers { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgvchargeStation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AGVChargeStation");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.ChargeStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Agvinfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AGVInfo");

                entity.Property(e => e.Agvbattery).HasColumnName("AGVBattery");

                entity.Property(e => e.Agvindex)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVIndex")
                    .IsFixedLength();

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.Agvnumber)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVNumber")
                    .IsFixedLength();

                entity.Property(e => e.Agvstatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVStatus")
                    .IsFixedLength();

                entity.Property(e => e.Agvtype)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVType")
                    .IsFixedLength();

                entity.Property(e => e.AgvwarningStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVWarningStatus")
                    .IsFixedLength();

                entity.Property(e => e.AutoManual)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CurrentTagNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DemoMode)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DoTaskName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Stage1Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Stage1CSTID")
                    .IsFixedLength();

                entity.Property(e => e.Stage2Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Stage2CSTID")
                    .IsFixedLength();

                entity.Property(e => e.Stage3Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Stage3CSTID")
                    .IsFixedLength();

                entity.Property(e => e.Stage4Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Stage4CSTID")
                    .IsFixedLength();
            });

            modelBuilder.Entity<AgvmaintainTable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AGVMaintainTable");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.LastMaintainTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AlarmResetLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AlarmResetLog");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.AlarmCode)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CauseBy)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CommentByEventCodeTable)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CommentFromAgv)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("CommentFromAGV")
                    .IsFixedLength();

                entity.Property(e => e.EventCode)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OccurTime).HasColumnType("datetime");

                entity.Property(e => e.Position)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ResetTime).HasColumnType("datetime");

                entity.Property(e => e.ResetType)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<CartInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CartInfo");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Size)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<CartMaintainTable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CartMaintainTable");

                entity.Property(e => e.CartName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.LastMaintainTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ChargeStationStatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ChargeStationStatus");

                entity.Property(e => e.ChargeStationName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Enable)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Dispatch>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Dispatch");

                entity.Property(e => e.AliveCheckIndex)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CallEmptyCart)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ClearFlowTask)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ClearTrafficRegistData)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Fire)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ResendTaskMessage)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StationName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<EarthquakeLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EarthquakeLog");

                entity.Property(e => e.Ip)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("IP")
                    .IsFixedLength();

                entity.Property(e => e.IsSendToAgv)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("IsSendToAGV")
                    .IsFixedLength();

                entity.Property(e => e.Message)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.OccurTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ElevatorInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ElevatorInfo");

                entity.Property(e => e.DoTaskName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.EDoorStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("E_Door_Status")
                    .IsFixedLength();

                entity.Property(e => e.EFloorStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("E_Floor_Status")
                    .IsFixedLength();

                entity.Property(e => e.ElevatorIndex)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ElevatorName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ElevatorStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ModeStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Mode_Status")
                    .IsFixedLength();
            });

            modelBuilder.Entity<ElevatorWithAgvinfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ElevatorWithAGVInfo");

                entity.Property(e => e.ElevatorName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ExecuteAgv)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("ExecuteAGV")
                    .IsFixedLength();

                entity.Property(e => e.ExecuteGoFloor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<FireAdaminfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FireADAMInfo");

                entity.Property(e => e.Channel)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Enable)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.InstallLocation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Ioevent)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("IOEvent")
                    .IsFixedLength();

                entity.Property(e => e.ReceiveTime).HasColumnType("datetime");

                entity.Property(e => e.Station)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StatusFlag)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<FireAlarmLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FireAlarmLog");

                entity.Property(e => e.Channel)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.IsSendToAgv)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("IsSendToAGV")
                    .IsFixedLength();

                entity.Property(e => e.OccurTime).HasColumnType("datetime");

                entity.Property(e => e.Station)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StatusFlag)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Hidlog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("HIDLog");

                entity.Property(e => e.Action)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.BoxId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BoxID")
                    .IsFixedLength();

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.Hid)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("HID")
                    .IsFixedLength();

                entity.Property(e => e.PortName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<MaterialStationInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MaterialStationInfo");

                entity.Property(e => e.AutoManualMode)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AvaliableCartSize)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.BoxId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BoxID")
                    .IsFixedLength();

                entity.Property(e => e.Enable)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Flag)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Lift)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Lock)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.MaterialStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.MaterialStationType)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NeedTagCart)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RecvTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Recv_Time");

                entity.Property(e => e.Size)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<MaterialWip>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MaterialWIP");

                entity.Property(e => e.BoxId)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BoxID")
                    .IsFixedLength();

                entity.Property(e => e.BoxIdtype)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("BoxIDType")
                    .IsFixedLength();

                entity.Property(e => e.Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CSTID")
                    .IsFixedLength();

                entity.Property(e => e.IsGdas)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("IsGDAS")
                    .IsFixedLength();

                entity.Property(e => e.MaterialStation)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Mrlist)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("MRList")
                    .IsFixedLength();

                entity.Property(e => e.Wipflag)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("WIPFlag")
                    .IsFixedLength();
            });

            modelBuilder.Entity<PhoneParam>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PhoneParam");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PhoneNo)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<RackInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RackInfo");

                entity.Property(e => e.ExistFlag)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RackName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RackNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<RunStatus>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RunStatus");

                entity.Property(e => e.Agvindex).HasColumnName("AGVIndex");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.Agvstatus).HasColumnName("AGVStatus");

                entity.Property(e => e.RecDateTime).HasColumnType("datetime");

                entity.Property(e => e.StartDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<StationGroup>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("StationGroup");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StationName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<StopRegion>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("StopRegion");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.SameDirectionAgv)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("SameDirectionAGV")
                    .IsFixedLength();

                entity.Property(e => e.StopRegion1)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("StopRegion")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Task");

                entity.Property(e => e.AlarmCode)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.AreaName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ArriveToStationTime)
                    .HasColumnType("datetime")
                    .HasColumnName("ArriveToStation_Time");

                entity.Property(e => e.CostTime).HasColumnName("Cost_Time");

                entity.Property(e => e.Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CSTID")
                    .IsFixedLength();

                entity.Property(e => e.DispatchDelete)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DispatchStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("End_Time");

                entity.Property(e => e.ExecuteAgv)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("ExecuteAGV")
                    .IsFixedLength();

                entity.Property(e => e.ExecuteTaskStartPosition)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FromAction)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FromStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.JobNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.MotorEncoder).HasColumnType("text");

                entity.Property(e => e.Mrnum)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("MRNum")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PhoneNumber).HasColumnType("text");

                entity.Property(e => e.Priority)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ReceiveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Receive_Time");

                entity.Property(e => e.ReturnDispatchStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ReturnDispatchTime).HasColumnType("datetime");

                entity.Property(e => e.ReturnValue)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Start_Time");

                entity.Property(e => e.ToAction)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ToStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<TaskQueue>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TaskQueue");

                entity.Property(e => e.AreaName)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Cstid)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CSTID")
                    .IsFixedLength();

                entity.Property(e => e.DispatchDelete)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.DispatchStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasColumnName("End_Time");

                entity.Property(e => e.ExecuteAgv)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("ExecuteAGV")
                    .IsFixedLength();

                entity.Property(e => e.ExecuteTaskStartPosition)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FromAction)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FromStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.JobNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.MotorEncoder).HasColumnType("text");

                entity.Property(e => e.Mrnum)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("MRNum")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PhoneNumber).HasColumnType("text");

                entity.Property(e => e.Priority)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ReceiveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Receive_Time");

                entity.Property(e => e.ReturnDispatchStatus)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ReturnDispatchTime).HasColumnType("datetime");

                entity.Property(e => e.ReturnValue)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("Start_Time");

                entity.Property(e => e.ToAction)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ToStation)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<TrafficManager>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TrafficManager");

                entity.Property(e => e.Agvname)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AGVName")
                    .IsFixedLength();

                entity.Property(e => e.ExecuteRoute).HasColumnType("text");

                entity.Property(e => e.Region)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RegistTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UserInfo");

                entity.Property(e => e.UserLayer)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}