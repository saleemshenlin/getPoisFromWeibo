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

//map-reduce
var map = function() {
	var yearMonth = this.created_at.getFullYear() + '-' + (this.created_at.getMonth() + 1) + '-' + this.created_at.getDate();
	//var yearMonth = this.created_at.getDay();
	var value = {
		count: 1,
		pics: parseInt(this.pic_count),
		year: this.created_at.getFullYear(),
		month: this.created_at.getMonth(),
		date: this.created_at.getDate(),
		day: this.created_at.getDay()
	};
	emit(yearMonth, value);
}

var reduce = function(key, countObjVals) {
	reducedVal = {
		count: 0,
		pics: 0,
		year: 0,
		month: 0,
		date: 0,
		day: 0
	};

	for (var idx = 0; idx < countObjVals.length; idx++) {
		reducedVal.count += countObjVals[idx].count;
		reducedVal.pics += countObjVals[idx].pics;
		reducedVal.year = countObjVals[idx].year;
		reducedVal.month = countObjVals[idx].month;
		reducedVal.date = countObjVals[idx].date;
		reducedVal.day = countObjVals[idx].day;
	}

	return reducedVal;
}

var query = {
	created_at: {
		$gte: new Date(2012, 8, 24),
		$lte: new Date(2012, 9, 15)
	}
}

var finalize = function(key, reducedVal) {
	reducedVal.time = new Date(parseInt(reducedVal.year), parseInt(reducedVal.month), parseInt(reducedVal.date))
	return reducedVal;

};
db.weibo.mapReduce(map, reduce, {
	query: query,
	out: "WeiboCountWeekend",
	finalize: finalize
})

//map-reduce
var map = function() {
	var yearMonth = this.created_at.getFullYear() + '-' + (this.created_at.getMonth() + 1) + '-' + this.created_at.getDate();
	//var yearMonth = this.created_at.getDay();
	var value = {
		count: 1,
		pics: parseInt(this.pic_count),
		year: this.created_at.getFullYear(),
		month: this.created_at.getMonth(),
		date: this.created_at.getDate(),
		day: this.created_at.getDay()
	};
	emit(yearMonth, value);
}

var reduce = function(key, countObjVals) {
	reducedVal = {
		count: 0,
		pics: 0,
		year: 0,
		month: 0,
		date: 0,
		day: 0
	};

	for (var idx = 0; idx < countObjVals.length; idx++) {
		reducedVal.count += countObjVals[idx].count;
		reducedVal.pics += countObjVals[idx].pics;
		reducedVal.year = countObjVals[idx].year;
		reducedVal.month = countObjVals[idx].month;
		reducedVal.date = countObjVals[idx].date;
		reducedVal.day = countObjVals[idx].day;
	}

	return reducedVal;
}

var query = {
	created_at: {
		$gte: new Date(2012, 8, 24),
		$lte: new Date(2012, 9, 15)
	}
}

var finalize = function(key, reducedVal) {
	reducedVal.time = new Date(parseInt(reducedVal.year), parseInt(reducedVal.month), parseInt(reducedVal.date))
	return reducedVal;

};
db.weibo.mapReduce(map, reduce, {
	query: query,
	out: "WeiboCountWeekend",
	finalize: finalize
})

//weibo user
var map = function() {
	var user = '001-' + this.value.province + '-' + this.value.city;
	//var yearMonth = this.created_at.getDay();
	var value = {
		count: 1,
		name: this.value.name,
		province: this.value.province,
		city: this.value.city,
		location: this.value.location
	};
	emit(user, value);
}

var reduce = function(key, countObjVals) {
	reducedVal = {
		count: 0,
		pics: 0,
		name: 0,
		province: 0,
		city: 0,
		location: 0
	};

	for (var idx = 0; idx < countObjVals.length; idx++) {
		reducedVal.count += countObjVals[idx].count;
		reducedVal.pics += countObjVals[idx].pics;
		reducedVal.name = countObjVals[idx].name;
		reducedVal.province = countObjVals[idx].province;
		reducedVal.city = countObjVals[idx].city;
		reducedVal.location = countObjVals[idx].location;
	}

	return reducedVal;
}

var query = {
	'user.province': {
		$ne: 31
	}
}

var finalize = function(key, reducedVal) {
	reducedVal.time = new Date(parseInt(reducedVal.name), parseInt(reducedVal.province), parseInt(reducedVal.city))
	return reducedVal;
};

db.WeiboCountUser.mapReduce(map, reduce, {
	//query: query,
	out: "WeiboCountUserPlace",
	//finalize: finalize
})