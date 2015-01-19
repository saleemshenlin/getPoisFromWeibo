var express = require('express');
var mongoose = require('mongoose');
var Weibo = require('../models/weibo.js');
var router = express.Router();
/* Post fetch weibo by date */
router.post('/',function (req,res) {
	var startDate = new Date(2013,1,1);
    var endDate = new Date(2013,1,2);
    Weibo.fetchDate(startDate,endDate,function (err, weibos) {
        if (err) {
            console.error(err);
        }
        res.send(weibos);
    });
});
module.exports = router;