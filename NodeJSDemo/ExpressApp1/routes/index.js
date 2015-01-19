var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
    res.render('index', {
        title: '基于众源地理数据的旅游热度可视化'
    });
});

module.exports = router;