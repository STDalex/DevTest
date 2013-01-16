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
                string report_file = "reports\\Report_"+ DateTime.Now.TimeOfDay.ToString().Replace(":",".") + ".html";
                string result_reports_html = "";

                ArrayList test_files = new ArrayList();
                for (int i = 0; i < ds_config.Tables["test_file"].Rows.Count; i++)
                    test_files.Add(dt.Rows[0]["test_files_folder"].ToString() + "\\" + ds_config.Tables["test_file"].Rows[i]["file_name"].ToString());

                WebFetch web_fetch = new WebFetch(base_url);
                TCPClient uvgs = new TCPClient(System.Net.IPAddress.Parse(dt.Rows[0]["uvgs_ip"].ToString()), Convert.ToInt32(dt.Rows[0]["uvgs_port"].ToString()));

                foreach (string f in test_files)
                {
                    Tester tester = new Tester(f);
                    Hashtable uvgs_settings = tester.GetUVGSuploadFile();
                    Console.WriteLine("\nStart test : " + f);
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("stop"));
                    System.Threading.Thread.Sleep(1000);
                    foreach (DictionaryEntry de in uvgs_settings)
                    {
                        uvgs.DataWrite(Encoding.UTF8.GetBytes("UVGS<" + de.Key + ">-<" + de.Value + ">"));
                       // System.Threading.Thread.Sleep(1000);
                      //  Console.WriteLine(de.Key + " " + de.Value);
                    }
                    System.Threading.Thread.Sleep(1000);
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("start"));

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

                    Console.WriteLine("Upload configuration files is complete. Tests started, please wait...\n");
                    System.Threading.Thread.Sleep(10000);

                    web_fetch.GetResponse(base_url);
                    string reports_html = "";
                    foreach (string device_type in device_types)
                    {
                        foreach (string[] device in device_list)
                        {
                            reports_html = "";
                            if (device_type == device[0])
                            {
                                tester.DeviceTest(web_fetch.GetDemValues(device));
                            }
                        }
                        
                    }
                    reports_html += tester.TestResultHTML();
                    result_reports_html += reports_html;
                    Console.WriteLine("Tests are finished!");
                    uvgs.DataWrite(Encoding.UTF8.GetBytes("stop"));
                    Console.WriteLine("UVGS stop");
                }

                System.IO.FileStream fs = new System.IO.FileStream(report_file, System.IO.FileMode.CreateNew);
                string html_report = "<html><head><title>" + report_file + "</title><meta http-equiv=Content-Type content=\"text/html; charset=windows-1251\"></head><body style=\"font-family:Verdana; font-size:14pt\">" + result_reports_html + "</body></html>";
                byte[] buffer = Encoding.GetEncoding(1251).GetBytes(html_report);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                Console.WriteLine("File with report was created " + report_file);
                System.Diagnostics.Process.Start(report_file);
                Console.WriteLine("All tests are finished!");
   
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            Console.ReadLine();

        }

        
    }

}
