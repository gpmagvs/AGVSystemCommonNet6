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
        CommandName_Does_Not_Exist = 0x01,
        Cannot_Perform_Now = 0x02,
        At_least_one_parameter_is_invalid = 0x03,
        Acknowledge_command_will_be_perform_with_completion_signaled_later_by_an_event = 0x04,
        Rejected_Already_in_desired_condition = 0x05,
        Not_such_object_exists = 0x06,
        CPNAME_and_CPVAL_Is_Insufficient = 64,
        System_Error = 65,
        Cannot_Find_Seat_For_The_Carrier_In_Rack = 0x2E,
    }

    public enum HCACK_RETURN_CODE_YELLOW : byte
    {
        CommandName_Does_Not_Exist = 0x01,
        System_Exception = 0x65,
        Control_State_Not_At_Online_Remote = 0x70,
        Cannot_Create_Command_By_Exception = 0x71,
        Not_Assign_Source_Port = 0x72,
        Cannot_Find_Source_Port = 0x73,
        Not_Assign_Destination_Port = 0x74,
        Cannot_Find_Destination_Port = 0x75,
        Cannot_Perform_Unload_For_Source_Port = 0x76,
        Cannot_Perform_Load_For_Destination_Port = 0x77,
        Save_Command_Data_To_Database_Fail = 0x78,
        Source_Port_Assign_Wrong_AGV = 0x79,
        AGV_Cargo_Status_Mismatch_With_Command = 0x7A,
        Wrong_Command_Data_Cause_Exception = 0x7B,
        Signal_Is_Not_Load_Request_For_Destination_Port = 0x7C,
        Signal_Is_Not_Unload_Request_For_Source_Port = 0x7D,
        Cannot_Detect_Carrier_Exist_For_Source_Port = 0x7E,
        Equipment_Status_Is_Down = 0x80,
        Detect_Carrier_Exist_For_Destination_Port = 0x81,
        Equipment_Load_Request_And_Unload_Request_Both_ON = 0x82,
        Equipment_Load_Request_And_Unload_Request_Both_OFF = 0x83,
        Unknown_Conditions = 0x84,
        Cannot_Find_The_Carrier_ID_In_Rack = 0x85,
        Cannot_Find_Seat_For_The_Carrier_In_Rack = 0x86,
        Not_Assign_Vehicle_For_Abnormal_Transfer = 0x87,
        Source_Port_Assign_AGV_But_Already_Exist_Same_Command = 0x88,
        Not_Assign_Carrier_ID_For_Command = 0x89,
        Rack_Source_Port_Already_Has_Task = 0x8A,
        Rack_Source_Port_Position_Is_Disable = 0x8B,
        Rack_Source_Port_Exist_Multiple_Same_Carrier_Id = 0x8C,
        Rack_Destination_Port_Already_Has_Task = 0x8D,
        Rack_Destination_Port_Position_Is_Disable = 0x8F,
        Rack_Destination_Port_Already_Has_Data = 0x90,
        Source_Port_Unload_Request_Is_OFF = 0x91,
        Destination_Port_Load_Request_Is_OFF = 0x92,
        Source_Port_Is_Disconnected = 0x93,
        Destination_Port_Is_Disconnected = 0x94,
        Rack_Sensor_Is_Error = 0x95
    }
}
