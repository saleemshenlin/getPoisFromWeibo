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
        lat:Number,
        lon:Number
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
weiboSchema.methods.print = function () {
    var printName = this.weibo_id
        ? "weibo_id: " + this.weibo_id
        : "This has not title!";
    console.log(printName);
}
