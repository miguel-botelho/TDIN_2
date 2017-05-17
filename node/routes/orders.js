var express = require('express');
var router = express.Router();
var order = require('../database/order.js');

router.get('/', function (req, res) {
    if (req.session.user !== undefined) {
        order.getAllOrdersByEmail(req.session.user, (orders) => {
            res.render('orders', { title: 'TDIN: Online Book Store', user: req.session.user, orders: orders })
        });
    } else res.redirect('/login');
});

module.exports = router;