using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BinHeap;
namespace Astar
{
    class navigate
    {
        public MAP map;
        public int[] parentList;
        public Point startPoint;
        private Point goalPoint;
        private TWD[] twds=new TWD[8];
        private int[] costMap;
        public Stack<Point> keyPoints;
          
        public navigate(MAP _map,Point _startPoint,Point _goalPoint,int [] _test)
        {
            map = _map;
            parentList = new int[map.width*map.height];
            startPoint = _startPoint;
            goalPoint = _goalPoint;
            costMap = _test;
            keyPoints = new Stack<Point>();
            twds[0].x = 0; twds[0].y = -1;    twds[0].cost=10  ;  //上
            twds[1].x = 0; twds[1].y = 1;     twds[1].cost=10  ;  //下
            twds[2].x = -1; twds[2].y = 0;    twds[2].cost=10  ;  //左
            twds[3].x = 1; twds[3].y = 0;     twds[3].cost=10  ;  //右
            twds[4].x = -1; twds[4].y = -1;   twds[4].cost=14  ;  //左上
            twds[5].x = 1; twds[5].y = -1;    twds[5].cost=14  ; //右上
            twds[6].x = -1; twds[6].y = 1;    twds[6].cost=14  ; //左下
            twds[7].x = 1; twds[7].y = 1;     twds[7].cost=14  ; //右下
        }
        public bool GetPath()
        {
            
            int row=0,col=0;
            BinHeap.BinHeap open= new BinHeap.BinHeap(map.width*map.height);
            BinHeap.NodeStruct startNode = new BinHeap.NodeStruct();
            BinHeap.NodeStruct popNode;

            startNode.point = startPoint;
            startNode.hValue = CalcDist(startNode.point, goalPoint);
            startNode.gValue = 0;
            startNode.fValue=startNode.hValue+startNode.gValue;
            open.InsertItem(startNode);



            int[] closed = new int[map.width*map.height];
            for (int i = 0; i < map.width * map.height; i++)
                closed[i] = 0;
            while(open.Size!=0)
            {
                popNode = open.PopMinItem();
                //Console.WriteLine("{0}...{1}...{2}", popNode.point.X, popNode.point.Y, popNode.hValue);
                closed[popNode.point.Y * map.width + popNode.point.X] = 1;
                for (int i = 0; i < 8; i++)
                {
                        
                    row = popNode.point.X + twds[i].x;
                    col = popNode.point.Y + twds[i].y;
                    //在地图内部
                    if (row >= 0 && row <map.width && col >= 0 && col <map.height)
                    {
                        //不是障碍物且不在closed列表里
                        if (closed[col * map.width + row] != 1 && map.mapdata[col * map.width + row] == 254 )
                        {
                            if(row!=goalPoint.X||col!=goalPoint.Y)
                            {
                                BinHeap.NodeStruct tmpNode = new BinHeap.NodeStruct();                                 
                                tmpNode.point.X = row;
                                tmpNode.point.Y = col;
                                tmpNode.hValue = CalcDist(tmpNode.point, goalPoint);
                                tmpNode.gValue = popNode.gValue + twds[i].cost + costMap[col * map.width + row];
                                tmpNode.fValue = tmpNode.hValue + tmpNode.gValue;
                              
                                //如果节点已经在open列表中，比较之前的fvalue和新的fvalue
                                int index=open.IsExist(tmpNode);
                                if (index!=0)
                                {                                       
                                    if (open.items[index].fValue > tmpNode.fValue)
                                    {
                                        open.items[index].fValue = tmpNode.fValue;
                                        //改变tmpNode的父节点索引
                                        parentList[tmpNode.point.Y * map.width + tmpNode.point.X] = popNode.point.Y * map.width + popNode.point.X;
                                    }
                                }
                                else
                                {
                                    //添加tmpNode的父节点索引
                                    parentList[tmpNode.point.Y * map.width + tmpNode.point.X] = popNode.point.Y * map.width + popNode.point.X;
                                    open.InsertItem(tmpNode);

                                }
                            }
                            else
                            {
                                parentList[col * map.width + row] = popNode.point.Y * map.width + popNode.point.X;
                                SmoothPath();
                                return true;     
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void SmoothPath()
        {
            Point parentPoint = new Point(0, 0);
            Point childPoint = new Point(0, 0);
            Point tempPoint = new Point(0, 0);

            int parentIndex = goalPoint.Y * map.width + goalPoint.X;
            childPoint.X = goalPoint.X;
            childPoint.Y = goalPoint.Y;
            keyPoints.Push(childPoint);
            while (true)
            {
                parentPoint.Y = parentList[parentIndex] / map.width;
                parentPoint.X = parentList[parentIndex] % map.width;
                if (IsExistObs(parentPoint, childPoint) == true)
                {
                    childPoint = tempPoint;
                    //添加关键点
                    keyPoints.Push(childPoint);
                    if (parentPoint == startPoint)
                    {
                        keyPoints.Push(parentPoint);
                        return;
                    }
                    tempPoint = parentPoint;
                    parentIndex = parentPoint.Y * map.width + parentPoint.X;
                }
                else
                {
                    if (parentPoint == startPoint)
                    {
                        keyPoints.Push(parentPoint);
                        return;
                    }
                    tempPoint = parentPoint;     
                    parentIndex = parentPoint.Y * map.width + parentPoint.X;
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
        public bool IsExistObs(Point p1, Point p2)
        {
            int size = 0;
            int up = 0;
            int down = 0;
            double result = 0;
            //p1p2垂直于x轴
            if (p1.X == p2.X)
            {
                up = Math.Max(p1.Y, p2.Y);
                down = Math.Min(p1.Y, p2.Y);
                size = up - down;
                for (int step = 0; step < size; step++)
                {
                    if (map.mapdata[(down + step) * map.width + p1.X] != 254)
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
                    if (map.mapdata[p1.Y * map.width + down + step] != 254)
                    {
                        return true;
                    }
                }
                return false;
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
                        if (map.mapdata[(int)(result) * map.width + down + step] != 254)
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
                        if (map.mapdata[(down + step) * map.width + (int)result] != 254)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
    }

     public struct TWD
    {
        public int x, y;
        public int cost;
    }
    public struct MAP
    {
        public int width,height;
        public byte[] mapdata;
        public byte[] type;
    }
    public class Func
    {
        public double k;
        public double b;
        public int type;//为0代表横向迭代，即给定x求y,为1代表纵向迭代，即给定y求x
        public Func(Point A, Point B)
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
