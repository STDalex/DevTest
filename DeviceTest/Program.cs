using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TCPserverClassLibrary;
using System.Data;

namespace DeviceTest
{
    class Program
    {
        static void Main(string[] args)
        {
           
            try
            {
                DataSet ds_config = new DataSet();
                ds_config.ReadXml("config.xml");
                DataTable dt = ds_config.Tables["config"];
                string base_url = "http://" + dt.Rows[0]["aps_socket"].ToString() + "/devices/";
                
                ArrayList test_files = new ArrayList();
                for (int i = 0; i < ds_config.Tables["test_file"].Rows.Count; i++)
                    test_files.Add(dt.Rows[0]["test_files_folder"].ToString() + "\\" + ds_config.Tables["test_file"].Rows[i]["file_name"].ToString());

                WebFetch web_fetch = new WebFetch(base_url);
                TCPClient uvgs = new TCPClient(System.Net.IPAddress.Parse(dt.Rows[0]["uvgs_ip"].ToString()), Convert.ToInt32(dt.Rows[0]["uvgs_port"].ToString()));

                foreach (string f in test_files)
                {
                    Tester tester = new Tester(f);
                    Hashtable uvgs_settings = tester.GetUVGSuploadFile();
                    Console.WriteLine("Start test : "+ f);
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("stop"));
                    foreach (DictionaryEntry de in uvgs_settings)
                    {
                        uvgs.DataWrite(Encoding.UTF8.GetBytes("UVGS<" + de.Key + ">-<" + de.Value + ">"));
                        System.Threading.Thread.Sleep(1000);
                        Console.WriteLine(de.Key + " " + de.Value);
                    }
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("start"));
                    Console.WriteLine("UVGS start");
                    Console.WriteLine("Start tests... ");

                    web_fetch.GetResponse(base_url);
                    string[] device_types = tester.GetTestDevices();
                    string upload_dem_file = tester.GetDeviceUploadFile(dt.Rows[0]["test_files_folder"].ToString());
                    ArrayList device_list = web_fetch.GetDevicesURL();
                    foreach (string device_type in device_types)
                    {
                        foreach (string[] device in device_list)
                        {
                            if (device_type == device[0])
                                web_fetch.UploadFile((base_url + device[0] + "/" + device[1] + "/"), upload_dem_file);
                        }
                    }

                    Console.WriteLine("\nUpload configuration files is complete. Tests started, please wait...\n");
                    System.Threading.Thread.Sleep(1000);

                    web_fetch.GetResponse(base_url);

                    foreach (string device_type in device_types)
                    {
                        foreach (string[] device in device_list)
                        {
                            if (device_type == device[0])
                                tester.DeviceTest(web_fetch.GetDemValues(device));
                        }
                    }

                    Console.WriteLine("Tests are finished!");
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("stop"));
                    Console.WriteLine("UVGS stop");
                }
                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

           

          //  TCPClient client = new TCPClient((new System.Net.IPAddress(new byte[4] { 192, 168, 33, 15 })), 28300);
         //   client.DataWrite(Encoding.UTF8.GetBytes("UVGS<Формат файла>-<7>"));
          //  client.DataWrite(Encoding.UTF8.GetBytes("start"));
            
            //client.DataWrite(Encoding.UTF8.GetBytes("stop"));            
        //   client.Disconect();
          /*  
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
           
            Console.WriteLine("\nUpload configuration files is complete. Tests started, please wait...\n");
            System.Threading.Thread.Sleep(1000);

            req.GetResponse(base_url);

            foreach (string device_type in device_types)
            {
                foreach (string[] device in device_list)
                {
                    if (device_type == device[0])
                        tester.DeviceTest(req.GetDemValues(device));
                }
            }
            
            */       
           
            Console.ReadLine();

        }
    }
    
}
