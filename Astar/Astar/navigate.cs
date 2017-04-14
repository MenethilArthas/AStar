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
        private MAP map;
        public int[] parentList;
        private Point startPoint;
        private Point goalPoint;
        private TWD[] twds=new TWD[8];
          
        public navigate(MAP _map,Point _startPoint,Point _goalPoint)
        {
            map = _map;
            parentList = new int[map.width*map.height];
            startPoint = _startPoint;
            goalPoint = _goalPoint;
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
                                return true;     
                            }
                        }
                    }
                }
            }
            return false;
        }
        public int CalcDist(Point start, Point goal)
        {
            
            int distX = (int)(Math.Abs(start.X - goal.X)*10);
            int distY = (int)(Math.Abs(start.Y - goal.Y)*10);
            return (distX + distY);
            //return (10*(distX+distY)+(14-20)*Math.Min(distX,distY));
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
}
