using AGVSystemCommonNet6.AGVDispatch.Messages;
using System;
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

            /// <summary>
            /// 要避開的TAG點位
            /// </summary>
            public List<int> AvoidTagNumbers { get; set; } = new List<int>();

        }

        public class clsPathInfo
        {
            public List<MapStation> stations { get; set; } = new List<MapStation>();
            public List<int> tags => stations.Select(s => s.TagNumber).ToList();
            public double total_travel_distance
            {
                get
                {
                    double totalDistance = 0; // 總走行距離

                    for (int i = 0; i < stations.Count - 1; i++)
                    {
                        // 取得當前座標和下一個座標
                        MapStation currentStation = stations[i];
                        MapStation nextStation = stations[i + 1];

                        // 計算兩點之間的距離
                        double distance = Math.Sqrt(Math.Pow(nextStation.X - currentStation.X, 2) + Math.Pow(nextStation.Y - currentStation.Y, 2));

                        // 加總距離
                        totalDistance += distance;
                    }
                    return totalDistance;
                }
            }
        }
        public clsPathInfo FindShortestPathByTagNumber(Dictionary<int, MapStation> stations, int startTag, int endTag, PathFinderOption options = null)
        {
            int startIndex = stations.FirstOrDefault(kp => kp.Value.TagNumber == startTag).Key;
            int endIndex = stations.FirstOrDefault(kp => kp.Value.TagNumber == endTag).Key;

            if (startIndex == endIndex)
                return new clsPathInfo
                {
                    stations = new List<MapStation>(),
                };

            return FindShortestPath(stations, startIndex, endIndex, options);
        }
        public clsPathInfo FindShortestPath(Dictionary<int, MapStation> stations, MapStation startStation, MapStation endStation, PathFinderOption options = null)
        {
            int startIndex = stations.FirstOrDefault(kp => kp.Value.TagNumber == startStation.TagNumber).Key;
            int endIndex = stations.FirstOrDefault(kp => kp.Value.TagNumber == endStation.TagNumber).Key;

            return FindShortestPath(stations, startIndex, endIndex, options);
        }
        public clsPathInfo FindShortestPath(Dictionary<int, MapStation> stations, int startPtIndex, int endPtIndex, PathFinderOption options = null)
        {
            try
            {

                //Tag 73->9
                List<KeyValuePair<int, MapStation>> staions_ordered = new List<KeyValuePair<int, MapStation>>();

                if (options != null)
                {
                    if (options.OnlyNormalPoint)
                    {
                        List<KeyValuePair<int, MapStation>> normal_pts = stations.ToList().FindAll(kp => kp.Value.StationType == 0);
                        staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                    }
                    else
                    {
                        List<KeyValuePair<int, MapStation>> normal_pts = stations.ToList();
                        staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                    }

                    if (options.AvoidTagNumbers.Count != 0)
                    {

                        staions_ordered = staions_ordered.FindAll(s => !options.AvoidTagNumbers.Contains(s.Value.TagNumber));
                    }

                }
                else
                {

                    List<KeyValuePair<int, MapStation>> normal_pts = stations.ToList();
                    staions_ordered = normal_pts.OrderBy(k => k.Key).ToList();
                }


                int[,] graph = CreateDistanceMatrix(staions_ordered);

                int startIndex = staions_ordered.FindIndex(v => v.Key == startPtIndex);
                int finalIndex = staions_ordered.FindIndex(v => v.Key == endPtIndex);

                if (startIndex == -1 | finalIndex == -1)
                {
                    return new clsPathInfo();
                }

                var _startStation = staions_ordered[startIndex].Value;
                var _endStation = staions_ordered[finalIndex].Value;

                var source = startIndex;
                DijkstraAlgorithm(graph, source, out int[] distance, out int[] parent);
                Console.WriteLine("From Tag\tTo Tag\tDistance from Source\tPath(Tag)");
                Console.Write($"{_startStation.TagNumber}\t{_endStation.TagNumber}\t{distance[finalIndex]}\t");

                var dist = distance[finalIndex];
                clsPathInfo pathinfo = new clsPathInfo();

                int node = finalIndex;
                while (node != source)
                {
                    if (node == -1)
                        return new clsPathInfo
                        {

                        };
                    pathinfo.stations.Insert(0, staions_ordered[node].Value);

                    node = parent[node];
                }
                pathinfo.stations.Insert(0, staions_ordered[source].Value);
                Console.WriteLine(string.Join(" -> ", pathinfo.tags));

                return pathinfo;
            }
            catch (Exception ex)
            {
                OnExceptionHappen?.Invoke(this, ex);
                throw ex;
            }
        }

        public clsMapPoint[] GetTrajectory(string MapName, List<MapStation> stations)
        {
            List<clsMapPoint> trajectoryPoints = new List<clsMapPoint>();
            for (int i = 0; i < stations.Count; i++)
            {
                try
                {
                    trajectoryPoints.Add(new clsMapPoint()
                    {
                        index = i,
                        X = stations[i].X,
                        Y = stations[i].Y,
                        Auto_Door = new clsAutoDoor
                        {
                            Key_Name = stations[i].AutoDoor?.KeyName,
                            Key_Password = stations[i].AutoDoor?.KeyPassword,
                        },
                        Control_Mode = new clsControlMode
                        {
                            Dodge = int.Parse(stations[i].DodgeMode == null ? "0" : stations[i].DodgeMode),
                            Spin = int.Parse(stations[i].SpinMode == null ? "0" : stations[i].SpinMode),
                        },
                        Laser = stations[i].LsrMode,
                        Map_Name = MapName,
                        Point_ID = stations[i].TagNumber,
                        Speed = stations[i].Speed,
                        Theta = stations[i].Direction,
                    });
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
            return trajectoryPoints.ToArray();
        }

        private int[,] CreateDistanceMatrix(List<KeyValuePair<int, MapStation>> stations)
        {
            var totalNormalStationNum = stations.Count;
            int[,] graph = new int[totalNormalStationNum, totalNormalStationNum];
            for (int row = 0; row < totalNormalStationNum; row++)
            {
                KeyValuePair<int, MapStation> start_station = stations[row];
                int start_station_index = start_station.Key;
                List<int> near_stationIndexs = start_station.Value.Target.Select(t => int.Parse(t.Key)).ToList();

                for (int col = 0; col < totalNormalStationNum; col++)
                {
                    KeyValuePair<int, MapStation> ele_station = stations[col];
                    int ele_station_index = ele_station.Key;
                    int weight = 0;
                    if (near_stationIndexs.Contains(ele_station_index))
                    {
                        weight = CalculationDistance(ele_station.Value, start_station.Value);
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

        private int CalculationDistance(MapStation value1, MapStation value2)
        {
            double distance = Math.Sqrt(Math.Pow(value1.X - value2.X, 2) + Math.Pow(value1.Y - value2.Y, 2));
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
