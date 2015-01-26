var express = require('express');
var router = express.Router();

/* GET weibochart page. */
router.get('/', function(req, res) {
	res.render('weibochart', {
		title: '微博图表'
	});
});

module.exports = router;