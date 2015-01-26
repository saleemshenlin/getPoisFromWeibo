var mongoose = require('mongoose');
var chartSchemas = require('../schemas/chart');
var chart = mongoose.model('charts', chartSchemas);

module.exports = chart;