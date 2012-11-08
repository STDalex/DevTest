using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DeviceTest
{
    public class MyWebRequest
    {
        private WebRequest request;
        private Stream dataStream;
        private string status;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public MyWebRequest(string url)
        {
            request = WebRequest.Create(url);
        }

        public MyWebRequest(string url, string method)
            : this(url)
        {

            if (method.Equals("GET") || method.Equals("POST"))
            {
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
        }

        public MyWebRequest(string url, string method, string data)
            : this(url, method)
        {

            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

        }

        public string GetResponse()
        {
            // Get the original response.
            WebResponse response = request.GetResponse();

            this.Status = ((HttpWebResponse)response).StatusDescription;

            // Get the stream containing all content returned by the requested server.
            dataStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("windows-1251");
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream, encode);

            // Read the content fully up to the end.
            string responseFromServer = reader.ReadToEnd();
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

    }

}
