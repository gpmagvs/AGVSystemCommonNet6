using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class DijkstraSearch
    {
        private static readonly int Infinity = int.MaxValue;

        public static List<List<int>> FindKShortestPaths(int[,] graph, int source, int target, int k)
        {
            int n = graph.GetLength(0);
            List<List<int>> kShortestPaths = new List<List<int>>();
            PriorityQueue<Path> pq = new PriorityQueue<Path>();
            int[] distances = new int[n];
            List<int>[] previousPaths = new List<int>[n];

            // 初始化距離和前驅路徑
            for (int i = 0; i < n; i++)
            {
                distances[i] = Infinity;
                previousPaths[i] = new List<int>();
            }
            distances[source] = 0;
            previousPaths[source].Add(source);

            // 將起始節點放入優先級隊列
            pq.Enqueue(new Path(source, 0, previousPaths[source]));

            // Dijkstra 算法主循環
            while (pq.Count > 0 && kShortestPaths.Count < k)
            {
                Path current = pq.Dequeue();

                int u = current.Node;
                int dist = current.Distance;
                List<int> path = current.PathToHere;

                // 如果當前路徑已經比之前找到的更長，則跳過
                if (dist > distances[u])
                    continue;

                // 如果到達目標節點，添加到最佳路徑列表中
                if (u == target)
                {
                    kShortestPaths.Add(path);
                    continue; // 繼續尋找下一條最短路徑
                }

                // 遍歷相鄰節點
                for (int v = 0; v < n; v++)
                {
                    if (graph[u, v] > 0) // 假設 graph[u, v] > 0 表示有連接
                    {
                        int newDist = dist + graph[u, v];

                        // 如果找到更短的路徑
                        if (newDist < distances[v])
                        {
                            distances[v] = newDist;

                            // 更新前驅路徑
                            List<int> newPath = new List<int>(path);
                            newPath.Add(v);
                            previousPaths[v] = newPath;

                            // 將新路徑添加到優先級隊列
                            pq.Enqueue(new Path(v, newDist, newPath));

                            // 如果優先級隊列超過 k，則移除最長的路徑
                            if (pq.Count > k)
                            {
                                pq.Dequeue();
                            }
                        }
                        else if (newDist == distances[v])
                        {
                            // 如果找到與當前最短路徑長度相同的路徑，則將其添加到前驅路徑列表中
                            List<int> newPath = new List<int>(path);
                            newPath.Add(v);
                            previousPaths[v].AddRange(newPath);
                        }
                    }
                }
            }

            return kShortestPaths;
        }

        public class Path : IComparable<Path>
        {
            public int Node { get; set; }
            public int Distance { get; set; }
            public List<int> PathToHere { get; set; }

            public Path(int node, int distance, List<int> pathToHere)
            {
                Node = node;
                Distance = distance;
                PathToHere = pathToHere;
            }

            public int CompareTo(Path other)
            {
                // 用於最小堆的比較，按照距離升序排列
                return this.Distance.CompareTo(other.Distance);
            }
        }

        // 最小堆實現
        public class PriorityQueue<T> where T : IComparable<T>
        {
            private List<T> heap = new List<T>();

            public int Count { get { return heap.Count; } }

            public void Enqueue(T item)
            {
                heap.Add(item);
                int i = heap.Count - 1;
                while (i > 0)
                {
                    int parent = (i - 1) / 2;
                    if (heap[parent].CompareTo(heap[i]) <= 0)
                        break;
                    T tmp = heap[i];
                    heap[i] = heap[parent];
                    heap[parent] = tmp;
                    i = parent;
                }
            }

            public T Dequeue()
            {
                T root = heap[0];
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);

                int i = 0;
                while (true)
                {
                    int left = 2 * i + 1;
                    int right = 2 * i + 2;
                    int smallest = i;

                    if (left < heap.Count && heap[left].CompareTo(heap[smallest]) < 0)
                        smallest = left;

                    if (right < heap.Count && heap[right].CompareTo(heap[smallest]) < 0)
                        smallest = right;

                    if (smallest == i)
                        break;

                    T tmp = heap[i];
                    heap[i] = heap[smallest];
                    heap[smallest] = tmp;
                    i = smallest;
                }

                return root;
            }

            public T Peek()
            {
                return heap[0];
            }
        }
    }
}
