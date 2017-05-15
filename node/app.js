const express = require('express');
const path = require('path');
const favicon = require('serve-favicon');
const logger = require('morgan');
const cookieParser = require('cookie-parser');
const bb = require('express-busboy');
const bodyParser = require('body-parser');
const session = require('express-session');
const hbs = require('hbs');
const fs = require('fs');
const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('database/database.db');

const app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'hbs');
hbs.registerPartials(path.join(__dirname, '/views/partials'));

app.use(favicon(path.join(__dirname, 'public', 'images', 'logo.jpg')));
app.use(logger('dev'));
app.use(bodyParser.json({ limit: '5mb' }));
app.use(bodyParser.urlencoded({ limit: '5mb', extended: true }));
app.use(cookieParser());

app.use(session({
    key: 'el_gato_de_nyan',
    secret: 'nyan_cat'
}));


app.use(session({ secret: 'nyan cat' }));
app.use(express.static(path.join(__dirname, 'public')));
app.use(express.static(path.join(__dirname, 'images')));
bb.extend(app, {
    upload: true,
    allowedPath: /./,
});

// catch 404 and forward to error handler
app.use((req, res, next) => {
    const err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use((err, req, res) => {
        res.status(err.status || 500);
        res.render('404', {
            message: err.message,
            error: err,
            title: 'JRCM: 404',
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use((err, req, res) => {
    res.status(err.status || 500);
    res.render('404', {
        message: err.message,
        error: {},
        title: 'JRCM: 404',
    });
});

db.serialize(() => {
    db.run("CREATE TABLE User( email TEXT PRIMARY KEY, name TEXT NOT NULL, address TEXT NOT NULL);");
    db.run("CREATE TABLE Book( title TEXT PRIMARY KEY, stock INTEGER, price INTEGER NOT NULL);");
    db.run("CREATE TABLE Encomenda( uuid TEXT PRIMARY KEY, quantity INTEGER NOT NULL, state TEXT NOT NULL, email TEXT REFERENCES User(email), title TEXT REFERENCES Book(title));");
});

db.close();

module.exports = app;