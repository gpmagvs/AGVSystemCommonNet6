using AGVSystemCommonNet6.MAP.GeoJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public static class MapExtensions
    {
        public static GeoMapData GetGeoMapData(this Map map)
        {
            GeoMapData geoMapData = new GeoMapData();
            geoMapData.PointsGeoJsonData.features = map.Points.Select(pt => new GeoFeature
            {
                geometry = new PointGeomety
                {
                     coordinates = new double[2] { pt.Value.Graph.X, pt.Value.Graph.Y }
                },
                properties = new FeatureProperties
                {
                    name = pt.Value.Name,
                    point_index = pt.Key,
                    station_type = pt.Value.StationType,
                }
            }).ToList();
            geoMapData.LinesGeoJsonData.features = map.Points.Select(pt => new GeoFeature()
            {
                properties = new FeatureProperties
                {

                },
                geometry = new LineStringGeomety()
                {
                    coordinates = GetLinePointsOfStation(ref map, pt.Value)
                }
            }).ToList();

            return geoMapData;
        }
        private static List<double[]> GetLinePointsOfStation(ref Map map, MapPoint pt)
        {
            if (pt.Target.Count == 0)
                return new List<double[]>();
            var linePoints = new List<double[]>() { new double[2] { pt.Graph.X, pt.Graph.Y } };
            foreach (var point in pt.Target)
            {
                MapPoint pt_link = map.Points[point.Key];
                linePoints.Add(new double[2] { pt_link.Graph.X, pt_link.Graph.Y });
            }
            return linePoints;
        }

    }
}
