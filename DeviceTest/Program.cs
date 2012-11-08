using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MyWebRequest req = new MyWebRequest("http://192.168.33.15:8000/devices/", "GET");
            Console.WriteLine(req.GetResponse());
            Console.Read();
        }
    }
}
