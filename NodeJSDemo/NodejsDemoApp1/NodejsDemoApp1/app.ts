console.log('Hello world');
var fs = require('fs');
var mongoose = require('mongoose');
var cheerio = require('cheerio')
mongoose.connect('mongodb://localhost/weibo_test1');

var db = mongoose.connection;
db.on('error', console.error.bind(console, 'connection error:'));
db.once('open', function (callback) {
    console.log('connected to MongoDB');
});

//var $ = cheerio.load('<a href=\"http://app.weibo.com/t/feed/3G5oUM\" rel=\"nofollow\">iPhone 5s</a>');
//var str = $('a').text();
//console.log(str);
var weiboSchema = mongoose.Schema({
    created_at: Date,
    updated_at: { type: Date, default: Date.now },
    weibo_id: String,
    text: String,
    source: String,
    pic_ids: Array,
    pic_count: Number,
    geo: {
        lat: Number,
        lon: Number
    },
    user: {
        id: String,
        name: String,
        province: Number,
        city: Number,
        location: String
    },
    annotations: {
        poiid: String,
        title: String,
        lat: Number,
        lon: Number
    }
});
weiboSchema.methods.print = function () {
    var printName = this.weibo_id
        ? "weibo_id: " + this.weibo_id
        : "This has not title!";
    console.log(printName);
}
var Weibo = mongoose.model('WEIBO', weiboSchema);

fs.readFile('./datas/B2094757D06FAAFA4899.json', function (err, data) {
    if (err)
        throw err;

    var jsonObj = JSON.parse(data);
    var space = ' ';
    var newLine = ' . ';
    var chunks = [];
    var length = 0;

    for (var i = 0; i < jsonObj.length; i++) {
        var item = jsonObj[i];
        if (!item['deleted']) {
            var $ = cheerio.load(item['source']);
            var weiboItem = new Weibo({
                created_at: Date(item['created_at']),
                weibo_id: item['id'],
                text: item['text'],
                source: $('a').text(),
                pic_ids: item['pic_ids']
            })
            weiboItem.save(function (err, weiboItem) {
                    if (err) return console.error(err);
                    weiboItem.print();
                });
        }
    }
});

/*

insert poi from mergeresult_final



var Poi = mongoose.model('POI', poiSchema); 
fs.readFile('./datas/mergeresult_final.json', function (err, data) {
    if (err)
        throw err;

    var jsonObj = JSON.parse(data);
    var space = ' ';
    var newLine = ' . ';
    var chunks = [];
    var length = 0;

    for (var i = 0; i < jsonObj.length ; i++) {
        var item = jsonObj[i];
        var poiItem = new Poi({
            poiid: item['poiid'],
            title: item['title'],
            lon: item['lon'],
            lat: item['lat'],
            category: item['category'],
            category_name: item['category_name'],
            address: item['address']
        })
        poiItem.save(function (err, poiItem) {
            if (err) return console.error(err);
            poiItem.print();
        });
    }
});

*/
