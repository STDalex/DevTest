using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DeviceTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string base_url = "http://192.168.33.15:8000/devices/";
            WebFetch req = new WebFetch(base_url);
            Tester tester = new Tester("test_tpc_1100_2000.xml");

            req.GetResponse(base_url);
            ArrayList device_list = req.GetDevicesURL();

            string[] device_types = tester.GetTestDevices();
            string upload_dem_file = tester.GetDeviceUploadFile();

            foreach (string device_type in device_types)
            {
                foreach (string[] device in device_list)
                {
                    if (device_type == device[0])
                        req.UploadFile((base_url + device[0] + "/" + device[1] + "/"), upload_dem_file);
                }
            }

            System.Threading.Thread.Sleep(5000);

            req.GetResponse(base_url);

            foreach (string device_type in device_types)
            {
                foreach (string[] device in device_list)
                {
                    if (device_type == device[0])
                        tester.DeviceTest(req.GetDemValues(device));
                }
            }
            
                   
            
            Console.Read();

        }
    }

    class Config
    {
       
    }
}
