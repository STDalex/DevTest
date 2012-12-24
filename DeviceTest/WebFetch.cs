using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace DeviceTest
{
    public class WebFetch
    {
        private string m_url;
              
        public WebFetch(string url)
        {
            m_url = url;
        }

        public void UploadFile(string uploadUrl, string fileToUpload)
        {
            WebClient wc = new WebClient();
            wc.Headers["Content-type"] = "text/xml";
            try
            {
                wc.UploadFile(uploadUrl, "POST", fileToUpload);
                Console.WriteLine("Config file is upload to "+uploadUrl+" successfully");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Problem with upload config file to "+uploadUrl+" " + exc.Message);
            }
            wc.Dispose();
        }

        public void GetResponse(string url)
        {
            try
            {
                //DataSet ds = new DataSet();
                WebClient client = new WebClient();
                client.DownloadFile(url,"resp.xml");
                //ds.ReadXml("resp.xml");
                client.Dispose();
            }
            catch (WebException webEx)
            {
                Console.WriteLine(webEx.ToString());
                if (webEx.Status == WebExceptionStatus.ConnectFailure)
                {
                    Console.WriteLine("Are you behind a firewall?  If so, go through the proxy server.");
                }
            }
        }

        public System.Collections.ArrayList GetDevicesURL()
        {
          //  DataSet ds = new DataSet();
          //  ds.ReadXml("resp.xml");
            DataSet ds = DataSet();
            if (ds != null)
            {
                System.Collections.ArrayList reference = new System.Collections.ArrayList();
                foreach (DataTable dt in ds.Tables)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string[] dev = new string[2];
                        dev[0] = dt.TableName;
                        dev[1] = dt.Rows[i]["Адрес"].ToString();
                        reference.Add(dev);
                    }
                }
                return reference;
            }
            else
                return null;
        }

        public Tuple<string,string,double,double> GetDemValues(string[] device)
        {
            //DataSet ds = new DataSet();
            //ds.ReadXml("resp.xml");
            DataSet ds = DataSet();
            if (ds != null)
            {
                string name = device[0];
                string address = device[1];
                string dSync = "";
                double dInfRate = 0;
                double EbN0 = 0;
                try
                {
                    DataTable dt = ds.Tables[name];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["Адрес"].ToString() == address)
                        {
                            dSync = dt.Rows[i]["Синхронизация демодулятора, декодера, УКС"].ToString();
                            dInfRate = Convert.ToDouble(dt.Rows[i]["Инф. скорость, кбит/с"].ToString());
                            EbN0 = Convert.ToDouble(dt.Rows[i]["Eb/No, дБ"].ToString().Replace(".", ","));
                            break;
                        }
                    }
                    name = name + "/" + address;
                    var device_values = Tuple.Create(name, dSync, dInfRate, EbN0);
                    return device_values;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return null;
                }
            }
            else
                return null;
        }

        private DataSet DataSet()
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml("resp.xml");
                return ds;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }

        private void ParseTable(DataTable dt)
        {
            Console.WriteLine(dt.TableName);
            ParseRows(dt);
            foreach (DataRelation dr in dt.ChildRelations)
            {
                ParseTable(dr.ChildTable);
            }
        }

        private void ParseRows(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    Console.Write(dc.ColumnName.ToString() + "=");
                    Console.WriteLine(dt.Rows[i][dc.ColumnName].ToString() + " ");
                }
                Console.WriteLine();
            }
        }


    }

}
