using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dijkestra_Tiled_Graph_Visualizer
{
        /*
        class Program
        {
            static void Main(string[] args)
            {

                //float I = float.PositiveInfinity;
                //float[,] weights = new float[8, 8]
                //{{I,I,I,I,I,5,7,3},
                //{I,I,2,5,4,I,I,I},
                //{I,2,I,2,I,7,I,12},
                //{I,5,2,I,1,4,3,I},
                //{I,4,I,1,I,I,4,I},
                //{5,I,7,4,I,I,1,I},
                //{7,I,I,3,4,1,I,I},
                //{3,I,12,I,I,I,I,I}};

                float I = float.PositiveInfinity;
                float[,] weights = new float[9, 9]
            {   
                {I,4,I,1,I,I,I,I,I},
                {4,I,2,I,5,I,I,I,I},
                {I,2,I,I,I,8,I,I,I},
                {1,I,I,I,1,I,3,I,I},
                {I,5,I,1,I,1,I,2,I},
                {I,I,8,I,1,I,I,I,6},
                {I,I,I,3,I,I,I,2,I},
                {I,I,I,I,2,I,2,I,1},
                {I,I,I,I,I,6,I,1,I}
            };



                AStarSearch aStar = new AStarSearch(weights, 9);

                aStar.Search(6, 2);

                Edge[] edges = aStar.getEdges();

                foreach (Edge e in edges)
                {
                    if (e != null)
                        Console.WriteLine("{0} -> {1} ", e.ToString(), e.Cost);
                }
                Queue<int> PathSeq = aStar.getPathSequence();
                foreach (int a in PathSeq)
                {
                    Console.WriteLine(a);
                }

                Console.ReadKey(true);
            }

        }
        */
        class AStarSearch
        {
            Edge[] ShortestPathTree;
            Edge[] SearchFrontier;

            float[] GCost;
            float[] FCost;

            int Source;
            int Destination;

            //int SourceIndex;

            int GraphNodesNum;

            int EdgeOfSquare;

            TiledGraphNode[] Nodes;

            //float[,] GraphWeights;
            TiledGraphGenerator tiledGraphSystem;

            public AStarSearch(TiledGraphGenerator tiledGraphSystem)
            {
                Source = Destination = 0;
                this.tiledGraphSystem = tiledGraphSystem;

                GraphNodesNum = tiledGraphSystem.TotalNumNodes;

                EdgeOfSquare = (int)Math.Sqrt(GraphNodesNum);

                GCost = new float[GraphNodesNum];
                FCost = new float[GraphNodesNum];

                int combine2_n = (GraphNodesNum * (GraphNodesNum - 1)) / 2;
                ShortestPathTree = new Edge[combine2_n];//new List<Edge>(32);
                SearchFrontier = new Edge[combine2_n];//new List<Edge>(32);

                Nodes = tiledGraphSystem.getNodes();
            }
            public void Search(int src, int dst)
            {
                Source = src;
                Destination = dst;

                ShortestPathTree[src] = new Edge(-1, src, 0);

                //   SortedDictionary<float, int> PQ = new SortedDictionary<float, int>(); // <F(x),nodeIndex>
                PriorityQueue PQ = new PriorityQueue(GraphNodesNum);

                PQ.Add(src, 0);

                while (!PQ.isEmpty())
                {
                    Console.WriteLine("Not empty yet...");
                    int nextClosestNode = PQ.Remove().index;
                    ShortestPathTree[nextClosestNode] = SearchFrontier[nextClosestNode];

                    if (nextClosestNode == dst) return;

                    foreach(Edge e in Nodes[nextClosestNode].AdjacentEdges)
                    {
                        if (e == null || !Nodes[e.Destination].isValid) continue;

                        //if (!float.IsInfinity(e.Cost))
                        //e = new Edge(nextClosestNode, i, GraphWeights[nextClosestNode, i]);

                     

                        float f_HCost = Heuristic.ManhattanDistance(nextClosestNode, dst, EdgeOfSquare);
                        float f_GCost = GCost[nextClosestNode] + e.Cost;

                        if (SearchFrontier[e.Destination] == null)
                        {
                            FCost[e.Destination] = f_GCost + f_HCost;
                            GCost[e.Destination] = f_GCost;
                            PQ.Add(e.Destination, FCost[e.Destination]);
                            SearchFrontier[e.Destination] = e;
                        }
                        else if (f_GCost < GCost[e.Destination] && ShortestPathTree[e.Destination] == null)
                        {
                            float tmp = FCost[e.Destination];

                            FCost[e.Destination] = f_GCost + f_HCost;
                            GCost[e.Destination] = f_GCost;

                            PQ.ChangePriority(e.Destination, FCost[e.Destination]);
                            SearchFrontier[e.Destination] = e;

                        }
                    }

                }

            }
            public Edge[] getEdges()
            {
                return ShortestPathTree;
            }
            public Queue<int> getPathSequence()
            {
                Queue<int> Path = new Queue<int>();
                //    Path.Enqueue(Destination);

                int iterator = Destination;
                while (iterator != Source)
                {
                    Path.Enqueue(iterator);
                    iterator = ShortestPathTree[iterator].Source;
                }
                Path.Enqueue(iterator);

                return Path;
            }

        }
        static class Heuristic
        {
            public static float ManhattanDistance(int nodeIndex1, int nodeIndex2, int EdgeOfSquareGraph)
            {
                int x1 = nodeIndex1 % EdgeOfSquareGraph;
                int x2 = nodeIndex2 % EdgeOfSquareGraph;

                int y1 = nodeIndex1 / EdgeOfSquareGraph;
                int y2 = nodeIndex2 / EdgeOfSquareGraph;

                return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            }
        }
        class Edge
        {
            public int Source;
            public int Destination;

            public float Cost;

            public Edge(int s, int d, float c)
            {
                Source = s;
                Destination = d;
                Cost = c;
            }
            public override string ToString()
            {
                return string.Format("<{0},{1}>", Source, Destination);
            }
        }

        class PriorityQueue
        {
            int queueSize;
            public Pair<int, float>[] QueueData;
            public void ListMembers()
            {
                for (int i = 0; i < queueSize; i++)
                    Console.WriteLine(QueueData[i].ToString());
            }
            public PriorityQueue(int maxSize)
            {
                QueueData = new Pair<int, float>[maxSize];
                for (int i = 0; i < maxSize; i++)
                    QueueData[i] = new Pair<int, float>(0, 0);

                queueSize = maxSize;
            }
            public static bool isRaw(Pair<int, float> kvp)
            {
                if (kvp.index == 0 && kvp.value == 0)
                    return true;
                return false;
            }
            public bool Add(int key, float value)
            {
                for (int i = 0; i < queueSize; i++)
                {
                    if (isRaw(QueueData[i]))
                    {
                        QueueData[i] = new Pair<int, float>(key, value);
                        return true;
                    }
                }
                return false;
            }
            public Pair<int, float> Remove()
            {
                Pair<int, float> tmp = new Pair<int, float>(int.MaxValue, float.PositiveInfinity);
                int minIndex = 0;
                for (int i = 0; i < queueSize; i++)
                {
                    if (tmp.value >= QueueData[i].value && !isRaw(QueueData[i]))
                    {
                        tmp = QueueData[i];
                        minIndex = i;
                        //    Console.WriteLine("{0} -> {1}",tmp.ToString(),minIndex);
                    }
                }
                QueueData[minIndex].index = 0;
                QueueData[minIndex].value = 0; // new Pair<int, float>(0, 0);

                return tmp;
            }
            public bool ChangePriority(int index, float newValue)
            {
                for (int i = 0; i < queueSize; i++)
                {
                    if (QueueData[i].index == index)
                    {
                        QueueData[i] = new Pair<int, float>(index, newValue);
                        return true;
                    }
                }
                return false;
            }
            public bool isEmpty()
            {
                for (int i = 0; i < queueSize; i++)
                    if (!isRaw(QueueData[i]))
                        return false;
                return true;
            }


        }
        struct Pair<Tindex, Tvalue>
        {
            public Tindex index;
            public Tvalue value;

            public Pair(Tindex i, Tvalue val)
            {
                index = i;
                value = val;
            }

            public override string ToString()
            {
                return string.Format("[{0},{1}]", index, value);
            }
        }
    }
