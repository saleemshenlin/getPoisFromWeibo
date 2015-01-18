var mongoose = require('mongoose');
var weiboSchema = require('../schemas/weibo');
var Weibo = mongoose.model('WEIBO', weiboSchema);

module.exports = Weibo;