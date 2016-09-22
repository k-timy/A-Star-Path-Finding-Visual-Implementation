using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dijkestra_Tiled_Graph_Visualizer
{
    public partial class Form1 : Form
    {
        static float ZERO = 1E-08f;
        static float INF = float.PositiveInfinity;
        static float edgeOfSquare = 20;
        public static float Radical2 = (float)Math.Sqrt(2);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnDrawPath_Click(object sender, EventArgs e)
        {
            
            Graphics g = this.CreateGraphics();
            Pen myPen = new Pen(Brushes.Cyan, 3);

            //int primaryIndex = TiledGraph.getIndexOfPoint(primaryPoint);

            //routePoints = Dijkestra.dijkestra(primaryIndex, PointsArray.Length, AdjacencyMatrix);


            int destIndex = TiledGraph.getIndexOfPoint(secondaryPoint); //,PointsArray); //new PointF(720, 500));
            int startIndex = TiledGraph.getIndexOfPoint(primaryPoint); //, PointsArray); //new PointF(59,20));//
    
            Pen penTmp = new Pen(Brushes.Red,4);
            DrawCrossHair(g, TiledGraph.getNodes()[destIndex].Position, 10, penTmp);

            penTmp.Color = Color.Yellow;
            DrawCrossHair(g, TiledGraph.getNodes()[startIndex].Position, 10, penTmp);

            // ta inja raDfe...

            // AStarSearch must be changed in a way that works with s.th. other than graph's weight matrix... XNA
            AStarSearch ASearch = new AStarSearch(TiledGraph);

            DateTime dt = System.DateTime.Now;
            ASearch.Search(startIndex, destIndex);

            int [] path = ASearch.getPathSequence().ToArray();

            MessageBox.Show(string.Format("{0}", DateTime.Now.Subtract(dt).TotalMilliseconds));

            TiledGraphNode [] nodes = TiledGraph.getNodes();

            for (int i = 1; i < path.Length; i++)
            {
                g.DrawLine(myPen,nodes[path[i - 1]].Position,nodes[path[i]].Position);
            }
            /*
            int routePointsCount = 0;

            int index = destIndex;
            while (true)
            {
                g.DrawLine(myPen, PointsArray[index], PointsArray[routePoints[index]]);
                index = routePoints[index];

                

                if (index == startIndex)
                {
                    g.DrawLine(myPen, PointsArray[index], PointsArray[routePoints[index]]);
                    break;
                }
                routePointsCount++;
            }
            MessageBox.Show(routePointsCount.ToString());
            */
        }
        #region Graph Provide

        private PointF primaryPoint;
        private PointF secondaryPoint;
        private int[] routePoints;

        private static int primary_secondary = 0;

        private string imageFilePath;
      //  private float Edge_of_Square = edgeOfSquare;

        TiledGraphGenerator TiledGraph;

        private void btnGenGraph_Click(object sender, EventArgs e)
        {
            Image image = Image.FromFile(imageFilePath);
            
            TiledGraph = new TiledGraphGenerator(new PointF(20, 20), new PointF(800 - 20, 610),edgeOfSquare,image);

            TiledGraphNode[] nodes = TiledGraph.getNodes();

            Graphics g = this.CreateGraphics();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].isValid)
                {
                    DrawCrossHair(g, nodes[i].Position, 3, Pens.Black);
                    nodes[i].setNeighbours(nodes, TiledGraph.GraphHeightNodeCount, TiledGraph.GraphWidthNodeCount,edgeOfSquare);
                  //  g.DrawString(nodes[i].Index.ToString(), System.Drawing.SystemFonts.DefaultFont, Brushes.Red, nodes[i].Position);
                }
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                if(nodes[i].isValid)
                    for (int j = 0; j < 8; j++)
                        if (nodes[i].AdjacentEdges[j] != null && nodes[nodes[i].AdjacentEdges[j].Destination].isValid)
                            g.DrawLine(Pens.DarkBlue, nodes[i].Position, nodes[nodes[i].AdjacentEdges[j].Destination].Position);
            }

            g.Dispose();

        }

        private static void DrawCrossHair(Graphics g, PointF pf, float raduis, Pen pen)
        {
            g.DrawEllipse(pen, pf.X - raduis, pf.Y - raduis, raduis * 2, raduis * 2);
            g.DrawLine(pen, new PointF(pf.X - raduis - 3, pf.Y), new PointF(pf.X + raduis + 3, pf.Y));
            g.DrawLine(pen, new PointF(pf.X, pf.Y - raduis - 3), new PointF(pf.X, pf.Y + raduis + 3));
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            float raduis = 4;
            Graphics g = this.CreateGraphics();
            if (primary_secondary == 0)
            {
                primaryPoint.X = e.X;
                primaryPoint.Y = e.Y;

                DrawCrossHair(g, primaryPoint, raduis, Pens.Red);

                primary_secondary++;
                MessageBox.Show("here! 0");
            }
            else
            {

                secondaryPoint.X = e.X;
                secondaryPoint.Y = e.Y;
                primary_secondary = 0;

                DrawCrossHair(g, secondaryPoint, raduis, Pens.Red);

                MessageBox.Show("here! 1");
            }


           
            g.Dispose();
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            imageFilePath = "e:\\vstemp\\sampleMapFile.bmp";
            this.BackgroundImage = Image.FromFile(imageFilePath);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();

            g.Clear(Color.White);

            this.BackgroundImage = Image.FromFile(imageFilePath);
        }
    }
}
