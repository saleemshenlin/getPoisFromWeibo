var mongoose = require('mongoose');
var poiSchema = require('../schemas/poi');
var poi = mongoose.model('pois', poiSchema);

module.exports = poi;