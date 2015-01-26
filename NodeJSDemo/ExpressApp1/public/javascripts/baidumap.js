var map;
<!--百度地图API功能-->
map = new BMap.Map("map-content"); // 创建Map实例
var gradient = {
	0.55: "rgb(156,39,176)",
	0.6: "rgb(174,57,146)",
	0.8: "rgb(200,83,99)",
	0.95: "rgb(226,109,55)",
	1.0: "rgb(245,127,23)"
}

map.centerAndZoom(new BMap.Point(121.45, 31.14), 11); // 初始化地图,设置中心点坐标和地图级别
map.addControl(new BMap.MapTypeControl()); //添加地图类型控件
map.setCurrentCity("上海"); // 设置地图显示的城市 此项是必须设置的
map.enableScrollWheelZoom(true); //开启鼠标滚轮缩放]

<!--DateTime pick-->
$('.form_datetime').datetimepicker({
	language: 'zh-CN',
	weekStart: 1,
	todayBtn: 1,
	autoclose: 1,
	todayHighlight: 1,
	startView: 2,
	minView: 2,
	forceParse: 0
});

$('.form-control').val('2013年一月1日');

$('.form_datetime').datetimepicker().on('changeDate', function(ev) {
	//alert(ev.date.valueOf());
	postDate(ev.date);
});

// if(!isSupportCanvas()){
//    	alert('热力图目前只支持有canvas支持的浏览器,您所使用的浏览器不能使用热力图功能~')
//    }

var postDate = function(date) {
	if (map.getOverlays().length > 0) {
		$(map.getOverlays()).each(function() {
			map.removeOverlay(this);
		});
		map.centerAndZoom(new BMap.Point(121.45, 31.14), 11);
	}
	var postdata = {
		date: date
	}
	$.post("/fetchbydate", postdata, function(responsedata) {
		var heatmapOverlay = new BMapLib.HeatmapOverlay({
			"radius": 18,
			"visible": true,
			"opacity": 85,
			"gradient": gradient
		});
		map.addOverlay(heatmapOverlay);
		var heatMapDataPoints = [];
		$(responsedata).each(function(index, val) {
			// var reqUrl = 'http://api.map.baidu.com/geoconv/v1/?coords='+
			// 	val.geo.lon+','+
			// 	val.geo.lat+'&from=1&to=5&ak=tL14LtUikdTRTyoVuGEIXHbs';
			// $.ajax(reqUrl, {    
			// 	dataType: 'jsonp',  
			// 	crossDomain: true,  
			// 	success: function(data) {  
			// 	  	if(data.status == '0'){
			// 	  		var results = data.result; 
			// 	  		var heatMapDataPoint = {
			// 	  			lng:results[0].x,
			// 	  			lat:results[0].y,
			// 	  			count:1
			// 	  		}
			// 	  		heatMapDataPoints.push(heatMapDataPoint);
			// 	  	}
			// 		if (index === responsedata.length-1) {
			// 			heatmapOverlay.setDataSet({data:heatMapDataPoints,max:10});
			// 		};   
			// 	} 
			// });
			var heatMapDataPoint = {
				lng: $(this)[0].geo.lon,
				lat: $(this)[0].geo.lat,
				count: 1
			}
			heatMapDataPoints.push(heatMapDataPoint);
		});
		//alert(heatMapDataPoints);
		heatmapOverlay.setDataSet({
			data: heatMapDataPoints,
			max: 10
		});
	});
}

postDate(new Date(2013, 0, 1));