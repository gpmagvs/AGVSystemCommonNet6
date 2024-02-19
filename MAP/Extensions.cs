using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public static class Extensions
    {
        public static IEnumerable<int> GetTagList(this IEnumerable<MapPoint> mapPointCollection)
        {
            return mapPointCollection.Select(pt => pt.TagNumber);
        }
        public static IEnumerable<int> GetTagList(this IEnumerable<clsMapPoint> mapPointCollection)
        {
            return mapPointCollection.Select(pt => pt.Point_ID);
        }
    }
}
