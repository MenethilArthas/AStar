using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BinHeap;
using System.IO;
namespace Astar
{
 
    public partial class Form1 : Form
    {
        public int setStartPointFlag = 0;
        public int setEndPointFlag = 0;
        public Point startPoint;
        public Point goalPoint;
        public Graphics g ;
        public Pen p;
        public List<Grid> map;
        //byte[] map;
        BinHeap.BinHeap open;
        bool findFlag = false;


        int obsWidth = 20;
        int obsHeight = 20;
        int obsNum = 50;
        public Form1()
        {
            InitializeComponent();           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            //string path = "C:\\Users\\Arthas\\Desktop\\毕业设计\\AStar-C\\AStar-C\\mymap.pgm";
            //if(File.Exists(path))
            //{
            //    FileStream fs = new FileStream(path, FileMode.Open);
            //    StreamReader sr = new StreamReader(fs,Encoding.UTF7);
            //    string str = string.Empty;
            //    str = sr.ReadLine();//p5
            //    str = sr.ReadLine();//width height
            //    string[] size=str.Split(' ');
            //    int width = Convert.ToInt32(size[0]);
            //    int height = Convert.ToInt32(size[1]);
            //    float scaleX = (float)width / this.pictureBox1.Width;
            //    float scaleY = (float)height/ this.pictureBox1.Height;
            //    this.pictureBox1.Width = width;
            //    this.pictureBox1.Height = height;
            //    this.Width = (int)(this.Width * scaleX);
            //    this.Height = (int)(this.Height * scaleY);
            //    str = sr.ReadLine();//255

            //    str = sr.ReadLine();
            //    //str = sr.ReadLine();
            //    map = new byte[width * height];
            //    for(int i=0;i<height;i++)
            //    {
            //        for (int j = 0; j < width;j++ )
            //        {
            //            map[i * width + j] = (byte)str[i * width + j];
            //        }
            //    }
            //}
            map = new List<Grid>();
            //初始化地图，用pointlist存储,地图只存储障碍物信息
            for (int i = 0; i < pictureBox1.Size.Height; i++)
            {
                for (int j = 0; j < pictureBox1.Size.Width; j++)
                {
                    Grid tmpPoint = new Grid();
                    tmpPoint.occupy = 0;
                    map.Add(tmpPoint);
                }
            }
        }
 

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
           if(findFlag==false)
           {
               g = pictureBox1.CreateGraphics();
               Point picBoxPosition = pictureBox1.PointToClient(Control.MousePosition);
               if (setStartPointFlag == 1)
               {
                   setStartPointFlag = 0;
                   g.FillRectangle(new SolidBrush(Color.Blue), picBoxPosition.X, picBoxPosition.Y, 1, 1);
                   startPoint = picBoxPosition;
               }
               else
               {
                   setEndPointFlag = 0;
                   g.FillRectangle(new SolidBrush(Color.Red), picBoxPosition.X, picBoxPosition.Y, 1, 1);
                   goalPoint = picBoxPosition;
               }
           }
           else
           {
                toolTip1.Dispose();
                toolTip1 = new ToolTip();

                Point picBoxPosition = pictureBox1.PointToClient(Control.MousePosition);
                BinHeap.NodeStruct debugNode = new BinHeap.NodeStruct();
                debugNode.point = picBoxPosition;
                int index = open.IsExist(debugNode);
                if (index != 0)
                {
                    this.toolTip1.Show("x=" + picBoxPosition.X + "  y=" + picBoxPosition.Y + "fvalue=" + open.items[index].fValue, this.pictureBox1);
                }
                else
                {
                    this.toolTip1.Show("x=" + picBoxPosition.X + "  y=" + picBoxPosition.Y, this.pictureBox1);
                }
               
           }
        }

        private void btnStartPoint_Click(object sender, EventArgs e)
        {
            btnStartPoint.Enabled = false;
            setStartPointFlag = 1;         
        }

        private void btnEndPoint_Click(object sender, EventArgs e)
        {
            btnEndPoint.Enabled = false;
            setEndPointFlag = 1;
        }

        private void btnCreateObs_Click(object sender, EventArgs e)
        {
            btnCreateObs.Enabled = false;
            g = pictureBox1.CreateGraphics();

            SolidBrush brush = new SolidBrush(Color.Black);
            Random ro = new Random();

            for (int i = 0; i < obsNum; i++)
            {
                int xDown = 0;
                int xUp = pictureBox1.Width - obsWidth;
                int yDown = 0;
                int yUp = pictureBox1.Height - obsHeight;
                int obsX = ro.Next(xDown, xUp);
                int obsY = ro.Next(yDown, yUp);

                g.FillRectangle(brush, obsX, obsY, obsWidth, obsHeight);
                //将对应的map上的点障碍物信息设为1
                for (int j = 0; j < obsWidth; j++)
                {
                    for (int k = 0; k < obsHeight; k++)
                    {
                        map[(obsY + k) * pictureBox1.Width + (obsX + j)].occupy = 1;
                    }
                }


            }
            btnCreateObs.Enabled = true;
            //g = pictureBox1.CreateGraphics();
            //SolidBrush blackBrush = new SolidBrush(Color.Black);
            //SolidBrush grayBrush = new SolidBrush(Color.Gray);
            //SolidBrush whiteBrush = new SolidBrush(Color.White);
            //for (int i = 0; i < this.pictureBox1.Height; i++)
            //{
            //    for (int j = 0; j < this.pictureBox1.Width; j++)
            //    {
                   
            //        if (map[i * this.pictureBox1.Width + j] == 254)
            //            g.FillRectangle(whiteBrush, j, i, 1, 1);
            //        else if (map[i * this.pictureBox1.Width + j] == 205)
            //            g.FillRectangle(grayBrush, j, i, 1, 1);
            //        else if (map[i * this.pictureBox1.Width + j] == 0)
            //            g.FillRectangle(blackBrush, j, i, 1, 1);
            //    }
            //}


        }

        private void btnNavgate_Click(object sender, EventArgs e)
        {
            
            btnNavgate.Enabled = false;
            BinHeap.NodeStruct startNode = new BinHeap.NodeStruct();
            BinHeap.NodeStruct popNode;
            

            open= new BinHeap.BinHeap(pictureBox1.Width*pictureBox1.Height);
            
            int[] closed = new int[pictureBox1.Width * pictureBox1.Height];
            for (int i = 0; i < pictureBox1.Width * pictureBox1.Height; i++)
                closed[i] = 0;

            int[] parentList=new int[pictureBox1.Width*pictureBox1.Height];
            for (int i = 0; i < pictureBox1.Width * pictureBox1.Height; i++)
                parentList[i] = 0;

            int col, row;

            p = new Pen(Color.Red, 1);
            g = pictureBox1.CreateGraphics();

            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);

            TWD[] twds=new TWD[8];
            twds[0].x = 0; twds[0].y = -1;    twds[0].cost=10  ;  //上
            twds[1].x = 0; twds[1].y = 1;     twds[1].cost=10  ;  //下
            twds[2].x = -1; twds[2].y = 0;    twds[2].cost=10  ;  //左
            twds[3].x = 1; twds[3].y = 0;     twds[3].cost=10  ;  //右
            twds[4].x = -1; twds[4].y = -1;   twds[4].cost=14  ;  //左上
            twds[5].x = 1; twds[5].y = -1;    twds[5].cost=14  ; //右上
            twds[6].x = -1; twds[6].y = 1;    twds[6].cost=14  ; //左下
            twds[7].x = 1; twds[7].y = 1;     twds[7].cost=14  ; //右下


            startNode.point = startPoint;
            startNode.hValue = CalcDist(startNode.point, goalPoint);
            startNode.gValue = 0;
            startNode.fValue=startNode.hValue+startNode.gValue;
            open.InsertItem(startNode);

            if (startPoint != goalPoint)
            {
               //open列表不为空
                while(open.Size!=0&&findFlag!=true)
                {

                    popNode = open.PopMinItem();
                    //Console.WriteLine("{0}...{1}...{2}", popNode.point.X, popNode.point.Y, popNode.hValue);
                    closed[popNode.point.Y * pictureBox1.Width + popNode.point.X] = 1;
                    for (int i = 0; i < 8; i++)
                    {
                        
                        row = popNode.point.X + twds[i].x;
                        col = popNode.point.Y + twds[i].y;
                        //在地图内部
                        if (row >= 0 && row <pictureBox1.Width && col >= 0 && col <pictureBox1.Height)
                        {
                            //不是障碍物且不在closed列表里
                            if (closed[col * pictureBox1.Width + row] != 1 && map[col * pictureBox1.Width + row].occupy != 1 )
                            {
                                if(row!=goalPoint.X||col!=goalPoint.Y)
                                {
                                    BinHeap.NodeStruct tmpNode = new BinHeap.NodeStruct();                                 
                                    tmpNode.point.X = row;
                                    tmpNode.point.Y = col;
                                    tmpNode.hValue = CalcDist(tmpNode.point, goalPoint);
                                    tmpNode.gValue = popNode.gValue + twds[i].cost;
                                    tmpNode.fValue = tmpNode.hValue + tmpNode.gValue;
                              
                                    //如果节点已经在open列表中，比较之前的fvalue和新的fvalue
                                    int index=open.IsExist(tmpNode);
                                    if (index!=0)
                                    {                                       
                                        if (open.items[index].fValue > tmpNode.fValue)
                                        {
                                            open.items[index].fValue = tmpNode.fValue;
                                            //改变tmpNode的父节点索引
                                            parentList[tmpNode.point.Y * pictureBox1.Width + tmpNode.point.X] = popNode.point.Y * pictureBox1.Width + popNode.point.X;
                                        }
                                    }
                                    else
                                    {
                                        //添加tmpNode的父节点索引
                                        parentList[tmpNode.point.Y * pictureBox1.Width + tmpNode.point.X] = popNode.point.Y * pictureBox1.Width + popNode.point.X;
                                        open.InsertItem(tmpNode);
                                    }
                                 
                                }
                                else
                                {
                                    
                                    row -= twds[i].x;
                                    col -= twds[i].y;
                                    Console.WriteLine("{0}...{1}", row, col);
                                    int parentIndex = col * pictureBox1.Width + row;
                                    int childRow = row;
                                    int childCol = col;
                                    int tempCol = 0;
                                    int tempRow = 0;
                                    while (parentList[parentIndex] != 0)
                                    {
                                        int parentCol = parentList[parentIndex] / pictureBox1.Width;
                                        int parentRow = parentList[parentIndex] % pictureBox1.Width;

                                        p1.X = parentRow;
                                        p1.Y = parentCol;
                                        p2.X = childRow;
                                        p2.Y = childCol;

                                        if(IsExistObs(p1,p2)==true)
                                        {
                                            childRow = tempRow;
                                            childCol = tempCol;
                                            parentIndex = childCol * pictureBox1.Width + childRow;
                                            p1.X = tempRow;
                                            p1.Y = tempCol;
                                            
                                            g.DrawLine(p, p1, p2);
                                        }
                                        else
                                        {
                                            tempCol = parentCol;
                                            tempRow = parentRow;
                                            if(tempCol==startPoint.Y&&tempRow==startPoint.X)
                                                g.DrawLine(p, p1, p2);
                                            parentIndex = parentCol * pictureBox1.Width + parentRow;
                                        }
                                        
                                        
                                    }
 
                                    findFlag = true;
                                    break;
                                }

                            }
                        }

                    }
                }
              
 
            }

        }
        public int CalcDist(Point start, Point goal)
        {
            
            int distX = (int)(Math.Abs(start.X - goal.X)*10);
            int distY = (int)(Math.Abs(start.Y - goal.Y)*10);
            return (distX + distY);
            //return (10*(distX+distY)+(14-20)*Math.Min(distX,distY));
        }

        public bool IsExistObs(Point p1,Point p2)
        {
            int size = 0;
            int up=0;
            int down=0;
            double result = 0;
            //p1p2垂直于x轴
            if (p1.X == p2.X)
            {
                up=Math.Max(p1.Y,p2.Y);
                down=Math.Min(p1.Y,p2.Y);
                size = up-down;
                for (int step = 0; step < size; step++)
                {
                    if (map[(down + step) * pictureBox1.Width + p1.X].occupy ==1)
                    {
                        return true;
                    }
                }
                return false;
            }
            //p1p2垂直于y轴
            else if (p1.Y == p2.Y)
            {
                up = Math.Max(p1.X, p2.X);
                down = Math.Min(p1.X, p2.X);
                size = up - down;
                for (int step = 0; step < size; step++)
                {
                    if (map[p1.Y * pictureBox1.Width + down + step].occupy == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Func func = new Func(p1, p2);
                if(func.type==0)
                {
                    up = Math.Max(p1.X, p2.X);
                    down = Math.Min(p1.X, p2.X);
                    size = up - down;
                    for (int step = 1; step < size; step++)
                    {
                        result = func.CalcResult(down + step);
                        if (map[(int)(result) * pictureBox1.Width + down+step].occupy == 1)
                        {
                            return true;
                        }

                    }
                    return false;
                }
                else
                {
                    up = Math.Max(p1.Y, p2.Y);
                    down = Math.Min(p1.Y, p2.Y);
                    size = up - down;
                    for (int step = 0; step < size; step++)
                    {
                        result = func.CalcResult(down + step);
                        if (map[(down + step) * pictureBox1.Width + (int)result].occupy == 1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        private void btnClearPath_Click(object sender, EventArgs e)
        {
            findFlag = false;
            open.ReInitHeap();
            g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            SolidBrush brush = new SolidBrush(Color.Black);

            //将对应的map上的点障碍物信息设为1
            for (int j = 0; j < pictureBox1.Width * pictureBox1.Height; j++)
            {
                if (map[j].occupy == 1)
                {
                    int Col = j / pictureBox1.Width;
                    int Row = j % pictureBox1.Width;
                    g.FillRectangle(brush, Row, Col, 1, 1);
                }


            }
            btnStartPoint.Enabled = true;
            btnEndPoint.Enabled = true;
            btnNavgate.Enabled = true;
        }

    }

    public struct TWD
    {
        public int x, y;
        public int cost;
    }
    public class Grid
    {
        public int occupy;
    }
    public class Func
    {
        public double k;
        public double b;
        public int type;//为0代表横向迭代，即给定x求y,为1代表纵向迭代，即给定y求x
        public Func (Point A,Point B)
        {
            k = (double)(B.Y - A.Y) / (double)(B.X - A.X);
            b = B.Y - k * B.X;
            if (Math.Abs(k) < 1)
                type = 0;
            else
                type = 1;
        }
        public double CalcResult(float var)
        {
            if (type == 0)
                return (k * var + b);
            else
                return (var - b) / k;
        }
    }
    //public class AstarNode:IComparable<AstarNode>
    //{
    //    public Point point;
    //    public int gValue;
    //    public int fValue;
    //    public int hValue;
    //    public int CompareTo(AstarNode other)
    //    {

    //        int value =(int)( this.hValue - other.hValue);
    //        return value;
    //    }
    //}
}
