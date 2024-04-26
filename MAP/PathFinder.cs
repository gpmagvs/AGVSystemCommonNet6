using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace AGVSystemCommonNet6.MAP
{
    public class PathFinder
    {
        public static event EventHandler<Exception> OnExceptionHappen;
        public class PathFinderOption
        {
            public bool OnlyNormalPoint { get; set; } = false;

            public bool ContainElevatorPoint { get; set; } = false;
            /// <summary>
            /// 要避開的TAG點位
            /// </summary>
            public List<int> ConstrainTags { get; set; } = new List<int>();

        }

        public class clsPathInfo
        {
            public ConcurrentQueue<MapPoint> waitPoints = new ConcurrentQueue<MapPoint>();

            public List<MapPoint> stations { get; set; } = new List<MapPoint>();
            public List<int> tags => stations.Select(s => s.TagNumber).ToList();
            public double total_travel_distance
            {
                get
                {
                    double totalDistance = 0; // 總走行距離

                    for (int i = 0; i < stations.Count - 1; i++)
                    {
                        // 取得當前座標和下一個座標
                        MapPoint currentStation = stations[i];
                        MapPoint nextStation = stations[i + 1];

                        // 計算兩點之間的距離
                        double distance = Math.Sqrt(Math.Pow(nextStation.X - currentStation.X, 2) + Math.Pow(nextStation.Y - currentStation.Y, 2));

                        // 加總距離
                        totalDistance += distance;
                    }
                    return totalDistance;
                }
            }
        }
        public clsPathInfo FindShortestPathByTagNumber(Map map, int startTag, int endTag, PathFinderOption options = null)
        {
            try
            {
                int startIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == startTag).Key;
                int endIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == endTag).Key;

                if (startIndex == endIndex)
                    return new clsPathInfo
                    {
                        stations = new List<MapPoint>() { map.Points[startIndex] },
                    };

                var pathinfo = FindShortestPath(map, startIndex, endIndex, options);
                if (pathinfo.stations.Count == 0)
                    throw new NoPathForNavigatorException();
                return pathinfo;
            }
            catch (NoPathForNavigatorException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public clsPathInfo FindShortestPath(Map map, MapPoint startStation, MapPoint endStation, PathFinderOption options = null)
        {
            int startIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == startStation.TagNumber).Key;
            int endIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == endStation.TagNumber).Key;

            return FindShortestPath(map, startIndex, endIndex, options);
        }

        public clsPathInfo FindShortestPath(Map map, int startPtIndex, int endPtIndex, PathFinderOption options = null)
        {
            var stations = map.Points.ToList().FindAll(st => st.Value.Enable).ToDictionary(pt => pt.Key, pt => pt.Value);
            try
            {
                List<KeyValuePair<int, MapPoint>> staions_ordered = new List<KeyValuePair<int, MapPoint>>();

                if (options != null)
                {
                    if (options.OnlyNormalPoint)
                    {
                        var normal_pts = stations.ToList().Where(kp => kp.Value.StationType == 0).ToList();
                        if (options.ContainElevatorPoint)
                        {
                            normal_pts.AddRange(stations.Where(kp => kp.Value.StationType == STATION_TYPE.Elevator));
                        }
                        staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                    }
                    else
                    {
                        List<KeyValuePair<int, MapPoint>> normal_pts = stations.ToList();
                        staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                    }

                    if (options.ConstrainTags.Count != 0)
                    {
                        var constrainTags = options.ConstrainTags.Distinct();
                        staions_ordered = staions_ordered.FindAll(s => !constrainTags.Contains(s.Value.TagNumber));
                    }

                }
                else
                {

                    List<KeyValuePair<int, MapPoint>> normal_pts = stations.ToList();
                    staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                }

                var refPathes = map.Segments;
                int[,] graph = CreateDistanceMatrix(staions_ordered, ref refPathes);

                int startIndex = staions_ordered.FindIndex(v => v.Key == startPtIndex);
                int finalIndex = staions_ordered.FindIndex(v => v.Key == endPtIndex);

                if (startIndex == -1 || finalIndex == -1)
                {
                    return null;
                }

                var _startStation = staions_ordered[startIndex].Value;
                var _endStation = staions_ordered[finalIndex].Value;

                var source = startIndex;
                DijkstraAlgorithm(graph, source, out int[] distance, out int[] parent);

                var dist = distance[finalIndex];
                if (dist < 0)
                {

                }
                clsPathInfo pathinfo = new clsPathInfo();

                int node = finalIndex;
                while (node != source)
                {
                    if (node == -1)
                    {
                        throw new NoPathForNavigatorException();
                    }
                    pathinfo.stations.Insert(0, staions_ordered[node].Value);

                    node = parent[node];
                }
                pathinfo.stations.Insert(0, staions_ordered[source].Value);
                return pathinfo;
            }
            catch (Exception ex)
            {
                OnExceptionHappen?.Invoke(this, ex);
                return null;
                //throw ex;
            }

        }
        public static clsMapPoint[] GetTrajectory(string MapName, List<MapPoint> stations)
        {
            List<clsMapPoint> trajectoryPoints = new List<clsMapPoint>();
            for (int i = 0; i < stations.Count; i++)
            {
                clsMapPoint createNewPoint(MapPoint mapStation)
                {
                    try
                    {
                        var Auto_Door = new clsAutoDoor
                        {
                            Key_Name = mapStation.AutoDoor?.KeyName,
                            Key_Password = mapStation.AutoDoor?.KeyPassword,
                        };

                        var Control_Mode = new clsControlMode
                        {
                            Dodge = (int)(mapStation.DodgeMode == null ? 0 : mapStation.DodgeMode),
                            Spin = (int)(mapStation.SpinMode == null ? 0 : mapStation.SpinMode)
                        };

                        return new clsMapPoint()
                        {
                            index = i,
                            X = mapStation.X,
                            Y = mapStation.Y,
                            Auto_Door = Auto_Door,
                            Control_Mode = Control_Mode,
                            Laser = mapStation.LsrMode,
                            Map_Name = MapName,
                            Point_ID = mapStation.TagNumber,
                            Speed = mapStation.Speed,
                            Theta = mapStation.Direction,
                        };
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                trajectoryPoints.Add(createNewPoint(stations[i]));


            }
            return trajectoryPoints.ToArray();
        }
        private int[,] CreateDistanceMatrix(List<KeyValuePair<int, MapPoint>> stations, ref List<MapPath> pathes)
        {
            var totalNormalStationNum = stations.Count;
            int[,] graph = new int[totalNormalStationNum, totalNormalStationNum];
            for (int row = 0; row < totalNormalStationNum; row++)
            {
                KeyValuePair<int, MapPoint> start_station = stations[row];
                int start_station_index = start_station.Key;
                var pathfound_ = pathes.FindAll(path => path.StartPtIndex == start_station_index);
                List<int> near_stationIndexs = pathfound_.Select(path => path.EndPtIndex).ToList();

                for (int col = 0; col < totalNormalStationNum; col++)
                {
                    KeyValuePair<int, MapPoint> ele_station = stations[col];
                    int ele_station_index = ele_station.Key;
                    int weight = 0;
                    if (near_stationIndexs.Contains(ele_station_index))
                    {
                        var pathID = start_station_index + "_" + ele_station_index;
                        var pathFound = pathes.FirstOrDefault(path => path.PathID == pathID);
                        double weightOfPath = 1.0;
                        if (pathFound != null)
                        {
                            weightOfPath = pathFound.Weight <= 0 ? 1.0 : pathFound.Weight;
                        }
                        weight = CalculationDistance(ele_station.Value, start_station.Value, weightOfPath);
                    }
                    else
                    {
                        weight = 0;
                    }
                    graph[row, col] = weight;
                }
            }
            return graph;
        }
        private int CalculationDistance(MapPoint value1, MapPoint value2, double weight = 1)
        {
            double distance = Math.Sqrt(Math.Pow(value1.X - value2.X, 2) + Math.Pow(value1.Y - value2.Y, 2)) / weight;
            return int.Parse(Math.Round(distance * 10000, 0).ToString());
        }

        static void DijkstraAlgorithm(int[,] graph, int source, out int[] distance, out int[] parent)
        {
            int n = graph.GetLength(0);
            bool[] visited = new bool[n];
            distance = new int[n];
            parent = new int[n];
            for (int i = 0; i < n; i++)
            {
                visited[i] = false;
                distance[i] = int.MaxValue;
                parent[i] = -1;
            }
            distance[source] = 0;

            for (int i = 0; i < n - 1; i++)
            {
                int u = -1;
                for (int j = 0; j < n; j++)
                {
                    if (!visited[j] && (u == -1 || distance[j] < distance[u]))
                    {
                        u = j;
                    }
                }
                visited[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (!visited[v] && graph[u, v] != 0)
                    {
                        int alt = distance[u] + graph[u, v];
                        if (alt < distance[v])
                        {
                            distance[v] = alt;
                            parent[v] = u;
                        }
                    }
                }
            }
        }

    }
}
