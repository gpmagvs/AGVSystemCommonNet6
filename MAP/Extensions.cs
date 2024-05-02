using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.AGVDispatch.Model;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public static class Extensions
    {
        public static IEnumerable<int> GetTagCollection(this IEnumerable<MapPoint> mapPointCollection)
        {
            return mapPointCollection.Select(pt => pt.TagNumber);
        }
        public static IEnumerable<int> GetTagList(this IEnumerable<clsMapPoint> mapPointCollection)
        {
            return mapPointCollection.Select(pt => pt.Point_ID);
        }

        public static IEnumerable<MapPoint> SkipByTagNumber(this IEnumerable<MapPoint> mapPointCollection, int tag)
        {
            var index = mapPointCollection.ToList().FindIndex(pt => pt.TagNumber == tag);
            return mapPointCollection.Skip(index);
        }
        public static MapRegion GetRegion(this double[] coordination, Map map)
        {
            //取出所有區域集合
            var checkResults = map.Regions.ToDictionary(region => region, region => isInRegion(region.PolygonCoordinations, coordination));
            var pari = checkResults.FirstOrDefault(region => region.Value);
            if (pari.Key == null)
                return null;
            return pari.Key;
            bool isInRegion(List<double[]> polygon, double[] testPoint)
            {
                bool result = false;
                double X = testPoint[0];
                double Y = testPoint[1];

                int j = polygon.Count - 1;
                for (int i = 0; i < polygon.Count; i++)
                {
                    if (polygon[i][1] < Y && polygon[j][1] >= Y || polygon[j][1] < Y && polygon[i][1] >= Y)
                    {
                        if (polygon[i][0] + (Y - polygon[i][1]) / (polygon[j][1] - polygon[i][1]) * (polygon[j][0] - polygon[i][0]) < X)
                        {
                            result = !result;
                        }
                    }
                    j = i;
                }
                return result;
            }
        }
        public static MapRegion GetRegion(this MapPoint mapPoint, Map map)
        {
            return new double[2] { mapPoint.X, mapPoint.Y }.GetRegion(map);
        }
        public static MapRegion GetRegion(this clsCoordination coordinaiton, Map map)
        {
            return new double[2] { coordinaiton.X, coordinaiton.Y }.GetRegion(map);
        }

        public static IEnumerable<MapRegion> GetRegions(this IEnumerable<MapPoint> paths, Map map)
        {
            return paths.Select(point => point.GetRegion(map));
        }

        public static MapPoint GetNearEntryPoint(this MapRegion region, Map refMap, MapPoint currentCoordination)
        {

            if (region == null || !region.EnteryTags.Any())
                return null;

            var mapPoints = region.EnteryTags.Select(tag => refMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag));
            return mapPoints.OrderBy(pt => pt.CalculateDistance(currentCoordination)).FirstOrDefault();
        }
        public static MapPoint GetNearLeavingPoint(this MapRegion region, Map refMap, MapPoint currentCoordination)
        {

            if (region == null || !region.LeavingTags.Any())
                return null;

            var mapPoints = region.LeavingTags.Select(tag => refMap.Points.Values.FirstOrDefault(pt => pt.TagNumber == tag));
            return mapPoints.OrderBy(pt => pt.CalculateDistance(currentCoordination)).FirstOrDefault();
        }

        public static int GetCurrentVehicleNum(this MapRegion region, Map refMap, IEnumerable<MapPoint> vehiclePoints)
        {
            var allRegionMatch = vehiclePoints.Where(pt => pt.GetRegion(refMap)?.Name == region.Name);
            if (!allRegionMatch.Any())
                return 0;
            return allRegionMatch.Count();
        }
    }
}
