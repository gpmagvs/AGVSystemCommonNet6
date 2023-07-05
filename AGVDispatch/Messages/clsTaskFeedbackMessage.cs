using AGVSystemCommonNet6.AGVDispatch.Model;
using Newtonsoft.Json;
using static AGVSystemCommonNet6.AGVDispatch.Messages.RunningStatus;

namespace AGVSystemCommonNet6.AGVDispatch.Messages
{
    public class clsTaskFeedbackMessage : MessageBase
    {
        public Dictionary<string, FeedbackData> Header { get; set; } = new Dictionary<string, FeedbackData>();
    }

    public class FeedbackData: clsFeedbackData
    {
        [JsonProperty("Time Stamp")]
        public override string TimeStamp { get; set; }

        [JsonProperty("Task Name")]
        public override string TaskName { get; set; }
        [JsonProperty("Task Simplex")]
        public override string TaskSimplex { get; set; }

        [JsonProperty("Task Sequence")]
        public override int TaskSequence { get; set; }


        [JsonProperty("Point Index")]
        public override int PointIndex { get; set; }

        [JsonProperty("Task Status")]
        public override TASK_RUN_STATUS TaskStatus { get; set; }

        [JsonProperty("Last Visited Node")]
        public override int Last_Visited_Node { get; set; } = 0;


        [JsonProperty("Coordination")]
        public override clsCoordination Coordination { get; set; } = new clsCoordination();
    }
}
