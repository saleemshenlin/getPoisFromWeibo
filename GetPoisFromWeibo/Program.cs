using NetDimension.Json;
using NetDimension.Json.Linq;
using NetDimension.Weibo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public static string APPKEY = "1663244227";
        public static string APPSECRET = "5cedafd36f790630c49775d7e56e741a";
        public static string RETURNURL = "https://api.weibo.com/oauth2/default.html";
        public static OAuth OAUTH;
        public static string WEIBO_NAME = "saleemshenlin@gmail.com";
        public static string PASSWORD = "1qaz2wsx";
        public static string ACCESSTOKEN = "2.00cGubtBnnnYoB2bfed4dc640UADXc";
        public static Client SINA = null;
        public static StreamWriter streamWriter = null;
        public static void Run()
        {
            string dateNow = DateTime.Now.ToString("yyyy_MM_dd");
            string timeNow = DateTime.Now.ToString("_hh_mm_ss");
            streamWriter = new StreamWriter(@"../../" + "/log" + "//log_" + dateNow + timeNow + ".txt", false);
            InitWeiboOAuth();
            List<Scenic> scenicList = ReadXml();
            int count = 0;
            if (scenicList.Count > 0)
            {
                foreach (Scenic scenic in scenicList)
                {
                    count++;
                    //if (count > 800)
                    //{
                    //    break;
                    //} 
                    if (count >= 828)
                    {
                        GetPoisFromWeibo(scenic.Lng, scenic.Lat, scenic.Title);
                    }
                }
            }
            streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
            streamWriter.Close();
            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
        }
        /// <summary>
        /// 初始化Weibo
        /// </summary>
        /// <returns></returns>
        private static void InitWeiboOAuth()
        {
            OAUTH = new OAuth(APPKEY, APPSECRET, RETURNURL, ACCESSTOKEN);
            if (OAUTH.VerifierAccessToken() == TokenResult.Success)//验证AccessToken是否有效
            {
                SINA = new NetDimension.Weibo.Client(OAUTH);
                Console.WriteLine(OAUTH.AccessToken); //还是来打印下AccessToken看看与前面方式获取的是不是一样的
                streamWriter.WriteLine(OAUTH.AccessToken);
            }
            else
            {
                OAUTH = new OAuth(APPKEY, APPSECRET, RETURNURL);
                bool result = OAUTH.ClientLogin(WEIBO_NAME, PASSWORD);
                if (result) //返回true授权成功
                {
                    SINA = new NetDimension.Weibo.Client(OAUTH);
                    Console.WriteLine(OAUTH.AccessToken); //还是来打印下AccessToken看看与前面方式获取的是不是一样的
                    streamWriter.WriteLine(OAUTH.AccessToken);
                }
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
    }
}
