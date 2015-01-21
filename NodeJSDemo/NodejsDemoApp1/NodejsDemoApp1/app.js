console.log('Hello world');
var fs = require('fs');
var mongoose = require('mongoose');

//var cheerio = require('cheerio')
//var path = './data';
mongoose.connect('mongodb://192.168.199.111/weibo');

var db = mongoose.connection;
db.on('error', console.error.bind(console, 'connection error:'));
db.once('open', function (callback) {
    console.log('connected to MongoDB');
});

var areaSchema = mongoose.Schema({
    created_at: { type: Date, default: Date.now },
    updated_at: { type: Date, default: Date.now },
    area_id: Number,
    title: String,
    geo: {
        lat: Number,
        lon: Number
    }
}, { collection: 'area' });

var Area = mongoose.model('AREA', areaSchema);

//insertWeiboDate('./datas/B2094757D06EAAF4409D.json');
//var fileList = fs.readdirSync(path);
//fileList.forEach(function (item) {
//    console.log(path + '/' + item);
//    insertWeiboDate(path + '/' + item);
//});
//function insertWeiboDate(filename) {
//    fs.readFile(filename, function (err, data) {
//        if (err)
//            throw err;
//        var jsonObj = JSON.parse(data);
//        var annotation;
//        for (var i = 0; i < jsonObj.length; i++) {
//            var item = jsonObj[i];
//            var itemLat, itemLon;
//            if (i === 0) {
//                annotation = {
//                    poiid: item['annotations'][0]['place']['poiid'],
//                    title: item['annotations'][0]['place']['title'],
//                    lat: item['annotations'][0]['place']['lat'] < 50 ? item['annotations'][0]['place']['lat'] : item['annotations'][0]['place']['lon'],
//                    lon: item['annotations'][0]['place']['lon'] > 110 ? item['annotations'][0]['place']['lon'] : item['annotations'][0]['place']['lat']
//                }
//        }
//            if (!item['deleted']) {
//                if (item['geo'] == null) {
//                    itemLat = annotation.lat;
//                    itemLon = annotation.lon;
//                }
//                else {
//                    itemLat = item['geo']['coordinates'][0] < 50 ? item['geo']['coordinates'][0] : item['geo']['coordinates'][1];
//                    itemLon = item['geo']['coordinates'][0] > 110 ? item['geo']['coordinates'][0] : item['geo']['coordinates'][1];
//                }
//                var $ = cheerio.load(item['source']);
//                var weiboItem = new Weibo({
//                    created_at: new Date(item['created_at']),
//                    weibo_id: item['id'],
//                    text: item['text'],
//                    source: $('a').text(),
//                    pic_ids: item['pic_ids'],
//                    pic_count: item['pic_ids'].length,
//                    annotations: {
//                        poiid: annotation.poiid,
//                        title: annotation.title,
//                        lat: annotation.lat,
//                        lon: annotation.lon
//                    },
//                    user: {
//                        id: item['user']['id'],
//                        name: item['user']['name'],
//                        province: item['user']['province'],
//                        city: item['user']['city'],
//                        location: item['user']['location']
//                    },
//                    geo: {
//                        lat: itemLat,
//                        lon: itemLon
//                    }
//                });
//                weiboItem.save(function (err, weiboItem) {
//                    if (err) return console.error(err);
//                    weiboItem.print();
//                });
//            }
//        }
//    });
//}
insertAreaDate('./datas/area_center.json');
function insertAreaDate(filename) {
    fs.readFile(filename, function (err, data) {
        if (err)
            throw err;

        var jsonObj = JSON.parse(data).features;

        for (var i = 0; i < jsonObj.length; i++) {
            var item = jsonObj[i];
            var areaItem = new Area({
                area_id: item.attributes.OBJECTID,
                title: item.attributes.NAME,
                geo: {
                    lat: item.geometry.y,
                    lon: item.geometry.x
                }
            });
            areaItem.save(function (err, weiboItem) {
                if (err)
                    return console.error(err);
            });
        }
    });
}
//# sourceMappingURL=app.js.map
