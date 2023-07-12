using AGVSystemCommonNet6.AGVDispatch.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.AGVDispatch.Model
{
    public class clsFeedbackData
    {
        private FeedbackData feedbackData;
        public clsFeedbackData() { }
        public clsFeedbackData(FeedbackData feedbackData)
        {
            TimeStamp = feedbackData.TimeStamp;
            TaskName = feedbackData.TaskName;
            TaskSimplex=feedbackData.TaskSimplex;
            TaskSequence=feedbackData.TaskSequence;
            PointIndex = feedbackData.PointIndex;
            TaskStatus = feedbackData.TaskStatus;
        }

        public virtual string TimeStamp { get; set; }

        public virtual string TaskName { get; set; }
        public virtual string TaskSimplex { get; set; }

        public virtual int TaskSequence { get; set; }

        public virtual int PointIndex { get; set; }

        public virtual TASK_RUN_STATUS TaskStatus { get; set; }

        public virtual int Last_Visited_Node { get; set; } = 0;

        public virtual clsCoordination Coordination { get; set; } = new clsCoordination();
    }
}
