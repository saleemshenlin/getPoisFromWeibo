var mongoose = require('mongoose');
var weiboSchema = mongoose.Schema({
    created_at: Date,
    updated_at: { type: Date, default: Date.now },
    weibo_id: String,
    text: String,
    source: String,
    pic_ids: [{ pic_id: String }],
    pic_count: Number,
    geo: {
        lat: Number,
        lon: Number
    },
    user: {
        id: String,
        name: String,
        province: Number,
        city: Number,
        location : String
    },
    annotations: {
        poiid: String,
        title: String,
        lat: Number,
        lon: Number
    }
});
weiboSchema.statics = {
    fetch: function (cb){
        return this
            .find({})
            .sort('created_at')
            .limit(10)
            .exec(cb)
    },
    findById: function (id, cb){
        return this
            .findOne({weibo_id:id})
            .exec(cb)
    },
    fetchDate: function(startdate,enddate,cb){
        return this
            .find({'created_at':{$gte:startdate,$lte:enddate}})
            .sort('created_at')
            .exec(cb)
    }
}
module.exports = weiboSchema;