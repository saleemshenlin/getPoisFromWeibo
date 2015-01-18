var mongoose = require('mongoose');
var poiSchema = mongoose.Schema({
    poiid: String,
    title: String,
    lon: Number,
    lat: Number,
    category: Number,
    category_name: String,
    address: String
}, { collection: 'pois'});

poiSchema.methods.print = function () {
    var printName = this.title
        ? "Title: " + this.title
        : "This has not title!";
    console.log(printName);
}

poiSchema.statics = {
    fetch:function(cb){
        return this
            .find({})
            .limit(15)
            .exec(cb)
    }
}

module.exports = poiSchema;