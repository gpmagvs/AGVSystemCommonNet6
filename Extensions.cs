using AGVSystemCommonNet6.AGVDispatch;
using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using AGVSystemCommonNet6.Configuration;
using AGVSystemCommonNet6.Log;
using AGVSystemCommonNet6.MAP;
using KGSWebAGVSystemAPI.Sys;
using Newtonsoft.Json;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using static AGVSystemCommonNet6.clsEnums;
using static AGVSystemCommonNet6.MAP.MapPoint;

namespace AGVSystemCommonNet6
{
    public static class Extensions
    {
        private static Map _mapUse = null;
        private static Map mapUse
        {
            get
            {
                if (_mapUse == null)
                {
                    string mapPath = KGSSettingsHelper.GetCurrentMapUseFilePath();
                    _mapUse = MapManager.LoadMapFromFile(mapPath, out string errMsg, auto_create_segment: false, auto_check_path_error: false);
                }
                return _mapUse;
            }
        }

        public static Time ToStdTime(this DateTime _time)
        {
            return new Time()
            {
                secs = (uint)(_time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds),
                nsecs = (uint)(_time.Millisecond * 1000000),
            };
        }

        public static string ToAGVSTimeFormat(this DateTime _time)
        {
            return _time.ToString("yyyyMMdd HH:mm:ss");
        }

        /// <summary>
        /// 將角度值轉換為 Quaternion(四位元)
        /// </summary>
        /// <param name="Theta"></param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(this double Theta)
        {
            double yaw_radians = (float)Theta * Math.PI / 180.0;
            double cos_yaw = Math.Cos(yaw_radians / 2.0);
            double sin_yaw = Math.Sin(yaw_radians / 2.0);
            return new Quaternion(0.0f, 0.0f, (float)sin_yaw, (float)cos_yaw);
        }

        public static int[] GetRemainPath(this IEnumerable<clsMapPoint> points, int startTag)
        {
            if (points.Count() == 0)
                return new int[1] { startTag };

            int find_index(int tag)
            {
                return points.ToList().FindIndex(p => p.Point_ID == tag);
            }
            var startTag_index = find_index(startTag);
            return points.ToList().FindAll(p => find_index(p.Point_ID) >= startTag_index).Select(pt => pt.Point_ID).ToArray();
        }

        public static double ToTheta(this RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion orientation)
        {
            double yaw;
            double x = orientation.x;
            double y = orientation.y;
            double z = orientation.z;
            double w = orientation.w;
            // 計算角度
            double siny_cosp = 2.0 * (w * z + x * y);
            double cosy_cosp = 1.0 - 2.0 * (y * y + z * z);
            yaw = Math.Atan2(siny_cosp, cosy_cosp);
            return yaw * 180.0 / Math.PI;
        }
        public static string ToJson(this object obj, Formatting format = Formatting.Indented)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, format);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return "{}";
            }
        }
        public static T Clone<T>(this T obt)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(obt.ToJson(Formatting.None));
            }
            catch (Exception)
            {
                return obt;
            }
        }
        public static string GetString(this byte[] byte_data, Encoding encoder)
        {
            try
            {
                return encoder.GetString(byte_data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return "{}";
            }
        }


        /// <summary>
        /// 該站點是否可充電
        /// </summary>
        /// <param name="map_station"></param>
        /// <returns></returns>
        public static bool IsChargeAble(this MapPoint map_station)
        {
            STATION_TYPE station_type = map_station.StationType;
            return station_type == STATION_TYPE.Charge || station_type == STATION_TYPE.Charge_Buffer || station_type == STATION_TYPE.Charge_STK;
        }

        /// <summary>
        /// 該站點是否可供AGV Load/Unload
        /// </summary>
        /// <param name="map_station"></param>
        /// <returns></returns>
        public static bool IsLoadAble(this MapPoint map_station)
        {
            STATION_TYPE station_type = map_station.StationType;
            return station_type == STATION_TYPE.EQ || station_type == STATION_TYPE.EQ_LD
                    || station_type == STATION_TYPE.STK || station_type == STATION_TYPE.STK_LD
                    || station_type == STATION_TYPE.Charge_STK;
        }
        /// <summary>
        /// 該站點是否可供AGV Load/Unload
        /// </summary>
        /// <param name="map_station"></param>
        /// <returns></returns>
        public static bool IsUnloadAble(this MapPoint map_station)
        {
            STATION_TYPE station_type = map_station.StationType;
            return station_type == STATION_TYPE.EQ || station_type == STATION_TYPE.EQ_ULD
                    || station_type == STATION_TYPE.STK || station_type == STATION_TYPE.STK_ULD
                    || station_type == STATION_TYPE.Charge_STK;
        }
        /// <summary>
        /// 計算與站點的距離
        /// </summary>
        /// <param name="map_station"></param>
        /// <param name="from_loc_x"></param>
        /// <param name="from_loc_y"></param>
        /// <returns></returns>
        public static double CalculateDistance(this MapPoint map_station, double from_loc_x, double from_loc_y, int tag = -1)
        {
            if (map_station == null)
                return -1;
            double distance = Math.Sqrt(Math.Pow(map_station.X - from_loc_x, 2) + Math.Pow(map_station.Y - from_loc_y, 2));
            //Console.WriteLine($"Distance from ({map_station.X},{map_station.Y}) (Tag:{map_station.TagNumber}) to ({from_loc_x},{from_loc_y})(Tag:{tag}) is {distance}");
            return distance;
        }
        public static double CalculateDistance(this MapPoint map_station, clsCoordination coord)
        {
            return map_station.CalculateDistance(coord.X, coord.Y);
        }
        public static double CalculateDistance(this clsCoordination from, clsCoordination to)
        {
            double distance = Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
            return distance;
        }

        /// <summary>
        /// 計算與站點的距離
        /// </summary>
        /// <param name="map_station"></param>
        /// <param name="from_loc_x"></param>
        /// <param name="from_loc_y"></param>
        /// <returns></returns>
        public static double CalculateDistance(this MapPoint map_station, MapPoint from_station)
        {
            return map_station.CalculateDistance(from_station.X, from_station.Y, from_station.TagNumber);
        }

        public static double TotalTravelDistance(this IEnumerable<MapPoint> points)
        {
            double distance = 0;
            for (int i = 0; i < points.Count() - 1; i++)
            {
                distance += points.ElementAt(i).CalculateDistance(points.ElementAt(i + 1));
            }
            return distance;
        }

        /// <summary>
        /// 將整數轉成2進位,每個bit用boolean表示0/1 (0:false, 1:ture)
        /// </summary>
        /// <param name="int_val"></param>
        /// <returns></returns>
        public static bool[] To4Booleans(this int int_val)
        {
            bool[] bool_ary = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                bool_ary[i] = ((int_val >> i) & 1) != 1;
            }
            return bool_ary;
        }

        public static int ToInt(this bool[] bool_ary)
        {
            return BitConverter.ToInt16(bool_ary.Select(b => b ? (byte)0x1 : (byte)0x00).Reverse().ToArray(), 0);

        }

        /// <summary>
        /// 將GPM任務訂單轉成 KGS 任務API需要的查詢參數封裝
        /// </summary>
        /// <param name="gpmDto"></param>
        /// <returns></returns>
        public static KGSWebAGVSystemAPI.TaskOrder.MissionRequestParams ToKGSMissionRequestParam(this clsTaskDto gpmDto)
        {
            KGSWebAGVSystemAPI.TaskOrder.MissionRequestParams param = new KGSWebAGVSystemAPI.TaskOrder.MissionRequestParams();
            param.CarName = gpmDto.DesignatedAGVName;
            if (param.CarName != null)
            {
                try
                {
                    param.AGVID = int.Parse(param.CarName.Split('_')[1]);
                }
                catch (Exception)
                {
                }
            }

            string fromStationName = mapUse.Points.Values.FirstOrDefault(pt => pt.TagNumber == gpmDto.From_Station_Tag)?.Name;
            string toStationName = mapUse.Points.Values.FirstOrDefault(pt => pt.TagNumber == gpmDto.To_Station_Tag)?.Name;

            param.Action = gpmDto.Action.ToString();
            param.Priority = gpmDto.Priority;
            param.FromStation = fromStationName;
            param.ToStation = toStationName;
            param.CSTID = gpmDto.Carrier_ID;
            param.CSTType = gpmDto.CST_TYPE;
            param.RepeatTime = 1;
            return param;
        }

        public static clsTaskDto ToGPMTaskDto(this KGSWebAGVSystemAPI.Models.ExecutingTask kgTask)
        {
            KGSWebAGVSystemAPI.Models.Task _Task = JsonConvert.DeserializeObject<KGSWebAGVSystemAPI.Models.Task>(JsonConvert.SerializeObject(kgTask));
            var dto = _Task.ToGPMTaskDto();
            return dto;
        }

        public static clsTaskDto ToGPMTaskDto(this KGSWebAGVSystemAPI.Models.Task kgTask)
        {
            try
            {

                var dto = new clsTaskDto
                {
                    Action = kgTask.ActionType == "Transfer" ? ACTION_TYPE.Carry : Enum.GetValues<ACTION_TYPE>().Cast<ACTION_TYPE>().FirstOrDefault(enuu => enuu.ToString() == kgTask.ActionType),
                    TaskName = kgTask.Name,
                    DesignatedAGVName = "AGV_" + kgTask.ExeVehicleID.ToString("X3"),
                    DispatcherName = kgTask.AssignUserName,
                    FailureReason = kgTask.FailReason,
                    Carrier_ID = kgTask.CSTID,
                    CST_TYPE = kgTask.CSTType == null ? 0 : (int)kgTask.CSTType,
                    From_Slot = kgTask.FromStationPortNo + "",
                    To_Slot = kgTask.ToStationPortNo + "",
                    FinishTime = kgTask.EndTime == null ? DateTime.MinValue : (DateTime)kgTask.EndTime,
                    RecieveTime = kgTask.Receive_Time,
                    StartTime = kgTask.StartTime == null ? DateTime.MinValue : (DateTime)kgTask.StartTime,
                    State = GetTaskStatus(kgTask.Status),
                    Priority = kgTask.Priority,
                };

                if (kgTask.FromStationId != null && mapUse.Points.TryGetValue((int)kgTask.FromStationId, out MapPoint fromPt))
                {
                    dto.From_Station = fromPt.TagNumber + "";
                }
                if (kgTask.ToStationId != null && mapUse.Points.TryGetValue((int)kgTask.ToStationId, out MapPoint toPt))
                {
                    dto.To_Station = toPt.TagNumber + "";
                }
                return dto;
                TASK_RUN_STATUS GetTaskStatus(int status)
                {
                    switch (status)
                    {
                        case 98:
                            return TASK_RUN_STATUS.WAIT;
                        case 1:
                            return TASK_RUN_STATUS.NAVIGATING;
                        case 2:
                            return TASK_RUN_STATUS.NAVIGATING;
                        case 3:
                            return TASK_RUN_STATUS.NAVIGATING;
                        case 4:
                            return TASK_RUN_STATUS.NAVIGATING;
                        case 90:
                            return TASK_RUN_STATUS.NAVIGATING;
                        case 100:
                            return TASK_RUN_STATUS.ACTION_FINISH;
                        case 101:
                            return TASK_RUN_STATUS.CANCEL;
                        default:
                            return TASK_RUN_STATUS.WAIT;
                    }
                }
            }
            catch (Exception ex)
            {
                return new clsTaskDto()
                {
                    TaskName = null
                };
            }
        }
        public static List<clsTaskDto> ToGPMTaskCollection(this IEnumerable<KGSWebAGVSystemAPI.Models.Task> tasks)
        {
            List<clsTaskDto> task_ = new List<clsTaskDto>();
            task_ = tasks.Select(kgTask => kgTask.ToGPMTaskDto()).ToList();
            return task_.Where(tk => tk.TaskName != null).ToList();
        }
        public static List<clsTaskDto> ToGPMTaskCollection(this IEnumerable<KGSWebAGVSystemAPI.Models.ExecutingTask> tasks)
        {
            List<clsTaskDto> task_ = new List<clsTaskDto>();
            task_ = tasks.Select(kgTask => kgTask.ToGPMTaskDto()).ToList();
            return task_.Where(tk => tk.TaskName != null).ToList();
        }
    }
}
