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
            ValidatePoiType();
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
                            if (!poiIdList.Contains(joTemp["poiid"].ToString()))
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

        private static void ValidatePoiType()
        {
            int[] typeArray = new int[]{
                33,254,20,156,219,
                45,252,52,671,678,
                116,179,180,182,183,
                184,185,186,187,188,
                189,195,196,197,198,
                199,200,201,202,203,
                204,205,206,207,208,
                220,221,222,223,224,
                225,226,227,228,229,
                230,231,232,233,234,
                235,236,237,238,239,
                240,243,244,245,246,
                250,604,607,677,627,
                628
            };
            List<int> typeList = new List<int>(typeArray);
            JArray ja = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(@"../../output/mergeresult.json"));
            JArray jaTypeTrue = new JArray();
            JArray jaTypeFalse = new JArray();
            JArray jaTypeNull = new JArray();
            foreach(JObject jo in ja){
                if (typeList.Contains(Int32.Parse(jo["category"].ToString())))
                {
                    jaTypeTrue.Add(jo);
                    Console.WriteLine("TypeTrue :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                    streamWriter.WriteLine("TypeTrue :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                }
                else if (Int32.Parse(jo["category"].ToString())==500)
                {
                    jaTypeNull.Add(jo);
                    Console.WriteLine("TypeNull :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                    streamWriter.WriteLine("TypeNull :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                
                }
                else
                {
                    jaTypeFalse.Add(jo);
                    Console.WriteLine("TypeFalse :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                    streamWriter.WriteLine("TypeFalse :" + DateTime.Now.ToLocalTime().ToString() + ";" + jo["title"].ToString() + ";" + jo["poiid"].ToString());
                }
            }
            Console.WriteLine("TypeFalseCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeFalse.Count.ToString());
            streamWriter.WriteLine("TypeFalseCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeFalse.Count.ToString());
            Console.WriteLine("TypeTrueCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeTrue.Count.ToString());
            streamWriter.WriteLine("TypeTrueCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeTrue.Count.ToString());
            Console.WriteLine("TypeNullCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeNull.Count.ToString());
            streamWriter.WriteLine("TypeNullCount:" + DateTime.Now.ToLocalTime().ToString() + ";" + jaTypeNull.Count.ToString());
            File.WriteAllText(@"../../" + "/output/typefalse.json", jaTypeFalse.ToString());
            File.WriteAllText(@"../../" + "/output/typetrue.json", jaTypeTrue.ToString());
            File.WriteAllText(@"../../" + "/output/typenull.json", jaTypeNull.ToString());
        }

    }

}
