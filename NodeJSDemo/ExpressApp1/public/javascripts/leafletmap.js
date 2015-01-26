var map, polyData = [],
	polygonLayer;
<!--Leaflet Map API -->
var map = L.map('map-content').setView([31.24, 121.45], 11); // 创建Map实例
var gradient = {
	0.25: "rgb(156,39,176)",
	0.55: "rgb(174,57,146)",
	0.85: "rgb(200,83,99)",
	0.95: "rgb(226,109,55)",
	1.0: "rgb(245,127,23)"
}
var cfg = {
	// radius should be small ONLY if scaleRadius is true (or small radius is intended)
	"radius": .004,
	"maxOpacity": .7,
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

// polygon test
var polyOp = {
	storke: false,
	fill: true,
	fillColor: 'rgb(156,39,176)',
	fillOpacity: 0.35
}


L.tileLayer('https://{s}.tiles.mapbox.com/v3/{id}/{z}/{x}/{y}.png', {
	maxZoom: 15,
	attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, ' +
		'<a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, ' +
		'Imagery © <a href="http://mapbox.com">Mapbox</a>',
	id: 'examples.map-i875mjb7'
}).addTo(map);

heatmapOverlay = new HeatmapOverlay(cfg);
map.addLayer(heatmapOverlay);

var drawHeatmap = function(date) {
	$('.progress').show();
	var postdata = {
		date: date
	}
	$.post("/fetchbydate", postdata, function(responsedata) {
		var heatMapDataPoints = [];
		$(responsedata).each(function(index, val) {
			if (val.geo) {
				var heatMapDataPoint = {
					lng: val.geo.lon,
					lat: val.geo.lat,
					count: 1
				}
				heatMapDataPoints.push(heatMapDataPoint);
			};
		});
		//alert(heatMapDataPoints);
		heatmapOverlay.setData({
			data: heatMapDataPoints,
			max: 10
		});
		$('.progress').hide();
	});
}
var selectArea = function(areaLat, areaLng, areaId, areaZoom) {
	map.setView([areaLat, areaLng], areaZoom, {
		animate: true
	});
	if (areaId == 0) {
		map.removeLayer(polygonLayer);
		polyData = []
	} else {
		if (map.hasLayer(polygonLayer)) {
			polyData = areaData[areaId - 1].geometry.rings[0];
			polygonLayer.setLatLngs(polyData);
			polygonLayer.redraw();
		} else {
			polyData = areaData[areaId - 1].geometry.rings[0];
			polygonLayer = L.polygon(polyData, polyOp);
			map.addLayer(polygonLayer);
		}
	}
}

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

$('.form-control').val('2013-1-1');

$('.form_datetime').datetimepicker().on('changeDate', function(ev) {
	selectArea($('.map-area-list>.active>.area-lat').val(),
		$('.map-area-list>.active>.area-lng').val(),
		$('.map-area-list>.active>.area-id').val(),
		$('.map-area-list>.active>.area-zoom').val());
	drawHeatmap(ev.date);
});

<!--map area item pick-->
$('.map-area-list li').click(function() {
	$('.map-area-list li').removeClass('active');
	$(this).addClass('active');
	var dateSelected = new Date($('.form-control').val());
	selectArea($(this).find('.area-lat').val(),
		$(this).find('.area-lng').val(),
		$(this).find('.area-id').val(),
		$(this).find('.area-zoom').val());
	drawHeatmap(dateSelected);
});

$('.progress').hide();
drawHeatmap(new Date(2013, 0, 1));