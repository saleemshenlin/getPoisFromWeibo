var map,heatmapOverlay;
<!--百度地图API功能-->
map = new BMap.Map("map-content");    // 创建Map实例
var gradient={
	0.55: "rgb(33,150,243)", 
	0.75: "rgb(75,125,213)", 
	0.85: "rgb(134,90,171)", 
	0.95: "rgb(196,52,125)", 
	1.0: "rgb(233,30,99)"
}
heatmapOverlay = new BMapLib.HeatmapOverlay({"radius":15, "visible":true, "opacity":70,"gradient":gradient});
map.centerAndZoom(new BMap.Point(121.45, 31.14), 11);  // 初始化地图,设置中心点坐标和地图级别
map.addControl(new BMap.MapTypeControl());   //添加地图类型控件
map.setCurrentCity("上海");          // 设置地图显示的城市 此项是必须设置的
map.enableScrollWheelZoom(true);     //开启鼠标滚轮缩放]

<!--DateTime pick-->
$('.form_datetime').datetimepicker({
	language:  'zh-CN',
	weekStart: 1,
	todayBtn:  1,
	autoclose: 1,
	todayHighlight: 1,
	startView: 2,
	minView: 2,
	forceParse: 0
});

// if(!isSupportCanvas()){
//    	alert('热力图目前只支持有canvas支持的浏览器,您所使用的浏览器不能使用热力图功能~')
//    }

$.post( "/fetchbydate", function(data) {
	map.removeOverlay(heatmapOverlay);
	map.addOverlay(heatmapOverlay);
	var heatMapDataPoints = [];
	$(data).each(function(){
		var heatMapDataPoint = {
			lng:$(this)[0].geo.lon,
			lat:$(this)[0].geo.lat,
			count:1
		}
		heatMapDataPoints.push(heatMapDataPoint);
	});
	//alert(heatMapDataPoints);
	heatmapOverlay.setDataSet({data:heatMapDataPoints,max:1});
});
