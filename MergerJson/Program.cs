using NetDimension.Json;
using NetDimension.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergerJson
{
    class Program
    {
        static void Main(string[] args)
        {
            Work.Run();
            Console.Read();
        }
    }

    class Work
    {
        public static StreamWriter streamWriter = null;
        public static void Run()
        {
            string dateNow = DateTime.Now.ToString("yyyy_MM_dd");
            string timeNow = DateTime.Now.ToString("_hh_mm_ss");
            streamWriter = new StreamWriter(@"../../" + "/log" + "//log_" + dateNow + timeNow + ".txt", false);
            MergeData();
            streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
            streamWriter.Close();
            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
        }


        /// <summary>
        /// 数据整合
        /// </summary>
        private static void MergeData()
        {
            DirectoryInfo rootDir = new DirectoryInfo(@"../../" + "input");
            JArray jaAll = new JArray();
            List<string> poiIdList = new List<string>();
            try
            {
                //遍历文件
                foreach (FileInfo file in rootDir.GetFiles("*.*"))
                {
                    string readText = File.ReadAllText(file.Directory + "\\" + file.Name);
                    JArray ja = (JArray)JsonConvert.DeserializeObject(readText);
                    foreach (JObject joTemp in ja)
                    {
                        //Scenic scenic = JsonConvert.DeserializeObject<Scenic>(jo.ToString());
                        if (jaAll.Count == 0)
                        {
                            jaAll.Add(joTemp);
                            poiIdList.Add(joTemp["poiid"].ToString());
                        }
                        else
                        {
                            if(!poiIdList.Contains(joTemp["poiid"].ToString()))
                            {
                                jaAll.Add(joTemp);
                                poiIdList.Add(joTemp["poiid"].ToString());
                            }
                        }
                    }
                    streamWriter.WriteLine("File: " + DateTime.Now.ToLocalTime().ToString() + " ; " + file.ToString() + " ; " + ja.Count);
                    Console.WriteLine("File: " + DateTime.Now.ToLocalTime().ToString() + " ; " + file.ToString() + " ; " + ja.Count);
                }
                File.WriteAllText(@"../../" + "/output/result.json", jaAll.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + DateTime.Now.ToLocalTime().ToString() + " ; " + ex.Message.ToString());
                streamWriter.WriteLine("Error :" + DateTime.Now.ToLocalTime().ToString() + " ; " + ex.Message.ToString());
            }
        }
    }

}
