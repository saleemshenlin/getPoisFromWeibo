getPoisFromWeibo
================

从新浪微博API，根据关键词按地址位置获取POI点的信息

使用微博SDK for .Net 2.0/3.5/4.0（OAuth2.0+官方V2版API），由网友 林选臣 提供

http://weibosdk.codeplex.com/

调用以下api，获取pois
location/pois/search/by_location	根据关键词按地址位置获取POI点的信息

SINA.GetCommand("place/nearby/pois",
  new WeiboParameter("lat", lat),
  new WeiboParameter("long", lng),
  new WeiboParameter("range", 5000),
  new WeiboParameter("q", ""),
  new WeiboParameter("category", ""),
  new WeiboParameter("count", 50),
  new WeiboParameter("page", page),
  new WeiboParameter("sort", 0),
  new WeiboParameter("offset", 0));
  
根据新浪协议，测试授权：
总限制：单用户每应用 150次/小时

针对 150次/小时 请求，还需要修改bug

已经修复bug，当遇到用户请求上限，程序暂停10min，在运行
