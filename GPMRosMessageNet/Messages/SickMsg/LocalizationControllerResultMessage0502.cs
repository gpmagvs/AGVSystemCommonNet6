using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace AGVSystemCommonNet6.GPMRosMessageNet.Messages.SickMsg
{
    public class LocalizationControllerResultMessage0502 : Message
    {
        public const string RosMessageName = "sick_lidar_localization/LocalizationControllerResultMessage0502";

        //  Localization result message type 5 version 2
        public Header header { get; set; }
        public ulong telegram_count { get; set; }
        //  8 byte TelegramCount uint64
        public ulong timestamp { get; set; }
        //  8 byte Timestamp uint64 [microseconds]
        public int source_id { get; set; }
        //  4 byte source id
        public long x { get; set; }
        //  8 byte X int64 [mm]
        public long y { get; set; }
        //  8 byte Y int64 [mm]
        public int heading { get; set; }
        //  4 byte Heading int32 [mdeg]

        /// <summary>
        ///1 byte LocalizationStatus [0...100, 10: OK, 20: Warning, 30: Not localized, 40: System error]
        /// </summary>
        public byte loc_status { get; set; }
        /// <summary>
        /// 1 byte MapMatchingStatus [0...100, 90: Good, 60: Medium, 30: Low, 0; None]
        /// </summary>
        public byte map_match_status { get; set; }
        public uint sync_timestamp_sec { get; set; }
        //  seconds part of synchronized timestamp in system time calculated by Software-PLL
        public uint sync_timestamp_nsec { get; set; }
        //  nanoseconds part of synchronized timestamp in system time calculated by Software-PLL
        public uint sync_timestamp_valid { get; set; }
        //  1: sync_timestamp successfully calculated by Software-PLL, 0: Software-PLL not yet synchronized

        public LocalizationControllerResultMessage0502()
        {
            this.header = new Header();
            this.telegram_count = 0;
            this.timestamp = 0;
            this.source_id = 0;
            this.x = 0;
            this.y = 0;
            this.heading = 0;
            this.loc_status = 40;
            this.map_match_status = 0;
            this.sync_timestamp_sec = 0;
            this.sync_timestamp_nsec = 0;
            this.sync_timestamp_valid = 0;
        }

        public LocalizationControllerResultMessage0502(Header header, ulong telegram_count, ulong timestamp, int source_id, long x, long y, int heading, byte loc_status, byte map_match_status, uint sync_timestamp_sec, uint sync_timestamp_nsec, uint sync_timestamp_valid)
        {
            this.header = header;
            this.telegram_count = telegram_count;
            this.timestamp = timestamp;
            this.source_id = source_id;
            this.x = x;
            this.y = y;
            this.heading = heading;
            this.loc_status = loc_status;
            this.map_match_status = map_match_status;
            this.sync_timestamp_sec = sync_timestamp_sec;
            this.sync_timestamp_nsec = sync_timestamp_nsec;
            this.sync_timestamp_valid = sync_timestamp_valid;
        }
    }
}
