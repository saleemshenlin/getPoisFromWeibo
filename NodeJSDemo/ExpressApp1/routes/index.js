var express = require('express');
var mongoose = require('mongoose');
var WeiboArea = require('../models/area.js');
var router = express.Router();


/* GET home page. */
router.get('/', function(req, res) {
	WeiboArea.fetch(function(err, areas) {
		if (err) {
			console.error(err);
		}
		res.render('index', {
			title: '基于众源地理数据的旅游热度可视化',
			areas: areas
		});

	});
});

module.exports = router;