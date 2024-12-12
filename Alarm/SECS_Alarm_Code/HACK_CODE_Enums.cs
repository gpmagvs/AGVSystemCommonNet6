using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Alarm.SECS_Alarm_Code.Enums
{

    public enum HCACK_RETURN_CODE_GPM : byte
    {
        Success = 0x00,
        OtherErrors = 0x01,
        ZoneIsFull = 0x02,
        DuplicateID = 0x03,
        IDMissmatch = 0x04,
        IDReadFail = 0x05,
        InterlockError = 0x64,


    }

    public enum HCACK_RETURN_CODE_YELLOW : byte
    {
        CommandName_Does_Not_Exist = 0x01,
        Control_State_Not_At_Online_Remote = 0x02,
        Cannot_Create_Command_By_Exception = 0x0A,
        Not_Assign_Source_Port = 0x0C,
        Cannot_Find_Source_Port = 0x0D,
        Not_Assign_Destination_Port = 0x0E,
        Cannot_Find_Destination_Port = 0x0F,
        Cannot_Perform_Unload_For_Source_Port = 0x10,
        Cannot_Perform_Load_For_Destination_Port = 0x13,
        Save_Command_Data_To_Database_Fail = 0x15,
        Source_Port_Assign_Wrong_AGV = 0x1A,
        AGV_Cargo_Status_Mismatch_With_Command = 0x1C,
        Wrong_Command_Data_Cause_Exception = 0x1D,
        Signal_Is_Not_Load_Request_For_Destination_Port = 0x1F,
        Signal_Is_Not_Unload_Request_For_Source_Port = 0x20,
        Cannot_Detect_Carrier_Exist_For_Source_Port = 0x21,
        System_Exception = 0x22,
        Equipment_Status_Is_Down = 0x23,
        Detect_Carrier_Exist_For_Destination_Port = 0x24,
        Equipment_Load_Request_And_Unload_Request_Both_ON = 0x25,
        Equipment_Load_Request_And_Unload_Request_Both_OFF = 0x26,
        Unknown_Conditions = 0x27,
        Cannot_Find_The_Carrier_ID_In_Rack = 0x2D,
        Cannot_Find_Seat_For_The_Carrier_In_Rack = 0x2E,
        Not_Assign_Vehicle_For_Abnormal_Transfer = 0x32,
        Source_Port_Assign_AGV_But_Already_Exist_Same_Command = 0x34,
        Not_Assign_Carrier_ID_For_Command = 0x37,
        Rack_Source_Port_Already_Has_Task = 0x64,
        Rack_Source_Port_Position_Is_Disable = 0x65,
        Rack_Source_Port_Exist_Multiple_Same_Carrier_Id = 0x66,
        Rack_Destination_Port_Already_Has_Task = 0x6E,
        Rack_Destination_Port_Position_Is_Disable = 0x6F,
        Rack_Destination_Port_Already_Has_Data = 0x70,
        Source_Port_Unload_Request_Is_OFF = 0x7C,
        Destination_Port_Load_Request_Is_OFF = 0x7D,
        Source_Port_Is_Disconnected = 0x7E,
        Destination_Port_Is_Disconnected = 0x7F,
        Rack_Sensor_Is_Error = 0x80
    }
}
