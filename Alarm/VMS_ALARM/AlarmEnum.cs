﻿namespace AGVSystemCommonNet6.Alarm.VMS_ALARM
{
    public enum AlarmCodes
    {
        None = 0,
        SoftwareEMS = 10,
        EMS = 20,
        Safety_Relays = 30,
        EMO_Button = 31,
        Bumper = 40,
        Zaxis_Up_Limit = 50,
        Zaxis_Down_Limit = 60,
        Wago_IO_Disconnect = 100,
        Wago_IO_Read_Fail = 120,
        Wago_IO_Write_Fail = 130,
        PLC_MC_Disconnect = 150,
        PLC_MC_Initial_Fail = 160,
        Not_Yet_Initial = 200,
        Battery_Not_Lock = 220,
        Battery1_Not_Lock = 221,
        Battery2_Not_Lock = 222,
        Action_Timeout = 250,
        Auto_Reset_Timeout = 260,
        Lock_Action_Timeout = 270,
        Lock1_Action_Timeout = 271,
        Lock2_Action_Timeout = 272,
        Unlock_Action_Timeout = 280,
        Unlock1_Action_Timeout = 281,
        Unlock2_Action_Timeout = 282,
        Wheel_Motor_IO_Error = 300,
        Wheel_Motor_IO_Error_Left = 301,
        Wheel_Motor_IO_Error_Right = 302,
        Wheel_Motor_IO_Error_Left_Front = 303,
        Wheel_Motor_IO_Error_Right_Front = 304,
        Wheel_Motor_IO_Error_Left_Rear = 305,
        Wheel_Motor_IO_Error_Right_Rear = 306,
        Wheel_Motor_Alarm = 310,
        Vertical_Motor_IO_Error = 400,
        Vertical_Motor_Alarm = 410,
        Barcode_Module_Error = 500,
        IMU_Module_Error = 510,
        Guide_Module_Error = 520,
        Pin_Module_Communication_Error = 530,
        Front_Pin_Module_Communication_Error = 531,
        Back_Pin_Module_Communication_Error = 532,
        Pin_Module_Pos_Check_Fail = 540,
        Front_Pin_Module_Pos_Check_Fail = 541,
        Back_Pin_Module_Pos_Check_Fail = 542,
        Pin_Action_Error = 550,
        Pin_Pose_Fail = 560,
        Front_Pin_Pose_not_up = 561,
        Back_Pin_Pose_not_up = 562,
        Front_Pin_Pose_not_down = 563,
        Back_Pin_Pose_not_down = 564,
        Belt_Sensor_Error = 600,
        Belt_Sensor_Error_1 = 601,
        Belt_Sensor_Error_2 = 602,
        Belt_Sensor_Error_3 = 603,
        Belt_Sensor_Error_4 = 604,
        Pin_IO_Error = 610,
        Oh_Over_Load = 620,
        Oh_Sway_Error_ = 630,
        Oh_Pos_Check_Fail = 640,
        Fork_IO_Error = 700,
        Fork_Bumper_Error = 710,
        Fork_Bumper_Error_Up = 711,
        Fork_Bumper_Error_Down = 712,
        Fork_Height_Setting_Error = 713,
        Ground_Hole_Sensor_Error = 800,
        Ground_Hole_Sensor_Error_Left_Front = 801,
        Ground_Hole_Sensor_Error_Right_Front = 802,
        Ground_Hole_Sensor_Error_Left_Rear = 803,
        Ground_Hole_Sensor_Error_Right_Rear = 804,
        Ground_Hole_CCD_Error = 810,
        Ground_Hole_CCD_Error_Left_Front = 811,
        Ground_Hole_CCD_Error_Right_Front = 812,
        Ground_Hole_CCD_Error_Left_Rear = 813,
        Ground_Hole_CCD_Error_Right_Rear = 814,
        Smoke_IO_Error = 820,
        N2_IO_Error = 830,
        Battery_Exist_Error_ = 840,
        Battery1_Exist_Error = 841,
        Battery2_Exist_Error = 842,
        Battery_Low_Volume = 850,
        Battery1_Low_Volume = 851,
        Battery2_Low_Volume_ = 852,
        Battery_Volume_Gap = 860,
        Battery1_Volume_Gap = 861,
        Battery2_Volume_Gap = 862,
        Ultrasonic_Sensor_Error = 870,
        Ultrasonic_Sensor_Error_Front = 871,
        Ultrasonic_Sensor_Error_Rear = 872,
        Ultrasonic_Sensor_Error_Left = 873,
        Ultrasonic_Sensor_Error_Right = 874,
        Ultrasonic_Sensor_Error_Far = 880,
        Ultrasonic_Sensor_Error_Front_Far = 881,
        Ultrasonic_Sensor_Error_Rear_Far = 882,
        Ultrasonic_Sensor_Error_Left_Far = 883,
        Ultrasonic_Sensor_Error_Right_Far = 884,
        Switch_Type_Error = 1000,
        Switch_Type_Error_Vertical = 1001,
        FrontProtection_Area3 = 1010,
        BackProtection_Area3 = 1020,
        LeftProtection_Area3 = 1030,
        RightProtection_Area3 = 1040,
        Cant_Change_With_Wrong_State = 1050,
        Monitor_Switch_Error_ = 1060,
        Cant_Online_Without_Tag = 1070,
        /// <summary>
        /// 前方遠處障礙物
        /// </summary>
        FrontProtection_Area2 = 1110,
        BackProtection_Area2 = 1120,
        LeftProtection_Area2 = 1130,
        RightProtection_Area2 = 1140,
        BotProtection_Area2 = 1150,
        Side_Sensor_Undetected = 1200,
        AGV_State_Cant_do_this_Action = 1500,
        AGV_State_Cant_Move = 1501,
        Cant_Move_in_Work_Station = 1510,
        Work_Station_Setting_Fail = 1520,
        Cant_Initial_when_Obstacle_Invade = 1530,
        AGV_State_Cant_Auto_Reset = 1540,
        Secondary_Done_But_is_Wrong_Pos = 1550,
        Cant_Manual_Action = 1560,
        Cant_Manual_Action_1 = 1561,
        Cant_Manual_Action_2 = 1562,
        Cant_Manual_Action_3 = 1563,
        Cant_Check_ZaxisPos0A = 1600,
        Cant_Check_ZaxisPos_IO = 1601,
        Cant_Check_ZaxisPos_Encoder = 1602,
        Cant_Check_ZaxisPos_Laser = 1603,
        Cant_Check_ZaxisPos_1 = 1604,
        Cant_Check_ZaxisPos_2 = 1605,
        Cant_Check_ZaxisPos_3 = 1606,
        Cst_Slope_Error_ = 1610,
        Cst_Slope_Error_1 = 1611,
        Cst_Slope_Error_2 = 1612,
        Cst_Slope_Error_3 = 1613,
        Cst_Slope_Error_4 = 1614,
        Cst_Slope_Error_5 = 1615,
        Cst_Slope_Error_6 = 1616,
        Oh_Cst_Error = 1620,
        Oh_Cst_Error0 = 1621,
        Cant_Check_Battery = 1630,
        Cant_Check_Move_Path = 1640,
        Has_Job_Without_Cst = 1700,
        Has_Job_Without_Cst_1 = 1701,
        Has_Job_Without_Cst_2 = 1702,
        Has_Job_Without_Cst_3 = 1703,
        Has_Job_Without_Cst_4 = 1704,
        Delete_Job_Data_With_Cst = 1710,
        Delete_Job_Data_With_Cst_1 = 1711,
        Delete_Job_Data_With_Cst_2 = 1712,
        Delete_Job_Data_With_Cst_3 = 1713,
        Delete_Job_Data_With_Cst_4 = 1714,
        Has_Cst_Without_Job = 1720,
        Has_Cst_Without_Job_1 = 1721,
        Has_Cst_Without_Job_2 = 1722,
        Has_Cst_Without_Job_3 = 1723,
        Has_Cst_Without_Job_4 = 1724,
        Oh_Action_but_Empty_Take = 1750,
        Load_But_Occupy = 1800,
        Read_Cst_ID_Fail = 1850,
        Cst_ID_Not_Match = 1860,
        Instrument_Error = 1900,
        Instrument_IO_Error_ = 1901,
        Instrument_Module_Error_ = 1902,
        Instrument_Module_Disconnect = 1903,
        Detection_Fail = 1910,
        Bay_Block = 1920,
        Bay_Block_1 = 1921,
        Bay_Block_2 = 1922,
        Bay_Move_Out_Fail = 1950,
        Escape = 1960,
        GPM_Motion_control_Tracking_Error = 2000,
        Motion_control_Wrong_Received_Msg = 2010,
        Motion_control_Wrong_Extend_Path = 2020,
        Motion_control_Out_Of_Line_While_Forwarding_End = 2030,
        Motion_control_Out_Of_Line_While_Tracking_End_Point = 2040,
        Motion_control_Out_Of_Line_While_Moving = 2050,
        Motion_control_Out_Of_Line_While_Secondary = 2060,
        Motion_control_Missing_Tag_On_End_Point = 2070,
        Motion_control_Missing_Tag_While_Moving = 2080,
        Motion_control_Missing_Tag_While_Secondary = 2090,
        Motion_control_Wrong_Initial_Position_In_Secondary = 2100,
        Motion_control_Wrong_Initial_Angle_In_Secondary = 2110,
        Motion_control_Wrong_Target_To_Pirouette = 2120,
        Map_Recognition_Rate_Too_Low = 2130,
        Motion_control_Tracking_Tag_Not_On_Tag_Or_Tap = 2200,
        Motion_control_Wrong_Position_To_Tracking_Tag = 2210,
        Motion_control_Tracking_Tag_Fail = 2220,
        F0GPM_Motion_control_RunMove_Abort = 2400,
        Can_not_Pass_Task_to_Motion_Control = 2401,
        F1Action_Server_Not_Ready0F1Action = 2410,
        FATask_Path_Road_Closed = 2500,
        AGV_Alive_Check_Fail = 3000,
        Registration_Reject = 3040,
        Registration_Timeout = 3050,
        Unregistration_Reject = 3070,
        Unregistration_Timeout = 3080,
        Handshake_Fail = 3200,
        Handshake_Fail_EQ_L_REQ = 3201,
        Handshake_Fail_EQ_U_REQ = 3202,
        Handshake_Fail_EQ_READY = 3203,
        Handshake_Fail_EQ_READY_UP = 3204,
        Handshake_Fail_EQ_READY_LOW = 3205,
        Handshake_Fail_EQ_GO = 3206,
        Handshake_Fail_Because_Person_EQ_GO = 3207,
        Handshake_Fail_Inside_EQ_EQ_GO = 3208,
        Handshake_Timeout = 3210,
        Precheck_IO_Fail_X20 = 3220,
        Precheck_IO_Fail_EQ_L_REQ = 3221,
        Precheck_IO_Fail_EQ_U_REQ = 3222,
        Precheck_IO_Fail_EQ_READY = 3223,
        Precheck_IO_Fail_EQ_READY_UP = 3224,
        Precheck_IO_Fail_EQ_READY_LOW = 3225,
        Precheck_IO_Fail_EQ_GO = 3226,
        Handshake_T1_Timeout = 3230,
        Handshake_Information_Fail = 3240,
        Handshake_Information_Fail_EQ_L_REQ = 3241,
        Handshake_Information_Fail_EQ_U_REQ = 3242,
        AAsk_Remote_Cst_Status_Fail = 3300,
        BAsk_Remote_Cst_Status_Timeout = 3310,
        AGVs_Abort_Task = 3500,
        Under_voltage_protection = 4010,
        Under_voltage_protection_Action = 4011,
        Under_voltage_protection_1 = 4012,
        Under_voltage_protection_2 = 4013,
        Under_voltage_protection_3 = 4014,
        Under_voltage_protection_4 = 4015,
        Under_current_protection = 4020,
        Under_current_protection_Action = 4021,
        Under_current_protection_1 = 4022,
        Under_current_protection_2 = 4023,
        Under_current_protection_3 = 4024,
        Under_current_protection_4 = 4025,
        Over_voltage_protection = 4030,
        Over_voltage_protection_Action = 4031,
        Over_voltage_protection_1 = 4032,
        Over_voltage_protection_2 = 4033,
        Over_voltage_protection_3 = 4034,
        Over_voltage_protection_4 = 4035,
        Over_current_protection = 4040,
        Over_current_protection_Action = 4041,
        Over_current_protection_1 = 4042,
        Over_current_protection_2 = 4043,
        Over_current_protection_3 = 4044,
        Over_current_protection_4 = 4045,
        Over_heat_protection = 4050,
        Over_heat_protection_Action = 4051,
        Over_heat_protection_1 = 4052,
        Over_heat_protection_2 = 4053,
        Over_heat_protection_3 = 4054,
        Over_heat_protection_4 = 4055,
        Over_load_protection = 4060,
        Over_load_protection_Action = 4061,
        Over_load_protection_1 = 4062,
        Over_load_protection_2 = 4063,
        Over_load_protection_3 = 4064,
        Over_load_protection_4 = 4065,
        Over_regeneration_load_protection = 4070,
        Over_regeneration_load_protection_Action = 4071,
        Over_regeneration_load_protection_2 = 4072,
        Over_regeneration_load_protection_3 = 4073,
        Over_regeneration_load_protection_4 = 4074,
        Over_regeneration_load_protection_5 = 4075,
        Over_speed_protection = 4080,
        Over_speed_protection_Action = 4081,
        Over_speed_protection_1 = 4082,
        Over_speed_protection_2 = 4083,
        Over_speed_protection_3 = 4084,
        Over_speed_protection_4 = 4085,
        Deviation_excess_protection = 4090,
        Deviation_excess_protection_Action = 4091,
        Deviation_excess_protection_1 = 4092,
        Deviation_excess_protection_2 = 4093,
        Deviation_excess_protection_3 = 4094,
        Deviation_excess_protection_4 = 4095,
        AConnection_error_protection = 4100,
        AConnection_error_protection_Action = 4101,
        AConnection_error_protection_1 = 4102,
        AConnection_error_protection_2 = 4103,
        AConnection_error_protection_3 = 4104,
        AConnection_error_protection_4 = 4105,
        Status_Error = 4110,
        Status_Error_Action = 4111,
        Status_Error_1 = 4112,
        Status_Error_2 = 4113,
        Status_Error_3 = 4114,
        Status_Error_4 = 4115,
        Communication_error = 4120,
        Communication_error_Action = 4121,
        Communication_error_1 = 4122,
        Communication_error_2 = 4123,
        Communication_error_3 = 4124,
        Communication_error_4 = 4125,
        Alarm_input_protection = 4130,
        Alarm_input_protection_Action = 4131,
        Alarm_input_protection_1 = 4132,
        Alarm_input_protection_2 = 4133,
        Alarm_input_protection_3 = 4134,
        Alarm_input_protection_4 = 4135,
        Command_error = 4140,
        Command_error_Action = 4141,
        Command_error_1 = 4142,
        Command_error_2 = 4143,
        Command_error_3 = 4144,
        Command_error_4 = 4145,
        Overtorque = 4150,
        Overtorque_Action = 4151,
        Overtorque_1 = 4152,
        Overtorque_2 = 4153,
        Overtorque_3 = 4154,
        Overtorque_4 = 4155,
        Other_error = 4160,
        Other_error_Action = 4161,
        Other_error_1 = 4162,
        Other_error_2 = 4163,
        Other_error_3 = 4164,
        Other_error_4 = 4165,
        Motor_Active_Error = 4166,
        Motor_Feedback_Signal_Error = 4167,
        Motor_EEP_Error = 4168,
        Motor_Driver_Over_Heat_Error = 4169,
        Motor_Over_Speed_Error = 4170,
        Motor_Encoder_Error = 4171,
        Motor_Run_Forbid = 4172,
        Motor_Extern_Stop = 4173,
        Motor_Hall_Sequence_Error = 4174,
        Motor_Parameters_Error = 4175,

        Battery_Status_Error_ = 5000,
        Battery_Status_Error_1 = 5001,
        Battery_Status_Error_2 = 5002,
        Over_Voltage = 5010,
        Over_Voltage_1 = 5011,
        Over_Voltage_2 = 5012,
        Under_Voltage = 5020,
        Under_Voltage_1 = 5021,
        Under_Voltage_2 = 5022,
        Over_Current_Charge = 5030,
        Over_Current_Charge_1 = 5031,
        Over_Current_Charge_2 = 5032,
        Over_Current_Discharge = 5040,
        Over_Current_Discharge_1 = 5041,
        Over_Current_Discharge_2 = 5042,
        Under_Current_Charge = 5050,
        Under_Current_Charge_1 = 5051,
        Under_Current_Charge_2 = 5052,
        Under_Current_Discharge = 5060,
        Under_Current_Discharge_1 = 5061,
        Under_Current_Discharge_2 = 5062,
        Over_Temperature = 5070,
        Over_Temperature_1_ = 5071,
        Over_Temperature_2_ = 5072,
        Under_Temperature = 5080,
        Under_Temperature_1_ = 5081,
        Under_Temperature_2_ = 5082,
        License_Error = 9990,
        License_Error_0x0000NOF3E7 = 9991,
        License_Error_0x0000NRD3E7 = 9992,
        License_Error_0x00000M13E7 = 9993,
        License_Error_0x00000D13E7 = 9994,
        Waiting_EQ_Handshake = 10000,
        Laser_Mode_value_fail = 10010,
        Code_Error_In_System = 860002,
        Subscribe_ROS_Topic_Err = 860004,
        ROS_Bridge_server_Disconnect = 860005,
        AGV_Pose_Angle_Not_Match_To_Sick_Angle = 860006,
        USER_Press_SoftwareEMO_But_Exception_Occur = 860007,
        AGVS_Disconnect = 860008,
        Cant_Get_Online_Status_From_AGVS = 860009,
        Task_Feedback_T1_Timeout = 860010,
        EQP_LOAD_BUT_EQP_HAS_OBSTACLE = 860011,
        EQP_UNLOAD_BUT_EQP_HAS_NO_CARGO = 860012,
        Cant_Change_Online_Mode_Code_Error = 860013,
        Cant_TransferTask_TO_AGVC = 860014,
        Motion_control_Wrong_Unknown_Code = 860018,
        UserAbort_Initialize_Process = 860019,
        Sick_Lidar_Communication_Error = 860020,
        Fork_WorkStation_Teach_Data_Not_Found_Tag = 860021,
        Fork_WorkStation_Teach_Data_Not_Found_layer = 860022,
        Fork_Arm_Pose_Error = 860023,
        Destine_Point_Is_Virtual_Point = 860024,
        Fork_Frontend_has_Obstacle = 860025,
        Fork_Has_Cargo_But_Initialize_Running = 860026,
        Fork_Disabled = 860027,
        AGV_Cant_Goto_Workstation_When_Fork_Is_Disabled = 860028,
        Unknown = 404404,

        Handshake_Fail_EQ_BUSY_ON = 810015,
        Handshake_Fail_EQ_BUSY_OFF = 810016,
        Handshake_Fail_AGV_DOWN = 810017,
        Handshake_Fail_EQ_Busy_ON_When_AGV_BUSY = 810018,
        Handshake_Fail_EQ_READY_OFF_When_AGV_BUSY = 810019,
        Fork_Initialize_Process_Interupt = 860029,
        Fork_Initialize_Process_Error_Occur = 860030,
        Fork_Initialized_But_Home_Input_Not_ON = 860031,
        Fork_Go_Home_But_Home_Sensor_Signal_Error = 860032,
        Fork_Initialized_But_Driver_Position_Not_ZERO = 860033,
        Handshake_Fail_EQ_BUSY_NOT_ON = 860034,
        Handshake_Fail_EQ_BUSY_NOT_OFF = 860035,
        Handshake_Fail_TA1_EQ_L_REQ = 460036,
        Handshake_Fail_TA1_EQ_U_REQ ,
        Handshake_Fail_TA2_EQ_READY ,
        Handshake_Fail_TA3_EQ_BUSY_ON ,
        Handshake_Fail_TA4_EQ_BUSY_OFF ,
        Handshake_Fail_TA5_EQ_L_REQ ,
        Handshake_Fail_TA5_EQ_U_REQ ,
        Laser_Mode_Switch_Fail_Exception,
        Laser_Mode_Switch_Fail_Timeout,
    }
}
