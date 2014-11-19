using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractWeiboData
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
        public static string dateNow = DateTime.Now.ToString("yyyy_MM_dd");
        public static string timeNow = DateTime.Now.ToString("_hh_mm_ss");
        public static List<WeiboData> listHasData = new List<WeiboData>();
        public static List<WeiboData> listNullData = new List<WeiboData>();
        public static void Run(){
            //streamWriter = new StreamWriter(@"../../" + "/log" + "//log_" + dateNow + timeNow + ".txt", false);
            listHasData.Capacity = 1000000;
            listNullData.Capacity = 50000;
            DirectoryInfo df = new DirectoryInfo(@"../../input/");
            foreach (FileInfo file in df.GetFiles())
            {
               ExtractWeiboData(file.Name);
               //streamWriter.WriteLine(file.Name.ToString() + " : finish!!!!");
            }
            //JArray jaHas = (JArray)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listHasData));
            //JArray jaNull = (JArray)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(listNullData));
            File.WriteAllText(@"../../output/listNullData.json", JsonConvert.SerializeObject(listHasData));
            File.WriteAllText(@"../../output/listHasData.json", JsonConvert.SerializeObject(listNullData));
            //streamWriter.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
            //streamWriter.Close();
            Console.WriteLine(DateTime.Now.ToLocalTime().ToString() + " : finish!!!!");
        }

        public static void ExtractWeiboData(string filename)
        {
            JArray jaInput = new JArray();
            jaInput = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(@"../../input/" + filename));
            foreach (JObject jo in jaInput)
            {
                if (jo["deleted"] == null)
                {
                    WeiboData weiboTemp = new WeiboData();
                    //微博信息
                    weiboTemp.Id = jo["id"].ToString();
                    weiboTemp.CreatedAt = jo["created_at"].ToString();
                    weiboTemp.Text = jo["text"].ToString();
                    if(jo["pic_ids"].ToArray().Count() != 0 ){
                        //weiboTemp.PicIds = jo["pic_ids"].ToString();
                        weiboTemp.PicCount = jo["pic_ids"].ToArray().Count().ToString();
                    }
                    else
                    {
                        //weiboTemp.PicIds = "null";
                        weiboTemp.PicCount = "0";
                    }
                    if (jo["geo"].Children().Count() != 0)
                    {
                        weiboTemp.Lat = jo["geo"]["coordinates"].ToArray()[0].ToString();
                        weiboTemp.Lng = jo["geo"]["coordinates"].ToArray()[1].ToString();
                    }
                    else
                    {
                        weiboTemp.Lat = "null";
                        weiboTemp.Lng = "null";
                    }
                    //微博用户信息   
                    weiboTemp.UserId = jo["user"]["id"].ToString();
                    weiboTemp.UserProvince = jo["user"]["province"].ToString();
                    weiboTemp.UserCity = jo["user"]["city"].ToString();
                    //微博签到信息
                    if (jo["annotations"] != null)
                    {
                        weiboTemp.AnnotationsId = jo["annotations"].ToArray()[0]["place"]["poiid"].ToString();
                        weiboTemp.AnnotationsTitle = jo["annotations"].ToArray()[0]["place"]["title"] != null ? jo["annotations"].ToArray()[0]["place"]["title"].ToString():"null";
                        if (jo["annotations"].ToArray()[0]["place"]["lat"] != null)
                        {
                            weiboTemp.AnnotationsLat = jo["annotations"].ToArray()[0]["place"]["lat"].ToString();
                            weiboTemp.AnnotationsLng = jo["annotations"].ToArray()[0]["place"]["lon"].ToString();
                        }
                        else if (jo["annotations"].ToArray()[0]["place"]["latitude"] != null)
                        {
                            weiboTemp.AnnotationsLat = jo["annotations"].ToArray()[0]["place"]["latitude"].ToString();
                            weiboTemp.AnnotationsLng = jo["annotations"].ToArray()[0]["place"]["longitude"].ToString();
                        }
                        else
                        {
                            weiboTemp.AnnotationsLat = "null";
                            weiboTemp.AnnotationsLng = "null";
                        }
                    }
                    listHasData.Add(weiboTemp);
                    Console.WriteLine("listHasData:" + listHasData.Count);
                }
                else
                {
                    WeiboData weiboTemp = new WeiboData();
                    //微博信息
                    weiboTemp.Id = jo["id"].ToString();
                    weiboTemp.CreatedAt = jo["created_at"].ToString();
                    //微博签到信息
                    if (jo["annotations"] != null)
                    {
                        weiboTemp.AnnotationsId = jo["annotations"].ToArray()[0]["place"]["poiid"].ToString();
                        weiboTemp.AnnotationsTitle = jo["annotations"].ToArray()[0]["place"]["title"] != null ? jo["annotations"].ToArray()[0]["place"]["title"].ToString() : "null";
                        if (jo["annotations"].ToArray()[0]["place"]["lat"] != null)
                        {
                            weiboTemp.AnnotationsLat = jo["annotations"].ToArray()[0]["place"]["lat"].ToString();
                            weiboTemp.AnnotationsLng = jo["annotations"].ToArray()[0]["place"]["lon"].ToString();
                        }
                        else if (jo["annotations"].ToArray()[0]["place"]["latitude"] != null)
                        {
                            weiboTemp.AnnotationsLat = jo["annotations"].ToArray()[0]["place"]["latitude"].ToString();
                            weiboTemp.AnnotationsLng = jo["annotations"].ToArray()[0]["place"]["longitude"].ToString();
                        }
                        else
                        {
                            weiboTemp.AnnotationsLat = "null";
                            weiboTemp.AnnotationsLng = "null";
                        }
                    }
                    listNullData.Add(weiboTemp);
                    Console.WriteLine("listNullData:" + listNullData.Count);
                    jaInput = null;
                }
            }

        }
    }
}
