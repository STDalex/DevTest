using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Collections;

namespace DeviceTest
{
    public class Tester
    {
        private string m_config;
        private DataSet m_ds_config;
        private string m_dem_test_report;
        private string m_result_report;

        public Tester(string config)
        {
            m_config = config;
            m_ds_config = new DataSet();
            m_ds_config.ReadXml(config);
            m_dem_test_report = "";
            m_result_report = tests_name_tabel();
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

        public string GetDeviceUploadFile(string path)
        {
            return path + "\\" + m_ds_config.Tables["Options"].Rows[0]["НастройкаДемодулятора"].ToString();
        }

        public Hashtable GetUVGSuploadFile()
        {
            Hashtable result = new Hashtable();
            DataTable dt = m_ds_config.Tables["НастройкаУВГС"];
            for (int i = 0; i < dt.Rows.Count; i++)
                result.Add(dt.Rows[i]["Key"].ToString(), dt.Rows[i]["Value"].ToString());
            return result;
        }


        public void DeviceTest(Tuple<string, string, double, double, int, int> data)
        {
            DataTable tb = m_ds_config.Tables["expected_values"];

            string dName = data.Item1;
            string dSync = data.Item2;
            double dInfRate = data.Item3;
            double dEbN0 = data.Item4;
            int dAru = data.Item5;
            int dDaru = data.Item6;

            string expected_Sync = tb.Rows[0]["Синхронизация_демодулятора_декодера_УКС"].ToString();
            double expected_InfSpeed_min = Convert.ToDouble(tb.Rows[0]["Инф_скорость_кбит_с_min"].ToString());
            double expected_InfSpeed_max = Convert.ToDouble(tb.Rows[0]["Инф_скорость_кбит_с_max"].ToString());
            double expected_EbN0_min = Convert.ToDouble(tb.Rows[0]["Eb_N0_min"].ToString());
            double expected_EbN0_max = Convert.ToDouble(tb.Rows[0]["Eb_N0_max"].ToString());
            int expected_ARU_min = Convert.ToInt32(tb.Rows[0]["АРУ_min"].ToString());
            int expected_ARU_max = Convert.ToInt32(tb.Rows[0]["АРУ_max"].ToString());
            int expected_DARU_min = Convert.ToInt32(tb.Rows[0]["ЦАРУ_min"].ToString());
            int expected_DARU_max = Convert.ToInt32(tb.Rows[0]["ЦАРУ_max"].ToString());

            if (IsSyncOk(dName, dSync, expected_Sync) &
                IsInfSpeedOk(dName, dInfRate, expected_InfSpeed_min, expected_InfSpeed_max) &
                IsEbN0Ok(dName, dEbN0, expected_EbN0_min, expected_EbN0_max) &
                IsARUOk(dName, dAru, expected_ARU_min, expected_ARU_max) &
                IsDARUOk(dName, dDaru, expected_DARU_min, expected_DARU_max))
            {
                Console.WriteLine(dName + " is OK!!!");
            }

            string dem_test_row = m_dem_test_report;
            m_result_report += demodulator_test_result(dName, dem_test_row);
            m_dem_test_report = "";
            
            
        }

        #region tests

        const string Sync_str = "синхронизация демодулятора, декодера и УКС";
        const string InfRate_str = "Информационная скорость (кбит/с)";
        const string SNR_str = "Соотношение сигнал/шум (дБ)";
        const string ARU_str = "АРУ";
        const string DARU_str = "ЦАРУ";

        private bool IsSyncOk(string dName, string dSync, string expected_Sync)
        {
            bool test_result;
            if (dSync == expected_Sync)
                test_result = true;
            else
            {
                msg(dName, Sync_str, dSync, expected_Sync);
                test_result = false;
            }
            m_dem_test_report += test_result_row(test_result, dSync);
            return test_result;
        }

        private bool IsInfSpeedOk(string dName, double dInfRate, double expected_InfSpeed_min, double expected_InfSpeed_max)
        {
            bool test_result;
            if ((dInfRate <= expected_InfSpeed_max) && (dInfRate >= expected_InfSpeed_min))
                test_result = true;
            else
            {
                msg(dName, InfRate_str, dInfRate.ToString(), "between " + expected_InfSpeed_min.ToString() + " and " + expected_InfSpeed_max.ToString());
                test_result = false;
            }
            m_dem_test_report += test_result_row(test_result, dInfRate.ToString());
            return test_result;
        }

        private bool IsEbN0Ok(string dName, double dEbN0, double expected_EbN0_min, double expected_EbN0_max)
        {
            bool test_result;
            if ((dEbN0 <= expected_EbN0_max) && (dEbN0 >= expected_EbN0_min))
                test_result = true;
            else
            {
                msg(dName, SNR_str, dEbN0.ToString(), "between " + expected_EbN0_min.ToString() + " and " + expected_EbN0_max.ToString());
                test_result = false;
            }
            m_dem_test_report += test_result_row(test_result, dEbN0.ToString());
            return test_result;
        }

        private bool IsARUOk(string dName, int dARU, int expected_ARU_min, int expected_ARU_max)
        {
            bool test_result;
            if ((dARU <= expected_ARU_max) && (dARU >= expected_ARU_min))
                test_result = true;
            else
            {
                msg(dName, ARU_str, dARU.ToString(), "between " + expected_ARU_min.ToString() + " and " + expected_ARU_max.ToString());
                test_result = false;
            }
            m_dem_test_report += test_result_row(test_result, dARU.ToString());
            return test_result;
        }

        private bool IsDARUOk(string dName, int dDARU, int expected_DARU_min, int expected_DARU_max)
        {
            bool test_result;
            if ((dDARU <= expected_DARU_max) && (dDARU >= expected_DARU_min))
                test_result = true;
            else
            {
                msg(dName, DARU_str, dDARU.ToString(), "between " + expected_DARU_min.ToString() + " and " + expected_DARU_max.ToString());
                test_result = false;
            }
            m_dem_test_report += test_result_row(test_result, dDARU.ToString());
            return test_result;
        }

        private void msg(string dName, string test_name, string dvalue, string expected_value)
        {
            Console.WriteLine("\nWARRNING!!! "+dName+" has the defect: "+test_name+". It is "+dvalue+", but should be "+expected_value+"\n");
        }

        #endregion

        #region html_report

        private string tests_name_tabel()
        {
            string name_table="";
            ArrayList test_names = new ArrayList();
            test_names.Add("  Устройство  ");
            test_names.Add(Sync_str);
            test_names.Add(InfRate_str);
            test_names.Add(SNR_str);
            test_names.Add(ARU_str);
            test_names.Add(DARU_str);

            name_table += "<tr bgcolor = \"#AAAAAA\">";
            foreach (string test_name in test_names)
                name_table += "<td><strong>" + test_name + "</strong></td>";
            name_table += "</tr>";
            return name_table;
        }

        private string test_result_row(bool isTestPass, string value)
        {
            string color = isTestPass ? "#CEE2D3" : "Red";
            string test_result = "<td valign=\"top\", bgcolor=\"" + color + "\">" + value + "</td>";
            return test_result;
        }

        private string demodulator_test_result(string demodulator_name, string tests)
        {
            string result = "<tr bgcolor = \"#DDDDDD\"><td valign=\"top\">" + demodulator_name + "</td>" + tests + "</tr>";
            return result;
        }

        public string TestResultHTML()
        {
            string result = "";
            result = "<table><tbody><tr><td colspan=6><h3>" + m_config + "</h3></td></tr>" + m_result_report + "</tbody></table><br><br>";
            return result;
        }

        #endregion

    }
}
