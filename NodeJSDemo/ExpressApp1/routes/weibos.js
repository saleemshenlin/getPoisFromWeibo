var express = require('express');
var mongoose = require('mongoose');
var Weibo = require('../models/weibo.js');
var router = express.Router();
mongoose.connect('mongodb://59.78.138.14/weibo');

/* GET weibo list page. */
router.get('/', function(req, res) {
    var startDate = new Date(2013, 4, 1);
    var endDate = new Date(2013, 4, 2);
    Weibo.fetch(function(err, weibos) {
        if (err) {
            console.error(err);
        }
        res.render('weibos', {
            title: '微博列表',
            weibos: weibos
        });

    });

});

module.exports = router;