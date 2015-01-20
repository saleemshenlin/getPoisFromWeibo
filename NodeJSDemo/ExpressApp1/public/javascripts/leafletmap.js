var map;
<!--Leaflet Map API -->
var map = L.map('map-content').setView([31.24, 121.45], 11);  // 创建Map实例
var gradient={
	0.25: "rgb(156,39,176)", 
	0.55: "rgb(174,57,146)", 
	0.85: "rgb(200,83,99)", 
	0.95: "rgb(226,109,55)", 
	1.0: "rgb(245,127,23)"
}
var cfg = {
	// radius should be small ONLY if scaleRadius is true (or small radius is intended)
	"radius": .005,
	"maxOpacity": .8, 
	// scales the radius based on map zoom
	"scaleRadius": true, 
	// if set to false the heatmap uses the global maximum for colorization
	// if activated: uses the data maximum within the current map boundaries 
	//   (there will always be a red spot with useLocalExtremas true)
	"useLocalExtrema": true,
	// which field name in your data represents the latitude - default "lat"
	latField: 'lat',
	// which field name in your data represents the longitude - default "lng"
	lngField: 'lng',
	// which field name in your data represents the data value - default "value"
	valueField: 'count'
};



L.tileLayer('https://{s}.tiles.mapbox.com/v3/{id}/{z}/{x}/{y}.png', {
	maxZoom: 18,
	attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, ' +
		'<a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
		'Imagery © <a href="http://mapbox.com">Mapbox</a>',
	id: 'examples.map-i875mjb7'
}).addTo(map);

heatmapOverlay = new HeatmapOverlay(cfg);
map.addLayer(heatmapOverlay);


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

$('.form-control').val('2013年一月1日');

$('.form_datetime').datetimepicker().on('changeDate', function(ev){
    //alert(ev.date.valueOf());
    postDate(ev.date);
});


var postDate = function(date){
	// if(map.getOverlays().length>0){
	// 	$(map.getOverlays()).each(function(){
	// 		map.removeOverlay(this);
	// 	});
	// 	map.centerAndZoom(new BMap.Point(121.45, 31.14), 11);
	// }
	var postdata = {
		date : date
	}
	$.post( "/fetchbydate",postdata, function(responsedata) {
		var heatMapDataPoints = [];
		$(responsedata).each(function(index,val){
			var heatMapDataPoint = {
				lng:$(this)[0].geo.lon,
				lat:$(this)[0].geo.lat,
				count:1
			}
			heatMapDataPoints.push(heatMapDataPoint);
		});
		//alert(heatMapDataPoints);
		heatmapOverlay.setData({data:heatMapDataPoints,max:10});
	});
}

postDate(new Date(2013,0,1));


