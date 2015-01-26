//count group by annotation.poiid
db.weibo.group({
	key: {
		'annotations.poiid': true,
	},
	initial: {
		counts: 0,
	},
	reduce: function(doc, argument) {
		argument.counts = argument.counts + 1;
		argument.title = doc.annotations.title;
		argument.lng = doc.annotations.lon;
		argument.lat = doc.annotations.lat;
	}
});
//insert index for created_at
db.weibo.ensureIndex({
	'created_at': 1
}, {
	unique: true,
	dropDups: true
});
//insert index for annotation.poiid
db.weibo.ensureIndex({
	'created_at': 1
}, {
	unique: true,
	dropDups: true
});