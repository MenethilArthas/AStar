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
        public Stack<Point> keyPoints;
        navigate nav;
        MAP map;
        private int[] costMap;
        int obsWidth = 5;
        int obsHeight = 5;
        int obsNum = 50;
        int obsCost = 200;
        public Form1()
        {
            InitializeComponent();           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            string path = "C:\\Users\\Arthas\\Desktop\\毕业设计\\AStar\\AStar\\AStar\\mymap.pgm";
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs, Encoding.UTF7);
                string str = string.Empty;
                str = sr.ReadLine();//p5
                str = sr.ReadLine();//comment context
                str = sr.ReadLine();//width height
                string[] size = str.Split(' ');
                int width = Convert.ToInt32(size[0]);
                int height = Convert.ToInt32(size[1]);

                map.width = width;
                map.height = height;

                costMap = new int[width * height];
                float scaleX = (float)width / this.pictureBox1.Width;
                float scaleY = (float)height / this.pictureBox1.Height;
                this.pictureBox1.Width = width;
                this.pictureBox1.Height = height;
                this.Width = (int)(this.Width * scaleX);
                this.Height = (int)(this.Height * scaleY);
                str = sr.ReadLine();//255

                str = sr.ReadLine();
                //str = sr.ReadLine();
                map.mapdata = new byte[width * height];
                map.type=new byte[width*height];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        map.mapdata[i * width + j] = (byte)str[i * width + j];
                        if (map.mapdata[i * width + j] == 0)
                        {
                            map.type[i * width + j] = 1;
                            int qunima = 5;
                            for(int k=i-qunima;k<=i+qunima;k++)
                            {
                                for (int w = -qunima; w <= qunima;w++ )
                                {
                                    costMap[k * width + j + w] = obsCost;
                                }
  
                            }
                        }
                           
                        else
                            map.type[i * width + j] = 0;
                    }
                }
            }
        }

        private void btnImportMap_Click(object sender, EventArgs e)
        {
            btnImportMap.Enabled = false;
            g = pictureBox1.CreateGraphics();
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            for (int i = 0; i < this.pictureBox1.Height; i++)
            {
                for (int j = 0; j < this.pictureBox1.Width; j++)
                {

                    if (map.mapdata[i * this.pictureBox1.Width + j] == 254)
                        g.FillRectangle(whiteBrush, j, i, 1, 1);

                    else if (map.mapdata[i * this.pictureBox1.Width + j] == 0)
                        g.FillRectangle(blackBrush, j, i, 1, 1);
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
            g = pictureBox1.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.Blue);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            int row = 0;
            int col = 0;
            for(int i=0;i<map.width*map.height;i++)
            {
                if(map.mapdata[i]==0&&map.type[i]==1)
                {
                    row = i % map.width-obsWidth;
                    col = i / map.width-obsHeight;
                    g.FillRectangle(brush, row, col, obsWidth*2, obsHeight*2);

                    g.FillRectangle(blackBrush, row+obsWidth, col+obsHeight, 1, 1);
                    for (int j = col; j < col+obsHeight*2; j++)
                    {
                        for (int k = row; k < row+obsWidth*2; k++)
                        {
                            map.mapdata[j * map.width + k] = 0;
                        }
                    }
                }
            } 
        }

        private void btnNavgate_Click(object sender, EventArgs e)
        {
          
            btnNavgate.Enabled = false;

            p = new Pen(Color.Red, 1);
            g = pictureBox1.CreateGraphics();

            Point childPoint = new Point(0, 0);
            Point parentPoint = new Point(0, 0);
            navigate nav = new navigate(map, startPoint, goalPoint, costMap);

            if(nav.GetPath()==true)
            {
                keyPoints = nav.keyPoints;
                //childPoint = keyPoints.Pop();//startPoint
                
                //while(keyPoints.Count!=0)
                //{
                //    parentPoint = keyPoints.Pop();
                //    g.DrawLine(p, childPoint, parentPoint);
                //    childPoint = parentPoint;
                //}                
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Point curPoint = new Point(0, 0);
            Point tarPoint = new Point(0, 0);
            int interval = 1;
            int vel = 2;
            double deltaX = 0.0;
            double deltaY = 0.0;
            double orientation = 0.0;

            p = new Pen(Color.Red, 1);
            g = pictureBox1.CreateGraphics();
            nav = new navigate(map, startPoint, goalPoint, costMap);
            nav.GetPath();
            keyPoints = nav.keyPoints;
            p1 = keyPoints.Pop();//startPoint
            while (keyPoints.Count != 0)
            {
                tarPoint = keyPoints.Pop();         
                while(Math.Abs(p2.X-tarPoint.X)>1||Math.Abs(p2.Y-tarPoint.Y)>1)
                {
                    orientation = CalcRad(p1, tarPoint);
                    deltaX = vel * interval * Math.Cos(orientation);
                    deltaY = vel * interval * Math.Sin(orientation);
                    p2.X = p1.X + (int)Math.Round(deltaX,0);
                    p2.Y = p1.Y + (int)Math.Round(deltaY, 0);
                    g.DrawLine(p, p1, p2);
                    //System.Threading.Thread.Sleep(100);
                    Delay(100);
                    if(nav.IsExistObs(p2,tarPoint)==true)
                    {
                        nav = new navigate(map, p2, goalPoint, costMap);
                        nav.GetPath();
                        keyPoints = nav.keyPoints;
                        p1 = keyPoints.Pop();
                        tarPoint = keyPoints.Pop();
                    }
                    else
                    {
                        p1 = p2;
                    }
                }
                curPoint = tarPoint;
                //g.DrawLine(p, childPoint, parentPoint);
                
            }
           
        }

        private void btnClearPath_Click(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics();
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            for (int i = 0; i < this.pictureBox1.Height; i++)
            {
                for (int j = 0; j < this.pictureBox1.Width; j++)
                {

                    if (map.mapdata[i * this.pictureBox1.Width + j] == 254)
                        g.FillRectangle(whiteBrush, j, i, 1, 1);

                    else if (map.mapdata[i * this.pictureBox1.Width + j] == 0)
                        g.FillRectangle(blackBrush, j, i, 1, 1);
                }
            }
            btnNavgate.Enabled = true;
            btnStartPoint.Enabled = true;
            btnEndPoint.Enabled = true;
            btnTest.Enabled = true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            g = pictureBox1.CreateGraphics();
            Point picBoxPosition = pictureBox1.PointToClient(Control.MousePosition);
            if (setStartPointFlag == 1)
            {
                setStartPointFlag = 0;
                g.FillRectangle(new SolidBrush(Color.Blue), picBoxPosition.X, picBoxPosition.Y, 1, 1);
                startPoint = picBoxPosition;
            }
            else if(setEndPointFlag==1)
            {
                setEndPointFlag = 0;
                g.FillRectangle(new SolidBrush(Color.Red), picBoxPosition.X, picBoxPosition.Y, 1, 1);
                goalPoint = picBoxPosition;
            }
            else if(btnTest.Enabled==false)
            {
                int dynObsNum = 4;
                int x = picBoxPosition.X - dynObsNum;
                int y = picBoxPosition.Y - dynObsNum;
                g.FillRectangle(new SolidBrush(Color.Black), x, y, dynObsNum * 2, dynObsNum * 2);
                for (int i = y; i <= y + dynObsNum * 2; i++)
                {
                    for (int j = x; j < x + dynObsNum * 2; j++)
                    {
                        nav.map.mapdata[i * map.width + j] = 0;
                        map.mapdata[i * map.width + j] = 0;
                    }
                }
            }
        }
        double CalcRad(Point p1, Point p2)
        {
            double disX = p2.X - p1.X;
            double disY = p2.Y - p1.Y;
            double resultRad = 0.0;
            double distance = Math.Sqrt(Math.Pow(disX, 2.0) + Math.Pow(disY, 2.0));
            if (disY > 0)
                resultRad = Math.Acos(disX / distance);
            else
                resultRad = -Math.Acos(disX / distance);
            return resultRad;
        }
        void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }


    }


}
