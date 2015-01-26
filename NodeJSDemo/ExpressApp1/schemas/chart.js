var mongoose = require('mongoose');
var chartSchema = mongoose.Schema({
    _id: String,
    value: {
        count: Number,
        pics: Number,
        year: Number,
        month: Number,
        date: Number,
        day: Number,
        time: Date
    }
}, {
    collection: 'WeiboCountWeekend'
});


chartSchema.statics = {
    fetch: function(cb) {
        return this
            .find({})
            .sort({
                'value.time': 1
            })
            .exec(cb)
    }
}

module.exports = chartSchema;