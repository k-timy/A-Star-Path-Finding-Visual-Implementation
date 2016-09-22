/*
    A-Star Search Visualizer
    A Program that gets an image that consists of black and white colors.
    which represent walkable areas of a map(white) and its obstacles(black)
    and gets two points and finds the optimal path possible between the two
    given points.   
 
    Copyright (C) 2016  Kourosh Teimouri Baghaei

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Dijkestra_Tiled_Graph_Visualizer
{
    class TiledGraphGenerator
    {
        TiledGraphNode[] graphNodes;

        /*
        public TiledGraphNode[] GraphNodes
        {
            get { return graphNodes; }
        }
        */

        private PointF startPosition;
        /// <summary>
        /// The Lower Right point of Graph in Cartesian coordinates.
        /// </summary>
        private PointF endPosition;
        
        /// <summary>
        /// The Upper Left point of Graph in Cartesian coordinates.
        /// </summary>
        public PointF StartPosition
        {
            get { return startPosition; }
        }

        private float LengthOfEdge;
        private float graphHeight;
        private float graphWidth;

        public float GraphHeight
        {
            get { return graphHeight; }
        }
        public float GraphWidth
        {
            get { return graphWidth; }
        }

        private int GraphHeightMosaics;
        private int GraphWidthMosaics;

        public int GraphHeightNodeCount
        {
            get { return GraphHeightMosaics; }
        }

        public int GraphWidthNodeCount
        {
            get { return GraphWidthMosaics; }
        }
        private int[, ,] pixelData;      

        int totalNumNodes;

        public int TotalNumNodes 
        {
            get { return totalNumNodes; }
        }

        public TiledGraphGenerator(PointF startPos, PointF endPos, float lengthOfEdge,Image imgFilter)
        {
            // Prepare Image File for filtering graph nodes:

            Bitmap bitmap = new Bitmap(imgFilter);

            int imgHeight = imgFilter.Height;
            int imgWidth = imgFilter.Width;

            pixelData  = new int[imgWidth, imgHeight, 3];

            for (int yi = 0; yi < imgHeight; yi++)
                for (int xi = 0; xi < imgWidth; xi++)
                {
                    Color pixel = bitmap.GetPixel(xi, yi);
                    pixelData[xi, yi, 0] = pixel.R;
                    pixelData[xi, yi, 1] = pixel.G;
                    pixelData[xi, yi, 2] = pixel.B;
                }

            bitmap.Dispose();


            // Prepare Graph nodes:

            LengthOfEdge = lengthOfEdge;
            startPosition = startPos;
            endPosition = endPos;

            graphHeight = (endPos.Y - startPos.Y);
            graphWidth = (endPos.X - startPos.X);

            GraphHeightMosaics = (int)(graphHeight / LengthOfEdge);
            GraphWidthMosaics = (int)(graphWidth / LengthOfEdge);

            Console.WriteLine("Height mosaics : {0}", GraphHeightMosaics);
            Console.WriteLine("Width mosaics : {0}", GraphWidthMosaics);

            totalNumNodes = (GraphWidthMosaics) * (GraphHeightMosaics);
            
            graphNodes = new TiledGraphNode[totalNumNodes];

            float tmpX = startPosition.X;
            float tmpY = startPosition.Y;

            int index = 0;

            for (int y = 0; y < GraphHeightMosaics; y++)
            {
                for (int x = 0; x < GraphWidthMosaics; x++)
                {
                    PointF posTmp = new PointF(tmpX + x * LengthOfEdge, tmpY + y * LengthOfEdge);

                    if (isPointValid(posTmp, LengthOfEdge / 2,graphHeight,graphWidth))
                    {
                        graphNodes[index] = new TiledGraphNode(index, posTmp, true);
                    }
                    else
                    {
                        graphNodes[index] = new TiledGraphNode(index, posTmp, false);
                    }

                    index++;
                }
            }




        }
        public TiledGraphNode[] getNodes()
        {
            return graphNodes;
        }
        private bool isPointValid(PointF centerPos, float radius,float maxHeight,float maxWidth)
        {
            if (residesInBlack(centerPos,maxHeight,maxWidth))
                return false;

            PointF tmpDir = new PointF(0,0);
            
            float angle = 0;
            float PIover4 = (float)Math.PI / 4;
            for (int i = 0; i < 8; i++)
            {
                tmpDir.X = (float)Math.Cos(angle) * radius;
                tmpDir.Y = (float)Math.Sin(angle) * radius;
                angle += PIover4;

                float x = tmpDir.X + centerPos.X;
                float y = tmpDir.Y + centerPos.Y;

                if (x >= maxWidth || x < 0 || y >= maxHeight || y < 0)
                    continue;
                PointF tmp = new PointF(x,y);
                if (residesInBlack(tmp,maxHeight,maxWidth))
                    return false;
            }
            return true;
        }
        private bool residesInBlack(PointF pos,float maxHeight,float maxWidth)
        {
            int tmpX = (int)pos.X;
            int tmpY = (int)pos.Y;

            if (tmpX >= maxWidth || tmpX < 0 || tmpY >= maxHeight || tmpY < 0)
                return true;
            if (pixelData[tmpX, tmpY, 0] == 0 && pixelData[tmpX, tmpY, 1] == 0 && pixelData[tmpX, tmpY, 2] == 0)
                return true;
            
            return false;
        }

        public int getIndexOfPoint(PointF pf)
        {

            int xi = (int)((pf.X / LengthOfEdge) - 0.5f);
            int yi = (int)((pf.Y / LengthOfEdge) - 0.5f);

            return yi * GraphWidthMosaics + xi;
        }
    }
    #region Tiled Graph Node

    class TiledGraphNode
    {
        public bool isValid;
        int index;
        PointF position;

        public PointF Position
        {
            get { return position; }
        }

        Edge [] adjacentEdges;

        public Edge[] AdjacentEdges
        {
            get { return adjacentEdges; }
        }

        public int Index
        {
            get { return index; }
        }


        public TiledGraphNode(int index,PointF position,bool validity)
        {
            this.index = index;
            this.position = position;
            isValid = validity;
            if(isValid)
                adjacentEdges = new Edge[8];
        }

        public void setNeighbours(TiledGraphNode[] nodes,int mosaicHeight,int mosaicWidth,float edgeOfSquare)
        {
            if ((index / mosaicWidth != mosaicHeight - 1 && index / mosaicWidth != 0) && (index % mosaicWidth != 0 && index % mosaicWidth != mosaicWidth - 1))
            {
                adjacentEdges[0] = new Edge(this.Index,index - mosaicWidth - 1,Form1.Radical2 * edgeOfSquare); // Upper Left
                adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top
                adjacentEdges[2] = new Edge(this.Index, index - mosaicWidth + 1, Form1.Radical2 * edgeOfSquare); // Upper Right

                adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare); // Left
                adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare); // Right

                adjacentEdges[5] = new Edge(this.Index,index + mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Lower Left
                adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth , edgeOfSquare); // Down

                
                adjacentEdges[7] = new Edge(this.Index,index + mosaicWidth + 1,edgeOfSquare * Form1.Radical2); // Lower Right
            }
            else
            {
                if (index % mosaicWidth != 0 && index % mosaicWidth != mosaicWidth - 1)
                {
                    if (index / mosaicWidth != mosaicHeight - 1 && index / mosaicWidth == 0) // Nodes sitting on Upper border
                    {
                        adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare); // Left
                        adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare); // Right
                        adjacentEdges[5] = new Edge(this.Index,index + mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Lower Left
                        adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth , edgeOfSquare); // Down
                        adjacentEdges[7] = new Edge(this.Index,index + mosaicWidth + 1, edgeOfSquare * Form1.Radical2); // Lower Right
                    }
                    if (index / mosaicWidth == mosaicHeight - 1 && index / mosaicWidth != 0) // Nodes sitting on Lower border
                    {
                        adjacentEdges[0] = new Edge(this.Index,index - mosaicWidth - 1,Form1.Radical2 * edgeOfSquare); // Upper Left
                        adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top
                        adjacentEdges[2] = new Edge(this.Index,index - mosaicWidth + 1,Form1.Radical2 * edgeOfSquare); // Upper Right
                        adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare); // Left
                        adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare); // Right

                    }
                }
                if (index / mosaicWidth != mosaicHeight - 1 && index / mosaicWidth != 0)
                {
                    if (index % mosaicWidth == 0) // Nodes sitting on Left border:
                    {
                        adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top                        
                        adjacentEdges[2] = new Edge(this.Index,index - mosaicWidth + 1,edgeOfSquare * Form1.Radical2); // Upper Right
                        adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare);   // Right                           
                        adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth,edgeOfSquare); // Down
                        adjacentEdges[7] = new Edge(this.Index,index + mosaicWidth + 1,edgeOfSquare * Form1.Radical2); // Lowe Right
                    }
                    if (index % mosaicWidth == mosaicWidth - 1) // Nodes sitting on Right border:
                    {
                        adjacentEdges[0] = new Edge(this.Index,index - mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Upper Left
                        adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top 
                        adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare);               // Left
                        adjacentEdges[5] = new Edge(this.Index,index + mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Lower Left                        
                        adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth,edgeOfSquare); // Down
                    }
                } // if                  
            } // else

            if (index / mosaicWidth == 0)
            {
                if (index % mosaicWidth == 0)
                {
                    adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare); // Right
                    adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth,edgeOfSquare); // Down
                    adjacentEdges[7] = new Edge(this.Index,index + mosaicWidth + 1,edgeOfSquare * Form1.Radical2); // Lower Right
                }
                if (index % mosaicWidth == mosaicWidth - 1)
                {
                    adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare); // Left
                    adjacentEdges[5] = new Edge(this.Index,index + mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Lower Left
                    adjacentEdges[6] = new Edge(this.Index,index + mosaicWidth,edgeOfSquare); // Down
                }
            }
            else if (index / mosaicWidth == mosaicHeight - 1)
            {
                if (index % mosaicWidth == 0)
                {
                    adjacentEdges[0] = new Edge(this.Index,index - mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Upper Left

                    adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top                    
                    adjacentEdges[4] = new Edge(this.Index,index + 1,edgeOfSquare); // Right
                }
                if (index % mosaicWidth == mosaicWidth - 1)
                {
                    adjacentEdges[0] = new Edge(this.Index,index - mosaicWidth - 1,edgeOfSquare * Form1.Radical2); // Upper Left
                    adjacentEdges[1] = new Edge(this.Index,index - mosaicWidth,edgeOfSquare); // Top
                    adjacentEdges[3] = new Edge(this.Index,index - 1,edgeOfSquare); // Left
                }
            }           
        } // method

    } // class

        #endregion
}
