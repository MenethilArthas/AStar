using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Astar
{
    class PgmFile
    {
        private string path;
        public MAP map;
        public int[] costMap;
        private int obsCost=200;
        public PgmFile(string filePath)
        {
            path = filePath;
        }
        public void Read_FileData()
        {
            
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs, Encoding.UTF7);
                string str = string.Empty;
                str = sr.ReadLine();//p5
                str = sr.ReadLine();//comment context
                str = sr.ReadLine();//width height
                string[] size = str.Split(' ');
                map.width = Convert.ToInt32(size[0])-300;
                map.height = Convert.ToInt32(size[1])-350;

        

                costMap = new int[(map.width) * (map.height)];
                
                str = sr.ReadLine();//255

                str = sr.ReadLine();
                //str = sr.ReadLine();
                map.mapdata = new byte[(map.width ) * (map.height )];
                map.type = new byte[(map.width ) * (map.height )];
                for (int i = 200; i < 200+map.height; i++)
                {
                    for (int j = 100; j < 100+map.width; j++)
                    {
                        map.mapdata[(i-200) * (map.width) + (j-100)] = (byte)str[i * (map.width+300)+ j];
                        if (map.mapdata[(i - 200) * (map.width ) + (j - 100)] == 0)
                        {
                            map.type[(i - 200) * (map.width ) + (j - 100)] = 1;
                            //int qunima = 5;
                            //for (int k = i - qunima; k <= i + qunima; k++)
                            //{
                            //    for (int w = -qunima; w <= qunima; w++)
                            //    {
                            //        costMap[k * map.width + j + w] = obsCost;
                            //    }
                            //}
                        }

                        else
                            map.type[(i - 200) * (map.width ) + (j - 100)] = 0;
                    }
                }
            }
        }
    }
}
