var express = require('express');
var mongoose = require('mongoose');
var Chart = require('../models/chart.js');
var router = express.Router();

/* Post fetch Chart by date */
router.post('/', function(req, res) {
	Chart.fetch(function(err, data) {
		if (err) {
			console.error(err);
		}
		res.send(data);
	});
});
module.exports = router;