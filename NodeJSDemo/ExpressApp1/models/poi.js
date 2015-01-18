var mongoose = require('mongoose');
var poiSchema = require('../schemas/poi');
var poi = mongoose.model('POI', poiSchema);

module.exports = poi;