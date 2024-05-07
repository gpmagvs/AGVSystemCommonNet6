using AGVSystemCommonNet6.AGVDispatch.Messages;
using AGVSystemCommonNet6.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static AGVSystemCommonNet6.MAP.MapPoint;
using static System.Collections.Specialized.BitVector32;

namespace AGVSystemCommonNet6.MAP
{
    public class PathFinder
    {
        public static Map defaultMap = new Map();
        public static event EventHandler<Exception> OnExceptionHappen;
        public class PathFinderOption
        {
            public enum STRATEGY
            {
                SHORST_DISTANCE,
                MINIMAL_ROTATION_ANGLE
            }
            public bool OnlyNormalPoint { get; set; } = false;

            public bool ContainElevatorPoint { get; set; } = false;
            /// <summary>
            /// 要避開的TAG點位
            /// </summary>
            public List<int> ConstrainTags { get; set; } = new List<int>();

            public STRATEGY Strategy { get; set; } = STRATEGY.MINIMAL_ROTATION_ANGLE;

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

            public double total_rotation_angle
            {
                get
                {
                    return CalculateTotalRotation(stations);
                }
            }

private double CalculateTotalRotation(List<MapPoint> Stations)
{
    double totalRotation = 0;

    // 如果站点数量少于2，无法计算角度
    if (Stations.Count < 2)
        return totalRotation;

    // 初始朝向从第一个点到第二个点的方位角
    double previousAngle = CalculateAngle(Stations[0], Stations[1]);

    for (int i = 1; i < Stations.Count - 1; i++)
    {
        MapPoint currentPoint = Stations[i];
        MapPoint nextPoint = Stations[i + 1];

        double currentAngle = CalculateAngle(currentPoint, nextPoint);
        double angleDifference = CalculateRelativeAngle(previousAngle, currentAngle);

        // 累积相对旋转角度
        totalRotation += Math.Abs(angleDifference);

        // 更新前一个角度为当前角度
        previousAngle = currentAngle;
    }

    return totalRotation;
}

// 计算从一个方位角度到另一个的相对旋转角度
private double CalculateRelativeAngle(double fromAngle, double toAngle)
{
    double angleDifference = toAngle - fromAngle;

    // 将角度转换到 [-180, 180] 范围
    angleDifference = (angleDifference + 180) % 360 - 180;

    return angleDifference;
}

private double CalculateAngle(MapPoint point1, MapPoint point2)
{
    double deltaX = point2.X - point1.X;
    double deltaY = point2.Y - point1.Y;
    double angle = Math.Atan2(deltaY, deltaX) * (180 / Math.PI);

    // 将角度转换到 [0, 360] 范围
    angle = (angle + 360) % 360;

    return angle;
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
        public clsPathInfo FindShortestPath(int startTag, int goalTag, PathFinderOption options = null)
        {
            return FindShortestPathByTagNumber(defaultMap, startTag, goalTag, options);
        }
        public List<clsPathInfo> FindPathes(Map map, MapPoint startPt, MapPoint endPt, PathFinderOption options = null)
        {
            int startIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == startPt.TagNumber).Key;
            int endIndex = map.Points.FirstOrDefault(kp => kp.Value.TagNumber == endPt.TagNumber).Key;
            return FindPathes(map, startIndex, endIndex, options);
        }
        public List<clsPathInfo> FindPathes(Map map, int startPtIndex, int endPtIndex, PathFinderOption options = null)
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
                var allPathes = FindAllPaths(graph, source, finalIndex);
                var pathWithStations = allPathes.Select(path => path.Select(index => staions_ordered[index].Value).ToList()).ToList();
                return pathWithStations.Select(path => new clsPathInfo()
                {
                    stations = path,

                }).ToList();

            }
            catch (Exception ex)
            {
                OnExceptionHappen?.Invoke(this, ex);
                return null;
                //throw ex;
            }

        }
        public clsPathInfo FindShortestPath(Map map, int startPtIndex, int endPtIndex, PathFinderOption options)
        {
            var pathes = FindPathes(map, startPtIndex, endPtIndex, options);
            if (options != null && options.Strategy == PathFinderOption.STRATEGY.SHORST_DISTANCE)
                return pathes.OrderBy(path => path.total_travel_distance).FirstOrDefault();
            else
                return pathes.OrderBy(path => path.total_rotation_angle).FirstOrDefault();
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
        // 使用 DFS 來找到所有可能路徑
        static void FindAllPathsDFS(int[,] graph, int source, int target, List<int> currentPath, List<List<int>> allPaths, bool[] visited)
        {
            visited[source] = true;
            currentPath.Add(source);

            if (source == target)
            {
                // 如果到達目標節點，記錄當前路徑
                allPaths.Add(new List<int>(currentPath));
            }
            else
            {
                // 遍歷所有相鄰節點
                for (int v = 0; v < graph.GetLength(0); v++)
                {
                    if (!visited[v] && graph[source, v] != 0 && graph[source, v] != int.MaxValue)
                    {
                        FindAllPathsDFS(graph, v, target, currentPath, allPaths, visited);
                    }
                }
            }

            // 回溯：取消當前節點的訪問狀態
            visited[source] = false;
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        static List<List<int>> FindAllPaths(int[,] graph, int source, int target)
        {
            int n = graph.GetLength(0);
            bool[] visited = new bool[n];
            List<int> currentPath = new List<int>();
            List<List<int>> allPaths = new List<List<int>>();

            FindAllPathsDFS(graph, source, target, currentPath, allPaths, visited);

            return allPaths;
        }
    }
}
