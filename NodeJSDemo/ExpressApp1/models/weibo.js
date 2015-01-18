var mongoose = require('mongoose');
var weiboSchema = require('../schemas/weibo');
var Weibo = mongoose.model('weibo', weiboSchema);
//Weibo.prototype.save = function (weibo, callback){
//        var newWeibo = new Weibo({
//            created_at: weibo.created_at,
//            updated_at: weibo.updated_at,
//            weibo_id: weibo.weibo_id,
//            text: weibo.text,
//            source: weibo.source,
//            pic_ids: weibo.pic_ids,
//            pic_count: weibo.pic_count,
//            geo: weibo.geo,
//            user: weibo.user,
//            annotations: weibo.annotations
//        });
//        newWeibo.save(function (err) {
//            callback(err);
//        });
//}
module.exports = Weibo;