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
const user = require('./database/user.js');
const book = require('./database/book.js');
const order = require('./database/order.js');
const amqp = require('amqplib/callback_api');

var index = require('./routes/index');
var orders = require('./routes/orders');

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


app.use('/', index);
app.use('/orders', orders);

amqp.connect('amqp://localhost', function (err, conn) {
    conn.createChannel(function (err, ch) {
        var q = 'orderWarehouse';

        ch.assertQueue(q, { durable: false });
        console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", q);
        ch.consume(q, function (msg) {
            console.log(msg.content.toString());
            order.updateOrderByWarehouse(msg.content.toString(), (response) => {
                emitSocket('orderWarehouse', msg.content.toString());
                console.log(" [x] Order %s will be dispatched the day after tomorrow.", msg.content.toString());
            });
        }, { noAck: true });
    });
});

amqp.connect('amqp://localhost', function (err, conn) {
    conn.createChannel(function (err, ch) {
        var q = 'orderStore';

        ch.assertQueue(q, { durable: false });
        console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", q);
        ch.consume(q, function (msg) {
            console.log(msg.content.toString());
            order.updateOrderByStore(msg.content.toString(), (response) => {
                console.log(" [x] Order %s will be dispatched today.", json.OrderCode);
            });
        }, { noAck: true });
    });
});

var debug = require('debug')('TDIN:server');
var http = require('http');
var port = normalizePort(process.env.PORT || '3000');
app.set('port', port);
var server = http.createServer(app);

server.listen(port);
server.on('error', onError);
server.on('listening', onListening);
var io = require('socket.io')(server);

function normalizePort(val) {
    var port = parseInt(val, 10);
    if (isNaN(port)) {
        // named pipe
        return val;
    }
    if (port >= 0) {
        // port number
        return port;
    }
    return false;
}

function onError(error) {
    if (error.syscall !== 'listen') {
        throw error;
    }
    var bind = typeof port === 'string'
        ? 'Pipe ' + port
        : 'Port ' + port;
    // handle specific listen errors with friendly messages
    switch (error.code) {
        case 'EACCES':
            console.error(bind + ' requires elevated privileges');
            process.exit(1);
            break;
        case 'EADDRINUSE':
            console.error(bind + ' is already in use');
            process.exit(1);
            break;
        default:
            throw error;
    }
}

function receiveSockets() {
    io.on('connection', (socket) => {
        socket.on('sell', (data) => {
            console.log("sell ");
            data = JSON.parse(data);
            console.log(data);
            order.placeOrder(data.user.email, data.numBooks, data.book.Name, (respo) => {
                console.log('Order ' + respo + ' placed.');
            });
        });

        socket.on('AvailableBooks', (data) => {
            book.getAllBooks((books) => {
                console.log('Available Books');
                socket.emit("AvailableBooks", JSON.stringify(books));
            });
            /*order.placeOrder(data.user.email, data.NumBooks, data.book.Name, (respo) => {
                console.log('Order ' + respo + ' placed.');
            });*/
        });
        
        socket.on('ordersWarehouse', (data) => {
            order.getOrdersInWarehouse((orders) => {
                console.log(orders);
                socket.emit('ordersWarehouse', JSON.stringify(orders));
            });
        });

        socket.on('ordersStore', (data) => {
            order.getOrdersInStore((orders) => {
                socket.emit('ordersStore', JSON.stringify(orders));
            });
        });

        socket.on('accept', (uuid) => {
            console.log('Accept ' + uuid);
            order.updateOrderByStore(uuid, (respo) => {
                console.log('Accepted order ' + uuid);
            });
        });
    });
}

function emitSocket(name, message) {
    io.emit(name, message);
}

function onListening() {
    var addr = server.address();
    var bind = typeof addr === 'string'
        ? 'pipe ' + addr
        : 'port ' + addr.port;
    debug('Listening on ' + bind);
}

receiveSockets();

module.exports.emitSocket = emitSocket;