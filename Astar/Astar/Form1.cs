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
using System.IO.Ports;
using System.IO;
using System.Threading;
namespace Astar
{
 
    public partial class Form1 : Form
    {
        public int setStartPointFlag = 0;
        public int setEndPointFlag = 0;
        public Point startPoint;
        public Point goalPoint;
        public Graphics g ;
        public Graphics g2;
        public Pen p;
        public Stack<Point> keyPoints;
        Navigate nav;
        PgmFile pgmFile;
        FileStream keyPoints_fs;
        FileStream calcResult_fs;
        FileStream laserData_fs;

        StreamWriter keyPoints_sw;
        StreamWriter calcResult_sw;
        StreamWriter laserData_sw;

        Rplidar rplidar;
        
        int obsWidth = 5;
        int obsHeight = 5;
       // int obsNum = 50;

        public Form1()
        {
            InitializeComponent();           
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            string logFIlePath;
            //创建日志文件
            logFIlePath = Environment.CurrentDirectory.ToString() + "\\KeyPoints.log";
            keyPoints_fs = new FileStream(logFIlePath, System.IO.FileMode.Append);
            keyPoints_sw = new StreamWriter(keyPoints_fs, System.Text.Encoding.Default);
            keyPoints_sw.WriteLine(DateTime.Now.ToString());

            logFIlePath = Environment.CurrentDirectory.ToString() + "\\CalcResult.log";
            calcResult_fs = new FileStream(logFIlePath, System.IO.FileMode.Append);
            calcResult_sw = new StreamWriter(calcResult_fs, System.Text.Encoding.Default);
            calcResult_sw.WriteLine(DateTime.Now.ToString());

            logFIlePath = Environment.CurrentDirectory.ToString() + "\\LaserData.log";
            laserData_fs = new FileStream(logFIlePath, System.IO.FileMode.Create);
            laserData_sw = new StreamWriter(laserData_fs, System.Text.Encoding.Default);
            laserData_sw.WriteLine(DateTime.Now.ToString());
            //读取pgm格式地图
            string path = "C:\\Users\\Arthas\\Desktop\\毕业设计\\AStar\\AStar\\AStar\\mymap.pgm";
            pgmFile = new PgmFile(path);
            pgmFile.Read_FileData();
            //重绘窗口
            //float scaleX = (float)(pgmFile.map.width-300) / this.pictureBox1.Width;
            //float scaleY = (float)(pgmFile.map.height-350) / this.pictureBox1.Height;
            int disX = this.Width - this.pictureBox1.Width;
            int disY = this.Height - this.pictureBox1.Height;
            this.pictureBox1.Width = pgmFile.map.width;
            this.pictureBox1.Height = pgmFile.map.height;
            this.pictureBox2.Width = this.pictureBox1.Width;
            this.pictureBox2.Height = this.pictureBox1.Height;
            Point location = new Point(20 + this.pictureBox1.Location.X+this.pictureBox1.Width,this.pictureBox1.Location.Y);

            this.pictureBox2.Location = location;
            this.Width = this.pictureBox1.Width * 2 + 200;//(int)(this.Width * scaleX);
            this.Height = this.pictureBox1.Height  + 150;//(int)(this.Height * scaleY);
            //获取串口号
            string[] serialPortName = SerialPort.GetPortNames();
            if (serialPortName.Length==0||(serialPortName.Length==1&&serialPortName[0]=="COM3"))
                MessageBox.Show("木有串口");
            else
            {
                foreach(string s in serialPortName)
                {
                    if(s!="COM3")
                        cbSerialName.Items.Add(s);
                }
                cbSerialName.SelectedIndex = 0;
            }
            //订阅委托
            serialPort.DataReceived += new SerialDataReceivedEventHandler(Serial_DataReceived);

            //设置默认起点坐标
            startPoint = new Point(310-100, 310-200);


            //rplidar = new Rplidar(startPoint);
            //rplidar.StartMotor();
            //rplidar.GetInfo();
            //Thread thread = new Thread(new ThreadStart(rplidar.Rplidar_DataReceived));
            //thread.Start();
        }

        private void btnImportMap_Click(object sender, EventArgs e)
        {
            btnImportMap.Enabled = false;
            g = pictureBox1.CreateGraphics();
            g2 = pictureBox2.CreateGraphics();
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            for (int i = 0; i < this.pictureBox1.Height; i++)
            {
                for (int j = 0; j < this.pictureBox1.Width; j++)
                {

                    if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 254)
                    {
                        g.FillRectangle(whiteBrush, j, i, 1, 1);
                        g2.FillRectangle(whiteBrush, j, i, 1, 1);
                    }
                        

                    else if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 0)
                    {
                        g.FillRectangle(blackBrush, j, i, 1, 1);
                        g2.FillRectangle(blackBrush, j, i, 1, 1);
                    }
                      
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
            SolidBrush brush = new SolidBrush(Color.Blue);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            int row = 0;
            int col = 0;
            int qunima = 5;
            for (int i = 0; i < pgmFile.map.width * pgmFile.map.height; i++)
            {
                if (pgmFile.map.mapdata[i] == 0 && pgmFile.map.type[i] == 1)
                {
                    row = i % pgmFile.map.width - obsWidth;
                    col = i / pgmFile.map.width - obsHeight;
                    g.FillRectangle(brush, row, col, obsWidth*2, obsHeight*2);
                    g.FillRectangle(blackBrush, row+obsWidth, col+obsHeight, 1, 1);
                    for (int j = col; j < col+obsHeight*2; j++)
                    {
                        for (int k = row; k < row+obsWidth*2; k++)
                        {
                            if ((j >= 0 && j < pgmFile.map.height) && ((k ) >= 0 && (k ) < pgmFile.map.width))
                            {
                                pgmFile.map.mapdata[j * pgmFile.map.width + k] = 0;
                                //设置代价地图
                                for (int q = j - qunima; q <= j + qunima; q++)
                                {
                                    for (int w = -qunima; w <= qunima; w++)
                                    {
                                        if ((q >= 0 && q < pgmFile.map.height) && ((k + w) >= 0 && (k + w) < pgmFile.map.width))
                                            pgmFile.costMap[q * pgmFile.map.width + k + w] = 100;
                                    }
                                }
                            }

                        }
                    }
                }
            } 
        }

        private void btnNavgate_Click(object sender, EventArgs e)
        {
          
            btnNavgate.Enabled = false;

            p = new Pen(Color.Blue, 1);
            //g = pictureBox1.CreateGraphics();

            Point childPoint = new Point(0, 0);
            Point parentPoint = new Point(0, 0);

            Navigate tempNav = new Navigate(pgmFile.map, startPoint, goalPoint, pgmFile.costMap);

            if (tempNav.GetPath() == true)
            {
                DrawPath(tempNav.keyPoints);
                keyPoints = tempNav.keyPoints;

                childPoint = keyPoints.Pop();//startPoint
               
                while (keyPoints.Count != 0)
                {
                    keyPoints_sw.WriteLine("x:{0}\ty:{1}", childPoint.X, childPoint.Y);
                    parentPoint = keyPoints.Pop();
                    g.DrawLine(p, childPoint, parentPoint);
                    childPoint = parentPoint;
                }                
            }
        }

        float coorX;
        float coorY;
        float angle;
        
        List<byte> buffer=new List<byte>(4096);
        byte[] bytesToSingle = new byte[22];
        public void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int length = serialPort.BytesToRead;
            byte[] Buff = new byte[length];
            serialPort.Read(Buff, 0, length);

            buffer.AddRange(Buff);
            while (buffer.Count >= 2)
            {
                if (buffer[0] == 'A' && buffer[1] == 'C')
                {
                    if (buffer.Count < 22)
                        break;
                    else
                    {
                        buffer.CopyTo(0, bytesToSingle, 0, 22);
                        coorX = BitConverter.ToSingle(bytesToSingle, 2);
                        coorY = -BitConverter.ToSingle(bytesToSingle, 6);
                        angle = (float)Math.PI - BitConverter.ToSingle(bytesToSingle, 10);
                        rplidar.coor.x = coorX;
                        rplidar.coor.y = coorY;
                        rplidar.coor.angle = angle;
                        //Console.WriteLine("x:{0}\ty:{1}\tangle:{2}\t", coorX, coorY, angle); 
                        buffer.RemoveRange(0, 22);
                    }
                }
                else
                {
                    buffer.RemoveAt(0);
                }

            }
        }

      
        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Point curPoint = new Point(0, 0);
            Point tarPoint = new Point(0, 0);
           // double interval = 0.01;
            int vel = 0;//速度200mm/s
            int omiga = 0;
            //double deltaX = 0.0;
            //double deltaY = 0.0;
            double orientation = 0.0;
            double errAngle = 0.0;
            double resolution = 0.05;
            byte[] sendBuff = new byte[4];
            string str;

            p = new Pen(Color.Blue, 1);
            //g = pictureBox1.CreateGraphics();

            //rplidar.StartExpressScan();
            rplidar.StartScan();
            
            nav = new Navigate(pgmFile.map, startPoint, goalPoint, pgmFile.costMap);
            if(nav.GetPath()==true)
            {
                DrawPath(nav.keyPoints);
                keyPoints = nav.keyPoints;
                p1 = keyPoints.Pop();//startPoint

                while (keyPoints.Count != 0)
                {  
                    tarPoint = keyPoints.Pop();
                    while (Math.Abs(p2.X - tarPoint.X) > 1 || Math.Abs(p2.Y - tarPoint.Y) > 1)
                    {
                        HandleLaserData();

                        orientation = CalcRad(p1, tarPoint);
                        errAngle = orientation - angle;
                        //需要转的角度超过180度，就从反方向转
                        if (errAngle > Math.PI)
                            errAngle = errAngle - Math.PI * 2;
                        else if (errAngle < -Math.PI)
                            errAngle = errAngle + Math.PI * 2;

                        omiga = (int)(errAngle * 3000);
                        calcResult_sw.WriteLine("angle={0}\torientation={1}\tomiga={2}\ttarPoint_x={3}\ttarPoint_y={4}\tp1_x={5}\tp1_y={6}", angle, orientation, omiga,tarPoint.X, tarPoint.Y, p1.X, p1.Y);
                        vel = 200 - (int)(Math.Abs(omiga) * 0.06);
                        if (vel <= 0)
                            vel = 0;
                        if (serialPort.IsOpen)
                        {
                            //帧头
                            str = "OL";
                            serialPort.Write(str);
                            //线速度
                            sendBuff[0] = (byte)(vel);
                            sendBuff[1] = (byte)(vel >> 8);
                            sendBuff[2] = (byte)(vel >> 16);
                            sendBuff[3] = (byte)(vel >> 24);
                            serialPort.Write(sendBuff, 0, 4);
                            //角速度
                            sendBuff[0] = (byte)(omiga);
                            sendBuff[1] = (byte)(omiga >> 8);
                            sendBuff[2] = (byte)(omiga >> 16);
                            sendBuff[3] = (byte)(omiga >> 24);
                            serialPort.Write(sendBuff, 0, 4);
                        }
                        else
                        {
                            MessageBox.Show("串口没有打开");
                            btnTest.Enabled = true;
                            return;
                        }
                        Delay(10);
                        p2.X = startPoint.X + (int)Math.Round(coorX / resolution, 0);
                        p2.Y = startPoint.Y + (int)Math.Round(coorY / resolution, 0);
                        //deltaX = vel * interval * Math.Cos(orientation);
                        //deltaY = vel * interval * Math.Sin(orientation);
                        //p2.X = p1.X + (int)Math.Round(deltaX, 0);
                        //p2.Y = p1.Y + (int)Math.Round(deltaY, 0);
                        g.DrawLine(p, p1, p2);

                        if (nav.IsExistObs(p2, tarPoint) == true)
                        {
                            nav = new Navigate(pgmFile.map, p2, goalPoint, pgmFile.costMap);
                            
                            if (nav.GetPath()==true)
                            {
                                DrawPath(nav.keyPoints);
                                keyPoints = nav.keyPoints;
                                p1 = keyPoints.Pop();
                                tarPoint = keyPoints.Pop();
                            }
                            else
                            {
                                MessageBox.Show("error!");
                                calcResult_sw.WriteLine("error!!!\tp2_x={0}\tpw_y={1}", p2.X, p2.Y);
                            }
                        }
                        else
                        {
                            p1 = p2;
                        }
                    }
                    curPoint = tarPoint;           
                }
                vel = 0;
                omiga = 0;
                if (serialPort.IsOpen)
                {
                    //帧头
                    str = "OL";
                    serialPort.Write(str);
                    //线速度
                    sendBuff[0] = (byte)(vel);
                    sendBuff[1] = (byte)(vel >> 8);
                    sendBuff[2] = (byte)(vel >> 16);
                    sendBuff[3] = (byte)(vel >> 24);
                    serialPort.Write(sendBuff, 0, 4);
                    //角速度
                    sendBuff[0] = (byte)(omiga);
                    sendBuff[1] = (byte)(omiga >> 8);
                    sendBuff[2] = (byte)(omiga >> 16);
                    sendBuff[3] = (byte)(omiga >> 24);
                    serialPort.Write(sendBuff, 0, 4);
                }
                else
                {
                    MessageBox.Show("串口没有打开");
                    btnTest.Enabled = true;
                    return;
                }
            }
            else
            {
                MessageBox.Show("error");
            }
            btnTest.Enabled = true;
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

                    if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 254)
                        g.FillRectangle(whiteBrush, j, i, 1, 1);

                    else if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 0)
                        g.FillRectangle(blackBrush, j, i, 1, 1);
                }
            }
            btnNavgate.Enabled = true;
           // btnStartPoint.Enabled = true;
            btnEndPoint.Enabled = true;
            btnTest.Enabled = true;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
           // g = pictureBox1.CreateGraphics();
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
                        nav.map.mapdata[i * pgmFile.map.width + j] = 0;
                        pgmFile.map.mapdata[i * pgmFile.map.width + j] = 0;
                    }
                }
            }
        }
        /*返回0~2*pi*/
        double CalcRad(Point p1, Point p2)
        {
            double disX = p2.X - p1.X;
            double disY = p2.Y - p1.Y;
            double resultRad = 0.0;
            double distance = Math.Sqrt(Math.Pow(disX, 2.0) + Math.Pow(disY, 2.0));
            if (disY > 0)
                resultRad = Math.Acos(disX / distance);
            else
                resultRad = Math.PI * 2 - Math.Acos(disX / distance);
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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            serialPort.BaudRate = 115200;
            serialPort.PortName = cbSerialName.SelectedItem.ToString();
            serialPort.Open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            calcResult_sw.Close();
            calcResult_fs.Close();

            keyPoints_sw.Close();
            keyPoints_fs.Close();

            laserData_sw.Close();
            laserData_fs.Close();
        }
     

        private void HandleLaserData()
        {
            if (rplidar.updateFLag == true)
            {
                Point curPoint = new Point();
                //Point obsPoint = new Point();

                curPoint.X = (int)Math.Round(coorX / 0.05, 0) + startPoint.X;
                curPoint.Y = (int)Math.Round(coorY / 0.05, 0) + startPoint.Y;
                for (int i = 0; i < rplidar.obsPoint_arr.Length; i++)
                {
                    //laserData_sw.WriteLine("theta:{0}\tdistance{1}\tangle{2}", rplidar.measureData_arr[i].angle_q6, rplidar.measureData_arr[i].distance_q2, angle);
                    //float xita = angle + rplidar.measureData_arr[i].angle_q6 - (float)Math.PI;
                    //if (rplidar.measureData_arr[i].angle_q6 < (float)Math.PI / 3 || rplidar.measureData_arr[i].angle_q6 > (float)Math.PI / 3 * 5)
                    {
                        //int X = curPoint.X - (int)(rplidar.measureData_arr[i].distance_q2 * Math.Cos(xita) / 50);
                        //int Y = curPoint.Y - (int)(rplidar.measureData_arr[i].distance_q2 * Math.Sin(xita) / 50);
                        //obsPoint.X = X;
                        //obsPoint.Y = Y;
                        UpdateMap(curPoint, rplidar.obsPoint_arr[i]);
                    }
                }
                rplidar.updateFLag = false;
            }
        }
        public void UpdateMap(Point p1, Point p2)
        {
            int size = 0;
            int up = 0;
            int down = 0;
            double result = 0;
            int inflatNum = 4;
           // g = pictureBox1.CreateGraphics();
            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush grayBrush = new SolidBrush(Color.White);
            SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
           // Console.WriteLine("lalalalallal");
            //p1p2垂直于x轴
            if (p1.X == p2.X)
            {
                up = Math.Max(p1.Y, p2.Y);
                down = Math.Min(p1.Y, p2.Y);
                size = up - down;
                for (int step = 1; step < size; step++)
                {
                    if (((down + step) >= 0 && (down + step) < pgmFile.map.height) && ((p1.X) >= 0 && (p1.X) < pgmFile.map.width))
                    {
                        nav.map.mapdata[(down + step) * pgmFile.map.width + p1.X] = 254;
                        pgmFile.map.mapdata[(down + step) * pgmFile.map.width + p1.X] = 254;
                        //g.FillRectangle(grayBrush, p1.X, (down + step), 1, 1);
                    }
               
                }
            }
            //p1p2垂直于y轴
            else if (p1.Y == p2.Y)
            {
                up = Math.Max(p1.X, p2.X);
                down = Math.Min(p1.X, p2.X);
                size = up - down;
                for (int step = 1; step < size; step++)
                {
                    if ((p1.Y >= 0 && p1.Y < pgmFile.map.height) && ((down + step) >= 0 && (down + step) < pgmFile.map.width))
                    {
                        nav.map.mapdata[p1.Y * pgmFile.map.width + down + step] = 254;
                        pgmFile.map.mapdata[p1.Y * pgmFile.map.width + down + step] = 254;
                        // g.FillRectangle(grayBrush, (down + step), p1.Y, 1, 1);
                    }

                }
            }
            else
            {
                Func func = new Func(p1, p2);
                if (func.type == 0)
                {
                    up = Math.Max(p1.X, p2.X);
                    down = Math.Min(p1.X, p2.X);
                    size = up - down;
                    for (int step = 1; step < size; step++)
                    {
                        result = func.CalcResult(down + step);
                        if (((int)Math.Round(result, 0) >= 0 && (int)Math.Round(result, 0) < pgmFile.map.height) && ((down + step) >= 0 && (down + step) < pgmFile.map.width))
                        {
                            nav.map.mapdata[(int)Math.Round(result, 0) * pgmFile.map.width + down + step] = 254;
                            pgmFile.map.mapdata[(int)Math.Round(result, 0) * pgmFile.map.width + down + step] = 254;
                            //g.FillRectangle(grayBrush, (down + step),(int)result , 1, 1);
                        }
                     
                    }
                }
                else
                {
                    up = Math.Max(p1.Y, p2.Y);
                    down = Math.Min(p1.Y, p2.Y);
                    size = up - down;
                    for (int step = 0; step < size; step++)
                    {
                        result = func.CalcResult(down + step);
                        if (((down + step) >= 0 && (down + step) < pgmFile.map.height) && ((int)Math.Round(result, 0) >= 0 && (int)Math.Round(result, 0) < pgmFile.map.width))
                        {
                            nav.map.mapdata[(down + step) * pgmFile.map.width + (int)Math.Round(result, 0)] = 254;
                            pgmFile.map.mapdata[(down + step) * pgmFile.map.width + (int)Math.Round(result, 0)] = 254;
                            //g.FillRectangle(grayBrush, (int)result, (down + step), 1, 1);
                        }
                      
                    }
           
                }
            }
            //g.FillRectangle(redBrush, p2.X - inflatNum, p2.Y - inflatNum, inflatNum * 2, inflatNum * 2);
            g.FillRectangle(redBrush, p2.X, p2.Y, 1, 1);
            int qunima = 2;
            int obsCost = 100;
            for (int i = 0; i < inflatNum * 2 + 1; i++)
            {
                for (int j = 0; j < inflatNum * 2 + 1; j++)
                {
                    if (((p2.Y - inflatNum + i) >= 0 && (p2.Y - inflatNum + i) < pgmFile.map.height) && ((p2.X - inflatNum + j) >= 0 && (p2.X - inflatNum + j) < pgmFile.map.width))
                    {
                        nav.map.mapdata[(p2.Y - inflatNum + i) * pgmFile.map.width + (p2.X - inflatNum + j)] = 0;
                        pgmFile.map.mapdata[(p2.Y - inflatNum + i) * pgmFile.map.width + (p2.X - inflatNum + j)] = 0;

                        for (int k = (p2.Y - inflatNum + i) - qunima; k <= (p2.Y - inflatNum + i) + qunima; k++)
                        {
                            for (int w = -qunima; w <= qunima; w++)
                            {
                                if ((k >= 0 && k < pgmFile.map.height) && ((p2.X - inflatNum + j) >= 0 && (p2.X - inflatNum + j) < pgmFile.map.width))
                                {
                                    if (pgmFile.map.mapdata[k * nav.map.width + (p2.X - inflatNum + j) + w] != 0)
                                    {
                                        pgmFile.costMap[k * nav.map.width + (p2.X - inflatNum + j) + w] = obsCost;
                                        //g.FillRectangle(yellowBrush, (p2.X - inflatNum + j) + w, k, 1, 1);
                                    }
                                }

                            }
                        }
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int curTime = 0;
            int preTime = 0;
            Point sdjfl=new Point(240,340);
            nav = new Navigate(pgmFile.map, startPoint, sdjfl, pgmFile.costMap);

            //rplidar.StartExpressScan();
            rplidar.StartScan();

            while (true)
            {
                curTime = System.DateTime.Now.Millisecond;
                if(rplidar.updateFLag==true)
                {
                    HandleLaserData();


                    preTime = curTime;
                    curTime = System.DateTime.Now.Millisecond;
                    if (curTime < preTime)
                        curTime += 1000;

                    Console.WriteLine("{0}", curTime - preTime);
                }

                //Delay(100);
            }
        }

        void DrawPath(Stack<Point> key)
        {
            int index = 0;
            Point[] pointArr= new Point[key.Count];
            key.CopyTo(pointArr,0);
            p = new Pen(Color.Blue, 1);
            //g = pictureBox1.CreateGraphics();

            Point childPoint = new Point(0, 0);
            Point parentPoint = new Point(0, 0);

            childPoint = pointArr[index];//startPoint
            index++;
            while (index < key.Count)
            {
                
                parentPoint = pointArr[index];
                g2.DrawLine(p, childPoint, parentPoint);
                childPoint = parentPoint;
                index++;
            }                
        }
    }


}
