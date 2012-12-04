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
    public class Tester
    {
        private string m_config;
        private DataSet m_ds_config;
        

        public Tester(string config)
        {
            try
            {
                m_config = config;
                m_ds_config = new DataSet();
                m_ds_config.ReadXml(config);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        
        public string[] GetTestDevices()
        {
            DataTable tb = m_ds_config.Tables["devices"];
            string[] devices = new string[tb.Rows.Count];
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                devices[i] = tb.Rows[i]["Name"].ToString();
            }
            return devices;
        }

        public string GetDeviceUploadFile()
        {
            return m_ds_config.Tables["Options"].Rows[0]["НастройкаДемодулятора"].ToString();
        }

        public string GetUVGSuploadFile()
        {
            return m_ds_config.Tables["Options"].Rows[0]["НастройкаУВГС"].ToString();
        }


        public void DeviceTest(Tuple<string,string,double,double> data)
        {
            DataTable tb = m_ds_config.Tables["expected_values"];

            string dName = data.Item1;
            string dSync = data.Item2;
            double dInfRate = data.Item3;
            double dEbN0 = data.Item4;

            string expected_Sync = tb.Rows[0]["Синхронизация_демодулятора_декодера_УКС"].ToString();
            double expected_InfSpeed_min = Convert.ToDouble(tb.Rows[0]["Инф_скорость_кбит_с_min"].ToString());
            double expected_InfSpeed_max = Convert.ToDouble(tb.Rows[0]["Инф_скорость_кбит_с_max"].ToString());
            double expected_EbN0_min = Convert.ToDouble(tb.Rows[0]["Eb_N0_min"].ToString());
            double expected_EbN0_max = Convert.ToDouble(tb.Rows[0]["Eb_N0_max"].ToString());
            int expected_ARU_min = Convert.ToInt32(tb.Rows[0]["АРУ_min"].ToString());
            int expected_ARU_max = Convert.ToInt32(tb.Rows[0]["АРУ_max"].ToString());
            int expected_DARU_min = Convert.ToInt32(tb.Rows[0]["ЦАРУ_min"].ToString());
            int expected_DARU_max = Convert.ToInt32(tb.Rows[0]["ЦАРУ_max"].ToString());

            if (IsInfSpeedOk(dName, dInfRate, expected_InfSpeed_min, expected_InfSpeed_max) &
                IsEbN0Ok(dName, dEbN0, expected_EbN0_min, expected_EbN0_max) &
                IsSyncOk(dName, dSync, expected_Sync))
                Console.WriteLine(dName + " is OK!!!");
        }

        #region tests
        private bool IsSyncOk(string dName, string dSync, string expected_Sync)
        {
            if (dSync == expected_Sync)
                return true;
            else
            {
                msg(dName, "синхронизация демодулятора, декодера и УКС", dSync, expected_Sync);
                return false;
            }
        }

        private bool IsInfSpeedOk(string dName, double dInfRate, double expected_InfSpeed_min, double expected_InfSpeed_max)
        {
            if ((dInfRate <= expected_InfSpeed_max) && (dInfRate >= expected_InfSpeed_min))
                return true;
            else
            {
                msg(dName, "Информационная скорость (кбит/с)", dInfRate.ToString(), "between " + expected_InfSpeed_min.ToString() + " and " + expected_InfSpeed_max.ToString());
                return false;
            }
        }

        private bool IsEbN0Ok(string dName, double dEbN0, double expected_EbN0_min, double expected_EbN0_max)
        {
            if ((dEbN0 <= expected_EbN0_max) && (dEbN0 >= expected_EbN0_min))
                return true;
            else
            {
                msg(dName, "Соотношение сигнал/шум (дБ)", dEbN0.ToString(), "between " + expected_EbN0_min.ToString() + " and " + expected_EbN0_max.ToString());
                return false;
            }
        }

        private bool IsARUOk(string dName, int dARU, int expected_ARU_min, int expected_ARU_max)
        {
            if ((dARU <= expected_ARU_max) && (dARU >= expected_ARU_min))
                return true;
            else
            {
                msg(dName, "АРУ", dARU.ToString(), "between " + expected_ARU_min.ToString() + " and " + expected_ARU_max.ToString());
                return false;
            }
        }

        private bool IsDARUOk(string dName, int dDARU, int expected_DARU_min, int expected_DARU_max)
        {
            if ((dDARU <= expected_DARU_max) && (dDARU >= expected_DARU_min))
                return true;
            else
            {
                msg(dName, "АРУ", dDARU.ToString(), "between " + expected_DARU_min.ToString() + " and " + expected_DARU_max.ToString());
                return false;
            }
        }

        private void msg(string dName, string test_name, string dvalue, string expected_value)
        {
            Console.WriteLine("WARRNING!!! "+dName+" has the defect: "+test_name+". It is "+dvalue+", but should be "+expected_value);
        }
        #endregion

    }
}
