using NetDimension.Json;
using NetDimension.Json.Linq;
using NetDimension.Weibo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace GetPoisFromWeibo
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
        public static string APPKEY_3 = "3745111922"; //1654060665 1663244227 
        public static string APPSECRET_3 = "8bb53cfe8d623fb1001d7c990b88f168"; //9b337ffd48099b2b94eecb568b65d1dc 5cedafd36f790630c49775d7e56e741a
        public static string ACCESSTOKEN_3 = "2.00cGubtBAhG9FEcf2f744601CkyoTB"; //2.00cGubtBpjQwnB65f4b0d1ccTNVbME 2.00cGubtBnnnYoB2bfed4dc640UADXc
        public static string RETURNURL = "http://weibo.com/saleemshenlin";
        public static OAuth OAUTH;
        public static string WEIBO_NAME = "saleemshenlin@gmail.com";
        public static string PASSWORD = "1qaz2wsx";
        public static string APPKEY_2 = "1654060665"; //1654060665 1663244227 
        public static string APPSECRET_2 = "9b337ffd48099b2b94eecb568b65d1dc"; //9b337ffd48099b2b94eecb568b65d1dc 5cedafd36f790630c49775d7e56e741a
        public static string ACCESSTOKEN_2 = "2.00cGubtBpjQwnB65f4b0d1ccTNVbME"; // 2.00cGubtBnnnYoB2bfed4dc640UADXc
        public static string APPKEY_1 = "1663244227"; //1654060665 1663244227 
        public static string APPSECRET_1 = "5cedafd36f790630c49775d7e56e741a"; //9b337ffd48099b2b94eecb568b65d1dc 5cedafd36f790630c49775d7e56e741a
        public static string ACCESSTOKEN_1 = "2.00cGubtBnnnYoB2bfed4dc640UADXc"; // 2.00cGubtBnnnYoB2bfed4dc640UADXc
        public static Client SINA = null;
        public static StreamWriter streamWriter = null;
        public static void Run()
        {
            string dateNow = DateTime.Now.ToString("yyyy_MM_dd");
            string timeNow = DateTime.Now.ToString("_hh_mm_ss");
            //streamWriter = new StreamWriter(@"../../" + "/log" + "//log_" + dateNow + timeNow + ".txt", false);
            InitWeiboOAuth(APPKEY_2, APPSECRET_2, ACCESSTOKEN_2);
            GetPoisCategory();
            //streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
            //streamWriter.Close();
            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
        }
        /// <summary>
        /// 初始化Weibo
        /// </summary>
        /// <returns></returns>
        private static void InitWeiboOAuth(string APPKEY, string APPSECRET, string ACCESSTOKEN)
        {
            OAUTH = new OAuth(APPKEY, APPSECRET, accessToken: ACCESSTOKEN);
            if (OAUTH.VerifierAccessToken() == TokenResult.Success)//验证AccessToken是否有效
            {
                SINA = new NetDimension.Weibo.Client(OAUTH);
                Console.WriteLine(OAUTH.AccessToken); //还是来打印下AccessToken看看与前面方式获取的是不是一样的
                //streamWriter.WriteLine(OAUTH.AccessToken);
            }
        }

        private static void GetPoisFromWeibo(string lng, string lat, string scenicname)
        {
            List<Scenic> scenicList = new List<Scenic>();
            double totalNumber = 0;
            double itemsNumber = 50;
            try
            {
                string jsonResult = GetNearByPoisComm(float.Parse(lng), float.Parse(lat), 1);
                JObject jo = (JObject)JsonConvert.DeserializeObject(jsonResult);
                if (jo["total_number"] != null)
                {
                    totalNumber = int.Parse(jo["total_number"].ToString());
                    string pois = jo["pois"].ToString();
                    scenicList = JsonConvert.DeserializeObject<List<Scenic>>(pois);
                    if (totalNumber > 50)
                    {
                        double pageCount = Math.Ceiling(totalNumber / itemsNumber);
                        for (int i = 2; i <= pageCount; i++)
                        {
                            string jsonMore = GetNearByPoisComm(float.Parse(lng), float.Parse(lat), i);
                            JObject jsonMoreO = (JObject)JsonConvert.DeserializeObject(jsonMore);
                            string poisMore = jsonMoreO["pois"].ToString();
                            IList<Scenic> scenicListMore = JsonConvert.DeserializeObject<List<Scenic>>(pois);
                            scenicList.AddRange(scenicListMore);
                        }
                    }
                    File.WriteAllText(@"../../" + "/output" + "//" + scenicname + ".json", JsonConvert.SerializeObject(scenicList));
                    Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : " + scenicname + " " + scenicList.Count);
                    streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : " + scenicname + " " + scenicList.Count);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("用户请求超过上限"))
                {
                    Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": 程序暂停..." + scenicname);
                    System.Threading.Thread.Sleep(600000);
                    Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": 程序继续..." + scenicname);
                    GetPoisFromWeibo(lng, lat, scenicname);
                }
                else
                {
                    Console.WriteLine("Error " + DateTime.Now.ToLocalTime().ToString() + ":" + scenicname + ex.Message.ToString());
                    streamWriter.WriteLine("Error " + DateTime.Now.ToLocalTime().ToString() + ":" + scenicname + ex.Message.ToString());
                }
            }
        }


        private static string GetNearByPoisComm(float lng, float lat, int page)
        {
            dynamic json = SINA.GetCommand("place/nearby/pois",
                new WeiboParameter("lat", lat),
                new WeiboParameter("long", lng),
                new WeiboParameter("range", 2000),
                new WeiboParameter("q", ""),
                new WeiboParameter("category", ""),
                new WeiboParameter("count", 50),
                new WeiboParameter("page", page),
                new WeiboParameter("sort", 0),
                new WeiboParameter("offset", 0));
            return json;
        }

        private static List<Scenic> ReadXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"../../" + "input/data_with_location_2.xml");
            XmlNode scenicNode = xmlDoc.SelectSingleNode("scenic");
            XmlNodeList items = scenicNode.ChildNodes;
            List<Scenic> scenicList = new List<Scenic>();
            foreach (XmlNode item in items)
            {

                XmlNode scenicNameNode = item.SelectSingleNode("scenicname");
                string scenicName = scenicNameNode != null ? scenicNameNode.InnerText : "null";
                XmlNode scenicLngNode = item.SelectSingleNode("lng");
                string scenicLng = scenicLngNode != null ? scenicLngNode.InnerText : "null";
                XmlNode scenicLatNode = item.SelectSingleNode("lat");
                string scenicLat = scenicLatNode != null ? scenicLatNode.InnerText : "null";
                XmlNode scenicComeNode = item.SelectSingleNode("from");
                string scenicComeFrom = scenicComeNode != null ? scenicComeNode.InnerText : "null";
                Scenic scenic = new Scenic();
                scenic.Title = scenicName;
                scenic.Lat = scenicLat;
                scenic.Lng = scenicLng;
                scenicList.Add(scenic);
            }
            Console.WriteLine("data_with_location_2: " + scenicList.Count);
            streamWriter.WriteLine("data_with_location_2: " + scenicList.Count);
            return scenicList;
        }

        private static void ValidateData()
        {
            List<Scenic> scenicList = ReadXml();
            List<string> fileList = new List<string>();
            DirectoryInfo rootDir = new DirectoryInfo(@"../../" + "output");
            foreach (FileInfo file in rootDir.GetFiles("*.*"))
            {
                string[] sArray = Regex.Split(file.ToString(), ".json", RegexOptions.IgnoreCase);
                fileList.Add(sArray[0]);
            }

            if (scenicList.Count > 0)
            {
                foreach (string fileName in fileList)
                {

                    //int count = 0;
                    foreach (Scenic scenic in scenicList)
                    {
                        if (fileName == scenic.Title)
                        {
                            scenicList.Remove(scenicList.Where(c => c.Title == fileName).FirstOrDefault());
                            break;
                        }
                    }
                    //if (count != 0)
                    //{
                    //    //streamWriter.WriteLine("ScenicTrue: " + DateTime.Now.ToLocalTime().ToString() + " ; " + scenic.Title + " ; " + scenic.Lat + " ; " + scenic.Lng);
                    //    streamWriter.WriteLine(count);
                    //    Console.WriteLine("ScenicTrue: " + DateTime.Now.ToLocalTime().ToString() + " ; " + scenic.Title + " ; " + scenic.Lat + " ; " + scenic.Lng);
                    //}
                    //else
                    //{
                    //    streamWriter.WriteLine("Scenic: " + DateTime.Now.ToLocalTime().ToString() + " ; " + scenic.Title + " ; " + scenic.Lat + " ; " + scenic.Lng);
                    //    Console.WriteLine("Scenic: " + DateTime.Now.ToLocalTime().ToString() + " ; " + scenic.Title + " ; " + scenic.Lat + " ; " + scenic.Lng);
                    //}

                }
            }
            foreach (Scenic scenic in scenicList)
            {
                streamWriter.WriteLine("ScenicTrue: " + DateTime.Now.ToLocalTime().ToString() + " ; " + "\" , \"" + scenic.Lng + "\" , \"" + scenic.Lat + "\" , \"" + scenic.Title + "\"");
            }
        }

        private static void GetPoisCategory()
        {
            dynamic json = SINA.GetCommand("place/pois/category",
                new WeiboParameter("pid", "0"),
                new WeiboParameter("flag", "1"));
            JArray ja = (JArray)JsonConvert.DeserializeObject(json);
            //File.WriteAllText(@"../../" + "/output/poi_category.json", ja.ToString());
        }
    }
}
