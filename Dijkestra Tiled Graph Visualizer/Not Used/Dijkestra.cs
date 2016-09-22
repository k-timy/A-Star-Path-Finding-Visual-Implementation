using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dijkestra_Tiled_Graph_Visualizer
{
    class Dijkestra
    {
        static void showRoute(int start, int dest, int[] route)
        {            
            //Console.WriteLine("The Route:");
            int index = dest;

            while (true)
            {
                //Console.Write(index + " ");
                index = route[index];

                if (index == start)
                {
               //     Console.Write((graphVertex)index + " ");
                    break;
                }

            }
        }
        public static int[] dijkestra(int start, int n, float[,] weights) //LinkedList<Edge>
        {
            int i, vnear = start;
            // Edge e;
            int[] touch = new int[n];
            float[] length = new float[n];

            //  LinkedList<Edge> edgesOfGraph = new LinkedList<Edge>();          

            for (i = 0; i <= n - 1; i++)
            {
                touch[i] = start;
                length[i] = weights[start, i];
            }

            for (int j = 0; j < n; j++)
            {
                float min = float.PositiveInfinity;
                for (i = 0; i <= n - 1; i++)
                    if (length[i] >= 0 && length[i] < min)
                    {
                        min = length[i];
                        vnear = i;
                    }
                //  e = new Edge(touch[vnear], vnear);

                //  edgesOfGraph.AddLast(e);


                for (i = 0; i <= n - 1; i++)
                {
                    if (length[vnear] + weights[vnear, i] < length[i])
                    {
                        length[i] = length[vnear] + weights[vnear, i];
                        touch[i] = vnear;
                    }
                }



                length[vnear] = -1;
            }
            Console.WriteLine("this: {0}", length[1]);

            //return edgesOfGraph;
            touch[start] = start;
            return touch;
        }
    }
}
