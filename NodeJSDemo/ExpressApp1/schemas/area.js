var mongoose = require('mongoose');
var areaSchema = mongoose.Schema({
    area_id: String,
    title: String,
    geo: {
        lat: Number,
        lon: Number
    },
    created_at: Date,
    updated_at: Date
}, {
    collection: 'area'
});

areaSchema.methods.print = function() {
    var printName = this.title ? "Title: " + this.title : "This has not title!";
    console.log(printName);
}

areaSchema.statics = {
    fetch: function(cb) {
        return this
            .find({})
            .exec(cb)
    }
}

module.exports = areaSchema;