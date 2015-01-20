var express = require('express');
var mongoose = require('mongoose');
var Weibo = require('../models/weibo.js');
var router = express.Router();
/* Post fetch weibo by date */
router.post('/',function (req,res) {
	var startDate = new Date(req.body.date);
    var endDate = new Date(startDate.valueOf()+ 24*60*60*1000);
    Weibo.fetchDate(startDate,endDate,function (err, weibos) {
        if (err) {
            console.error(err);
        }
        res.send(weibos);
    });
});
module.exports = router;