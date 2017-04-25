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
        Navigate nav;
        PgmFile pgmFile;
        FileStream keyPoints_fs;
        FileStream calcResult_fs;
        FileStream laserData_fs;

        StreamWriter keyPoints_sw;
        StreamWriter calcResult_sw;
        StreamWriter laserData_sw; 
        
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
            float scaleX = (float)pgmFile.map.width / this.pictureBox1.Width;
            float scaleY = (float)pgmFile.map.height / this.pictureBox1.Height;
            int disX = this.Width - this.pictureBox1.Width;
            int disY = this.Height - this.pictureBox1.Height;
            this.pictureBox1.Width = pgmFile.map.width;
            this.pictureBox1.Height = pgmFile.map.height;
            this.Width = this.pictureBox1.Width + disX;//(int)(this.Width * scaleX);
            this.Height = this.pictureBox1.Height+disY;//(int)(this.Height * scaleY);
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
            rplidar.DataReceived += new SerialDataReceivedEventHandler(Rplidar_DataReceived);
            rplidar.Open();
            StartMotor();
            GetInfo();

            //设置默认起点坐标
            startPoint = new Point(310, 310);

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

                    if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 254)
                        g.FillRectangle(whiteBrush, j, i, 1, 1);

                    else if (pgmFile.map.mapdata[i * this.pictureBox1.Width + j] == 0)
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
                            pgmFile.map.mapdata[j * pgmFile.map.width + k] = 0;
                            //设置代价地图
                            for (int q = j - qunima; q <= j + qunima; q++)
                            {
                                for (int w = -qunima; w <= qunima; w++)
                                {
                                    pgmFile.costMap[q * pgmFile.map.width + k + w] = 100;
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
            g = pictureBox1.CreateGraphics();

            Point childPoint = new Point(0, 0);
            Point parentPoint = new Point(0, 0);

            startPoint = new Point(310, 310);//默认起点
            Navigate tempNav = new Navigate(pgmFile.map, startPoint, goalPoint, pgmFile.costMap);

            if (tempNav.GetPath() == true)
            {
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
                       
                        Console.WriteLine("x:{0}\ty:{1}\tangle:{2}\t", coorX, coorY, angle); 
                        buffer.RemoveRange(0, 22);
                    }
                }
                else
                {
                    buffer.RemoveAt(0);
                }

            }

        }

        struct rplidar_measurement_data
        {
            public float angle_q6;
            public float distance_q2;
        };
        rplidar_measurement_data tempData=new rplidar_measurement_data();
        List<byte> rplidar_buffer = new List<byte>(1024);
        List<rplidar_measurement_data> measurementData = new List<rplidar_measurement_data>(360);
        byte messageType = 0;//0代表起始应答报文，1代表数据应答报文
        byte cmdType = 0;//0x04代表设备信息获取命令，0x81代表开始扫描采样命令
        int majorModel = 0;
        byte subModel = 0;
        byte firmwareVer_minor = 0;
        byte firmwareVer_major = 0;
        byte hardware = 0;
        byte[] serialNumber = new byte[32];
        byte quality = 0;
        UInt16 angle_q6 = 0;
        UInt16 distance_q2 = 0;
        byte[] angle_q6_buf = new byte[2];
        byte[] distance_q2_buf = new byte[2];
        void Rplidar_DataReceived(object obj, SerialDataReceivedEventArgs e)
        {
            int dataSize = 0;
            int length = rplidar.BytesToRead;
            byte[] readBuf = new byte[length];
            rplidar.Read(readBuf, 0, length);
            rplidar_buffer.AddRange(readBuf);

            if (messageType == 0)
            {
                while (rplidar_buffer.Count > 1)
                {
                    //检验帧头
                    if (rplidar_buffer[0] == 0xA5 && rplidar_buffer[1] == 0x5A)
                    {
                        //检查数据长度
                        if (rplidar_buffer.Count >= 7)
                        {
                            messageType = 1;
                            dataSize = rplidar_buffer[2];
                            cmdType = rplidar_buffer[6];
                            rplidar_buffer.RemoveRange(0, 7);
                            if (cmdType == 0x04)
                            {
                                subModel = (byte)(rplidar_buffer[0] & 0x0f);
                                majorModel = (rplidar_buffer[0] & 0xf0) >> 4;
                                Console.WriteLine("Ver:{0}.{1}", majorModel, subModel);
                                firmwareVer_minor = rplidar_buffer[1];
                                firmwareVer_major = rplidar_buffer[2];
                                Console.WriteLine("firmware ver:{0}.{1}", firmwareVer_major, firmwareVer_minor);
                                hardware = rplidar_buffer[3];
                                Console.WriteLine("hardware ver:{0}", hardware);
                                Console.Write("S/N:");
                                for (int i = 0; i < 16; i++)
                                {
                                    serialNumber[i * 2] = (byte)(rplidar_buffer[4 + i] & 0x0f);
                                    serialNumber[i * 2 + 1] = (byte)((rplidar_buffer[4 + i] & 0xf0) >> 4);
                                    Console.Write("{0:X}{1:X}", serialNumber[i * 2 + 1], serialNumber[i * 2]);
                                    messageType = 0;
                                }
                                Console.WriteLine(" ");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        rplidar_buffer.RemoveAt(0);
                    }
                }
            }
            else
            {
                if (cmdType == 0x81)
                {
                    while (rplidar_buffer.Count > 1)
                    {
                        if ((rplidar_buffer[0] & 0x01) + ((rplidar_buffer[0] & 0x02) >> 1) == 1)
                        {
                            if (rplidar_buffer.Count >= 5)
                            {
                                quality = (byte)(rplidar_buffer[0] & 0xfc);
                                if (quality != 0)
                                {
                                    distance_q2_buf[0] = rplidar_buffer[3];
                                    distance_q2_buf[1] = rplidar_buffer[4];
                                    distance_q2 = BitConverter.ToUInt16(distance_q2_buf, 0);
                                    if(distance_q2!=0)
                                    {
                                        angle_q6_buf[0] = rplidar_buffer[1];
                                        angle_q6_buf[1] = rplidar_buffer[2];
                                        angle_q6 = (UInt16)(BitConverter.ToUInt16(angle_q6_buf, 0) >> 1);

                                        tempData.angle_q6 = angle_q6 / 64.0f / 360 * 2 * (float)Math.PI;
                                        tempData.distance_q2 = distance_q2 / 4.0f;
                                        if (measurementData.Count < 360)
                                            measurementData.Add(tempData);
                                        else
                                        {
                                            measurementData.RemoveRange(0, measurementData.Count);
                                        }
                                        Console.WriteLine("theta:{0}\tdistance:{1}\tquality:{2}", angle_q6 / 64.0f, distance_q2 / 4.0f, quality);                                      
                                    }
                                }
                                rplidar_buffer.RemoveRange(0, 5);

                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            rplidar_buffer.RemoveAt(0);
                        }
                    }
                }
            }

        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
           // startPoint = new Point(310, 310);//默认起点
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            Point curPoint = new Point(0, 0);
            Point tarPoint = new Point(0, 0);
           // double interval = 0.01;
            int vel = 200;//速度200mm/s
            int omiga = 0;
            //double deltaX = 0.0;
            //double deltaY = 0.0;
            double orientation = 0.0;
            double errAngle = 0.0;
            double resolution = 0.05;
            byte[] sendBuff = new byte[4];
            string str;

            p = new Pen(Color.Blue, 1);
            g = pictureBox1.CreateGraphics();
            if (rplidar.IsOpen == true)
            {
                StartScan();
            }
            else
            {
                MessageBox.Show("激光雷达串口没打开");
                return;
            }
            nav = new Navigate(pgmFile.map, startPoint, goalPoint, pgmFile.costMap);
            if(nav.GetPath()==true)
            {
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
                        calcResult_sw.WriteLine("angle={0}\torientation={1}\tomiga={2}\ttime={3}", angle, orientation, omiga,System.DateTime.Now.Millisecond.ToString());
                       
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
                        Delay(100);
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
                                keyPoints = nav.keyPoints;
                                p1 = keyPoints.Pop();
                                tarPoint = keyPoints.Pop();
                            }
                            else
                            {
                                MessageBox.Show("error!");
                            }
                        }
                        else
                        {
                            p1 = p2;
                        }
                    }
                    curPoint = tarPoint;
                    //g.DrawLine(p, childPoint, parentPoint);                
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
        private void StartMotor()
        {
            byte[] buff = new byte[6];
            buff[0] = 0xA5;
            buff[1] = 0xF0;
            buff[2] = 0x02;
            buff[3] = 0x94;
            buff[4] = 0x02;
            buff[5] = 0xc1;
            rplidar.Write(buff, 0, 6);
        }

        private void StartScan()
        {
            byte[] buff = new byte[2];
            buff[0] = 0xA5;
            buff[1] = 0x20;
            rplidar.Write(buff, 0, 2);
        }

        private void GetInfo()
        {
            byte[] buff = new byte[2];
            buff[0] = 0xA5;
            buff[1] = 0x50;
            rplidar.Write(buff, 0, 2);
        }

        private void StopMotor()
        {
            byte[] buff = new byte[6];
            buff[0] = 0xA5;
            buff[1] = 0xF0;
            buff[2] = 0x02;
            buff[3] = 0x00;
            buff[4] = 0x00;
            buff[5] = 0x57;
            rplidar.Write(buff, 0, 6);
        }

        private void StopScan()
        {
            byte[] buff = new byte[2];
            buff[0] = 0xA5;
            buff[1] = 0x25;
            rplidar.Write(buff, 0, 2);
        }

        private void HandleLaserData()
        {
            Point curPoint = new Point();
            Point obsPoint = new Point();

            curPoint.X = (int)Math.Round(coorX / 0.05, 0) + startPoint.X;
            curPoint.Y = (int)Math.Round(coorY / 0.05, 0) + startPoint.Y;
            for (int i = 0; i < measurementData.Count; i++)
            {
                laserData_sw.WriteLine("theta:{0}\tdistance{1}", measurementData[i].angle_q6, measurementData[i].distance_q2);
                float xita = angle + measurementData[i].angle_q6 - (float)Math.PI;
                int X = curPoint.X - (int)(measurementData[i].distance_q2 * Math.Cos(xita) / 50);
                int Y = curPoint.Y - (int)(measurementData[i].distance_q2 * Math.Sin(xita) / 50);
                obsPoint.X = X;
                obsPoint.Y = Y;
                UpdateMap(curPoint, obsPoint);
            }
        }
        public void UpdateMap(Point p1, Point p2)
        {
            int size = 0;
            int up = 0;
            int down = 0;
            double result = 0;
            int inflatNum = 3;
           // g = pictureBox1.CreateGraphics();
            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush grayBrush = new SolidBrush(Color.White);

           // Console.WriteLine("lalalalallal");
            //p1p2垂直于x轴
            if (p1.X == p2.X)
            {
                up = Math.Max(p1.Y, p2.Y);
                down = Math.Min(p1.Y, p2.Y);
                size = up - down;
                for (int step = 1; step < size; step++)
                {
                    nav.map.mapdata[(down + step) * pgmFile.map.width + p1.X] = 254;
                    pgmFile.map.mapdata[(down + step) * pgmFile.map.width + p1.X] = 254;
                    g.FillRectangle(grayBrush, p1.X, (down + step), 1, 1);
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
                    nav.map.mapdata[p1.Y * pgmFile.map.width + down + step] = 254;
                    pgmFile.map.mapdata[p1.Y * pgmFile.map.width + down + step] = 254;
                    g.FillRectangle(grayBrush, (down + step), p1.Y, 1, 1);
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
                        nav.map.mapdata[(int)(result) * pgmFile.map.width + down + step] = 254;
                        pgmFile.map.mapdata[(int)(result) * pgmFile.map.width + down + step] = 254;
                        g.FillRectangle(grayBrush, (down + step),(int)result , 1, 1);
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
                        nav.map.mapdata[(down + step) * pgmFile.map.width + (int)result] = 254;
                        pgmFile.map.mapdata[(down + step) * pgmFile.map.width + (int)result] = 254;
                        g.FillRectangle(grayBrush, (int)result, (down + step), 1, 1);
                    }
           
                }
            }
            g.FillRectangle(redBrush, p2.X - inflatNum, p2.Y - inflatNum, inflatNum * 2, inflatNum * 2);
            for (int i = 0; i < inflatNum * 2 + 1; i++)
            {
                for (int j = 0; j < inflatNum * 2 + 1; j++)
                {
                    nav.map.mapdata[(p2.Y - inflatNum + i) * pgmFile.map.width + (p2.X - inflatNum + j)] = 0;
                    pgmFile.map.mapdata[(p2.Y - inflatNum + i) * pgmFile.map.width + (p2.X - inflatNum + j)] = 0;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int curTime = 0;
            int preTime = 0;
            Point sdjfl=new Point(240,340);
            nav = new Navigate(pgmFile.map, startPoint, sdjfl, pgmFile.costMap);
            if (rplidar.IsOpen == true)
            {

                StartScan();
            }
            else
            {
                MessageBox.Show("激光雷达串口没打开");
                return;
            }
            while(true)
            {
                curTime = System.DateTime.Now.Millisecond;
                HandleLaserData();
                preTime = curTime;
                curTime = System.DateTime.Now.Millisecond;
                if (curTime < preTime)
                    curTime += 1000;

                Console.WriteLine("{0}", curTime-preTime);
                Delay(100);
            }
        }
    }


}
