var mongoose = require('mongoose');
var areaSchema = require('../schemas/area');
var area = mongoose.model('areas', areaSchema);

module.exports = area;