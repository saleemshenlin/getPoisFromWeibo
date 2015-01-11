var poiSchema = mongoose.Schema({
    poiid: String,
    title: String,
    lon: Number,
    lat: Number,
    category: Number,
    category_name: String,
    address: String
});

poiSchema.methods.print = function () {
    var printName = this.title
        ? "Title: " + this.title
        : "This has not title!";
    console.log(printName);
}