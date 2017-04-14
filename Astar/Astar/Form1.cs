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
        //public List<Grid> map;
        MAP map;
        int[] parentList;
        int obsWidth = 5;
        int obsHeight = 5;
        int obsNum = 50;
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
                            map.type[i * width + j] = 1;
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

            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            

            navigate nav = new navigate(map, startPoint, goalPoint);

            if(nav.GetPath()==true)
            {
                parentList = nav.parentList;
                int parentIndex = goalPoint.Y * pictureBox1.Width + goalPoint.X;
                int childRow = goalPoint.X;
                int childCol = goalPoint.Y;
                int tempCol = 0;
                int tempRow = 0;
                while (nav.parentList[parentIndex] != 0)
                {
                    int parentCol = nav.parentList[parentIndex] / pictureBox1.Width;
                    int parentRow = nav.parentList[parentIndex] % pictureBox1.Width;

                    p1.X = parentRow;
                    p1.Y = parentCol;
                    p2.X = childRow;
                    p2.Y = childCol;

                    if (IsExistObs(p1, p2) == true)
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
                        if (tempCol == startPoint.Y && tempRow == startPoint.X)
                            g.DrawLine(p, p1, p2);
                        parentIndex = parentCol * pictureBox1.Width + parentRow;
                    }
                }
            }
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
                    if (map.mapdata[(down + step) * pictureBox1.Width + p1.X]!=254)
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
                    if (map.mapdata[p1.Y * pictureBox1.Width + down + step]!= 254)
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
                        if (map.mapdata[(int)(result) * pictureBox1.Width + down+step] != 254)
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
                        if (map.mapdata[(down + step) * pictureBox1.Width + (int)result] != 254)
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
            p = new Pen(Color.White, 1);
            g = pictureBox1.CreateGraphics();
            Point p1 = new Point();
            Point p2 = new Point();
            int parentIndex = goalPoint.Y * pictureBox1.Width + goalPoint.X;
            int childRow = goalPoint.X;
            int childCol = goalPoint.Y;
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

                if (IsExistObs(p1, p2) == true)
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
                    if (tempCol == startPoint.Y && tempRow == startPoint.X)
                        g.DrawLine(p, p1, p2);
                    parentIndex = parentCol * pictureBox1.Width + parentRow;
                }
            }
            btnStartPoint.Enabled = true;
            btnEndPoint.Enabled = true;
            btnNavgate.Enabled = true;
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
            else
            {
                setEndPointFlag = 0;
                g.FillRectangle(new SolidBrush(Color.Red), picBoxPosition.X, picBoxPosition.Y, 1, 1);
                goalPoint = picBoxPosition;
            }
        }



    }

    //public struct TWD
    //{
    //    public int x, y;
    //    public int cost;
    //}
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

}
