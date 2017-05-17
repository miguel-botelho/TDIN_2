var express = require('express');
var router = express.Router();
var book = require('../database/book.js');
var user = require('../database/user.js');

router.get('/', function (req, res) {
    book.getAllBooks((books) => {
        res.render('index', { title: 'TDIN: Online Book Store', user: req.session.user, books: books });
    });
});

router.post('/order', function (req, res) {
    if (req.session.user === undefined) {
        res.redirect('/login');
    } else { // place order
        console.log(req.body);
    }
});

router.get('/login', function (req, res) {
    if (req.session.user === undefined)
        res.render('login', { title: 'TDIN: Online Book Store', user: req.session.user });
    else res.redirect('/orders');
});

router.get('/logout', (req, res) => {
    req.session.user = undefined;
    res.redirect('/');
});

router.post('/login', function (req, res) {
    user.compareCredentials(req.body.email, req.body.password, (response) => {
        if (response) {
            req.session.user = req.body.email;
            res.redirect('/');
        } else res.redirect('/login');
    });
});

router.post('/register', function (req, res) {
    console.log(req.body);
    if (req.body.password !== req.body.confirmPassword)
        res.redirect('/login');
    else {
        user.createUser(req.body.name, req.body.password, req.body.address, req.body.email, (response) => {
            if (response === undefined)
                res.redirect('/');
            else res.redirect('/login');
        });
    }
});

module.exports = router;