var express = require('express');
var http = require('http');
var mongoose = require('mongoose');
var Poi = require('../models/poi.js');
var router = express.Router();
//mongoose.connect('mongodb://192.168.199.111/weibo');

// change appkey to yours
// var appkey = '1654060665';
// var secret = '9b337ffd48099b2b94eecb568b65d1dc';
// var oauth_callback_url = 'http://weibo.com/saleemshenlin';
// weibo.init('weibo', appkey, secret, oauth_callback_url);

/* GET insert weibo page. */
router.get('/', function (req, res) {
	//var poiid = 'B2094655D26BA7FF4199&';

	Poi.fetch(function(err,pois){
		if (err) {console.log(err)};
	    res.render('weiboinsert', { 
	    	title: '微博详细'
	    });		
	    for (var i = 0; i < pois.length; i++) {
	    	var requestUrl = 'http://api.weibo.com/2/place/poi_timeline.json?'
	    		+'source=1654060665'
	    		+'&access_token=2.00cGubtBAhG9FEcf2f744601CkyoTB'
	    		+'&poiid='
	    		+pois[i].poiid;
	    	var responseData = '';
	    	http.get(requestUrl, function(res) {
	    		console.log("Got response: " +res.statusCode);
	    		res.on('data',function(data){
	    			responseData+=data;
	    		}).on('end',function(){
	    			console.log("res end: " +responseData);
	    		});
	    	}).on('error', function(e) {
	    		console.log("Got error: " + e.message);
	    	});
	    };
	});

});

module.exports = router;