var express = require('express');
var http = require('http');
var mongoose = require('mongoose');
var cheerio = require('cheerio');
var Poi = require('../models/poi.js');
var Weibo = require('../models/weibo.js');
var router = express.Router();
var date;
//mongoose.connect('mongodb://192.168.199.111/weibo');

// change appkey to yours
//var appkey = '1654060665';
//var secret = '9b337ffd48099b2b94eecb568b65d1dc';
//var oauth_callback_url = 'http://weibo.com/saleemshenlin';

/* GET insert weibo page. */
router.get('/', function(req, res) {
    //var poiid = 'B2094655D26BA7FF4199&';
    Poi.fetch(function(err, pois) {
        if (err) {
            console.log(err)
        };
        res.render('weiboinsert', {
            title: '微博详细'
        });
        getTimeLineById(pois, 809, 1);
    });

    var getTimeLineById = function(pois, index, page) {
        var requestUrl = 'http://api.weibo.com/2/place/poi_timeline.json?' + 'source=1654060665' + '&access_token=2.00cGubtBAhG9FEcf2f744601CkyoTB' + '&count=50' + '&page=' + page + '&poiid=' + pois[index].poiid;
        var responseData = '';
        http.get(requestUrl, function(res) {
            console.log("Got response: " + res.statusCode);
            res.on('data', function(data) {
                responseData += data;
            }).on('end', function() {
                var jsObjs = JSON.parse(responseData).statuses;
                if (!jsObjs) {
                    index = index + 1;
                    page = 1;
                    if (index < pois.length) {
                        getTimeLineById(pois, index, page);
                        console.log('finish:index' + index);
                    } else {
                        console.log('finish:all!');
                    }
                } else {
                    console.log("res end: " + jsObjs);
                    saveNewWeibo(jsObjs, 0, pois, index, page);
                }
            });
        }).on('error', function(e) {
            console.log("Got error: " + e.message);
        });
    }
    var saveNewWeibo = function(newweibos, indexN, pois, indexP, page) {
        Weibo.fetchByAnnoID(pois[indexP].poiid, function(err, weibo) {
            if (err) {
                console.log(err);
                return err;
            }
            if (weibo.length > 0) {
                if (indexN === 0 && page === 1) {
                    date = weibo[0].created_at;
                }
                console.log('origin:' + pois[indexP].poiid + 'created_at:' + date);
                var newWeibo = newweibos[indexN];
                if (new Date(newWeibo.created_at) > date) {
                    console.log('save:' + newWeibo.id + ' created_at:' + newWeibo.created_at+' indexP:'+ indexP);
                    if (!newWeibo['deleted']) {
                        var annotation = {
                            poiid: newWeibo['annotations'][0]['place']['poiid'],
                            title: newWeibo['annotations'][0]['place']['title'],
                            lat: newWeibo['annotations'][0]['place']['lat'] < 50 ? newWeibo['annotations'][0]['place']['lat'] : newWeibo['annotations'][0]['place']['lon'],
                            lon: newWeibo['annotations'][0]['place']['lon'] > 110 ? newWeibo['annotations'][0]['place']['lon'] : newWeibo['annotations'][0]['place']['lat']
                        }
                        if (newWeibo['geo'] == null) {
                            var itemLat = annotation.lat;
                            var itemLon = annotation.lon;
                        } else {
                            itemLat = newWeibo['geo']['coordinates'][0] < 50 ? newWeibo['geo']['coordinates'][0] : newWeibo['geo']['coordinates'][1];
                            itemLon = newWeibo['geo']['coordinates'][0] > 110 ? newWeibo['geo']['coordinates'][0] : newWeibo['geo']['coordinates'][1];
                        }
                        var $ = cheerio.load(newWeibo['source']);
                        var weiboContent = {
                            created_at: new Date(newWeibo['created_at']),
                            weibo_id: newWeibo['id'],
                            text: newWeibo['text'],
                            source: $('a').text(),
                            //pic_ids: newWeibo['pic_ids'].toString(),
                            pic_count: newWeibo['pic_ids'].length,
                            annotations: {
                                poiid: annotation.poiid,
                                title: annotation.title,
                                lat: annotation.lat,
                                lon: annotation.lon
                            },
                            user: {
                                id: newWeibo['user']['id'],
                                name: newWeibo['user']['name'],
                                province: newWeibo['user']['province'],
                                city: newWeibo['user']['city'],
                                location: newWeibo['user']['location']
                            },
                            geo: {
                                lat: itemLat,
                                lon: itemLon
                            }
                        };
                        var weiboItem = new Weibo(weiboContent);
                        weiboItem.save(function(err, weiboItem) {
                            if (!err) {
                                indexN = indexN + 1;
                                if (indexN < newweibos.length) {
                                    saveNewWeibo(newweibos, indexN, pois, indexP, page);
                                } else {
                                    page = page + 1;
                                    getTimeLineById(pois, indexP, page);
                                }
                            }
                        });
                    } else {
                        indexN = indexN + 1;
                        if (indexN < newweibos.length) {
                            saveNewWeibo(newweibos, indexN, pois, indexP, page);
                        } else {
                            page = page + 1;
                            getTimeLineById(pois, indexP, page);
                        }
                    }
                } else {
                    indexP = indexP + 1;
                    page = 1;
                    if (indexP < pois.length) {
                        getTimeLineById(pois, indexP, page);
                        console.log('finish:index' + indexP);
                    } else {
                        console.log('finish:all!');
                    }
                }
            } else {
                indexP = indexP + 1;
                page = 1;
                if (indexP < pois.length) {
                    getTimeLineById(pois, indexP, page);
                    console.log('finish:index' + indexP);
                } else {
                    console.log('finish:all!');
                }
            }
        });
    }

});

module.exports = router;