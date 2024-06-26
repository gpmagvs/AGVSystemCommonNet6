using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class DijkstraSearch
    {
        private int[,] graph;
        private int verticesCount;

        public DijkstraSearch(int[,] adjacencyMatrix)
        {
            graph = adjacencyMatrix;
            verticesCount = adjacencyMatrix.GetLength(0);
        }
        public List<int> FindShortestPath(int source, int target)
        {
            // Initialize distance array and visited array
            int[] distances = new int[verticesCount];
            bool[] visited = new bool[verticesCount];
            int[] previous = new int[verticesCount];

            // Initialize distances with infinity and source with 0
            for (int i = 0; i < verticesCount; i++)
            {
                distances[i] = int.MaxValue;
                visited[i] = false;
                previous[i] = -1;
            }
            distances[source] = 0;

            // Dijkstra's algorithm
            for (int count = 0; count < verticesCount - 1; count++)
            {
                // Find the vertex with the minimum distance value
                int u = MinimumDistance(distances, visited);

                // Mark the picked vertex as visited
                visited[u] = true;

                // Update the distance values of adjacent vertices
                for (int v = 0; v < verticesCount; v++)
                {
                    if (!visited[v] && graph[u, v] != 0 && distances[u] != int.MaxValue && distances[u] + graph[u, v] < distances[v])
                    {
                        distances[v] = distances[u] + graph[u, v];
                        previous[v] = u;
                    }
                }
            }

            // Build the path from source to target using previous array
            List<int> path = new List<int>();
            int current = target;
            while (current != -1)
            {
                path.Add(current);
                current = previous[current];
            }
            path.Reverse(); // Reverse to get path from source to target

            return path;
        }

        private int MinimumDistance(int[] distances, bool[] visited)
        {
            int min = int.MaxValue;
            int minIndex = -1;

            for (int v = 0; v < verticesCount; v++)
            {
                if (visited[v] == false && distances[v] <= min)
                {
                    min = distances[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }
    }
}
