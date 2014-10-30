using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NetDimension.Json;
using NetDimension.Json.Linq;
using NetDimension.Weibo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GetDataWithAZure
{ 
    class Program
    {
        private static NamespaceManager namespaceManager;
        private static QueueClient queueClient;
        private static String queueName = "weiboqueue";
        static String connectionString = @"Endpoint=sb://azuretest-ns.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3HzV1a5bcs/OOj0YS+Iec7XPA9ys2s9oqrUrg8pcILA=";

        public static OAuth OAUTH;
        public static string WEIBO_NAME = "saleemshenlin@gmail.com";
        public static string PASSWORD = "1qaz2wsx";
        public static string APPKEY = "1654060665"; //1654060665 1663244227 
        public static string APPSECRET = "9b337ffd48099b2b94eecb568b65d1dc"; //9b337ffd48099b2b94eecb568b65d1dc 5cedafd36f790630c49775d7e56e741a
        public static string ACCESSTOKEN = "2.00cGubtBpjQwnB65f4b0d1ccTNVbME"; // 2.00cGubtBnnnYoB2bfed4dc640UADXc
        public static Client SINA = null;
        public static StreamWriter streamWriter = null;

        public static List<string> nullPoiList = new List<string>();
        
        static void Main(string[] args)
        {
            try
            {
                if (args.Count() != 0)
                {

                    if (args[0].ToLower().CompareTo("createqueue") == 0)
                    {
                        // No processing to occur other than creating the queue.
                        namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                        namespaceManager.CreateQueue(queueName);
                        Console.WriteLine("Queue named {0} was created.", queueName);
                        Environment.Exit(0);
                    }

                    if (args[0].ToLower().CompareTo("deletequeue") == 0)
                    {
                        // No processing to occur other than deleting the queue.
                        namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
                        namespaceManager.DeleteQueue(queueName);
                        Console.WriteLine("Queue named {0} was deleted.", queueName);
                        Environment.Exit(0);
                    }
                }
                queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
                
                Run();
                Console.WriteLine("Final solution found!");
                queueClient.Send(new BrokeredMessage("Complete"));

                queueClient.Close();
                Environment.Exit(0);
            }
            catch (ServerBusyException serverBusyException)
            {
                Console.WriteLine("ServerBusyException encountered");
                Console.WriteLine(serverBusyException.Message);
                Console.WriteLine(serverBusyException.StackTrace);
                Environment.Exit(-1);
            }
            catch (ServerErrorException serverErrorException)
            {
                Console.WriteLine("ServerErrorException encountered");
                Console.WriteLine(serverErrorException.Message);
                Console.WriteLine(serverErrorException.StackTrace);
                Environment.Exit(-1);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception encountered");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
                Environment.Exit(-1);
            }
        }

        internal static void Run()
        {
            string dateNow = DateTime.Now.ToString("yyyy_MM_dd");
            string timeNow = DateTime.Now.ToString("_hh_mm_ss");
            streamWriter = new StreamWriter(@"log" + "//log_" + dateNow + timeNow + ".txt", false);
            InitWeiboOAuth(APPKEY, APPSECRET, ACCESSTOKEN);
            JArray jaInput = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(@"input/mergeresult_final.json"));
            int fileCount = GetFileCountFromDir(@"output/");
            int count = 0;
            foreach (JObject jo in jaInput)
            {
                count++;
                if (count >= 76)
                {
                    if (count <= 500)
                    {
                        string poiId = jo["poiid"].ToString();
                        GetPoiTimelineToJson(poiId);
                        Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : " + poiId + " Poi Count: " + count);
                        queueClient.Send(new BrokeredMessage(DateTime.Now.ToLocalTime().ToString() + " : " + poiId + " Poi Count: " + count));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (nullPoiList.Count > 0)
            {
                Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : nullPoiList Count: " + count);
                queueClient.Send(new BrokeredMessage(DateTime.Now.ToLocalTime().ToString() + " : nullPoiList Count: " + count));
                foreach (string poiId in nullPoiList)
                {
                    Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : nullPoiList" + poiId);
                    queueClient.Send(new BrokeredMessage(DateTime.Now.ToLocalTime().ToString() + " :nullPoiList " + poiId));
                    streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : nullPoiList" + poiId);
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
        private static void InitWeiboOAuth(string APPKEY, string APPSECRET, string ACCESSTOKEN)
        {
            OAUTH = new OAuth(APPKEY, APPSECRET, accessToken: ACCESSTOKEN);
            if (OAUTH.VerifierAccessToken() == TokenResult.Success)//验证AccessToken是否有效
            {
                SINA = new NetDimension.Weibo.Client(OAUTH);
                Console.WriteLine(OAUTH.AccessToken); //还是来打印下AccessToken看看与前面方式获取的是不是一样的
                streamWriter.WriteLine(OAUTH.AccessToken);
            }
        }

        private static void GetPoiTimelineToJson(string poiId)
        {

            double totalNumber = 0;
            double itemsNumber = 50;
            string jsonResult = GetPoiTimeLine(poiId, 1);
            if (!jsonResult.Equals("[]"))
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(jsonResult);
                if (jo["total_number"] != null)
                {
                    totalNumber = int.Parse(jo["total_number"].ToString());
                    JArray ja = (JArray)JsonConvert.DeserializeObject(jo["statuses"].ToString());
                    if (totalNumber > 50)
                    {
                        double pageCount = Math.Ceiling(totalNumber / itemsNumber);
                        for (int i = 2; i <= pageCount; i++)
                        {
                            string jsonMore = GetPoiTimeLine(poiId, i);
                            JObject jsonMoreO = (JObject)JsonConvert.DeserializeObject(jsonMore);
                            JArray jaMore = (JArray)JsonConvert.DeserializeObject(jsonMoreO["statuses"].ToString());
                            MergeJArray(ja, jaMore);
                            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " poiid: " + poiId + " pages:" + pageCount + " page:" + i);
                            queueClient.Send(new BrokeredMessage(DateTime.Now.ToLocalTime().ToString() + " pages:" + pageCount + " page:" + i));
                        }
                    }
                    File.WriteAllText(@"output" + "//" + poiId + ".json", ja.ToString());
                    Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : " + poiId + " " + ja.Count);
                    queueClient.Send(new BrokeredMessage(DateTime.Now.ToLocalTime().ToString() + " : " + poiId + " " + ja.Count));
                    streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : " + poiId + " " + ja.Count);
                }
            }
            else
            {
                nullPoiList.Add(poiId);
            }
            
        }

        private static string GetPoiTimeLine(string poiid, int page)
        {
            dynamic json = SINA.GetCommand("place/poi_timeline",
                new WeiboParameter("poiid", poiid),
                new WeiboParameter("count", 50),
                new WeiboParameter("page", page),
                new WeiboParameter("base_app", 0));
            return json;
        }

        public static JArray MergeJArray(JArray arr1, JArray arr2)
        {
            if (arr2.Count > 0)
            {
                foreach (JObject jo in arr2)
                {
                    arr1.Add(jo);
                }
            }
            return arr1;
        }

        public static int GetFileCountFromDir(string dir){
            int result = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            result = dirInfo.GetFiles().Length;
            return result;
        }
    }
    
}
