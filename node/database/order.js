const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('database/database.db');
const nodemailer = require('nodemailer');
const uuidV4 = require('uuid/v4');
const sockets = require('../app.js');
const async = require('async');
const amqp = require('amqplib/callback_api');

function placeOrder(email, quantity, title, callback) {
    const date = new Date();
    date.setDate(date.getDate() + 1);
    let stmt = db.prepare('SELECT * FROM Book WHERE title = ?');
    // get the book
    stmt.get(title, (err, book) => {
        stmt = db.prepare('SELECT * FROM User WHERE email = ?');
        stmt.get(email, (err, user) => {
            if (book.stock > quantity) { // place the order, discount on the quantity and send an e-mail
                discountStockOnBook(quantity, book.stock, title, () => {
                    createOrder(email, title, quantity, 'Dispatch Will Ocurr At ' + date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear(), (uuid) => {
                        sendEmail(email, 'Dispatching order number: ' + uuid + ' at ' + date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear()
                            + '.\n\nTitle: ' + title + '.\nQuantity: ' + quantity + '.\nPreco Por Livro: ' + book.price + '.\nPreco Total: '
                            + (book.price * quantity), (response) => {
                                var json = {
                                    'user': {
                                        'email': email,
                                        'name': user.name,
                                        'address': user.address,
                                    },
                                    'book': {
                                        'Name': title,
                                        'PageNumber': 0,
                                        'Author': '',
                                        'Stars': 0,
                                        'Price': book.price,
                                        'ISBN': 0,
                                    },
                                    'NumBooks': Number(quantity) + 10,
                                    'OrderCode': uuid
                                };
                                sockets.emitSocket('newOrder', json);
                                callback(uuid);
                            });
                    });
                });
            } else { // place the order, send an e-mail and connect to the warehouse server (ask for the quantity + 10)
                createOrder(email, title, quantity, 'Waiting Expedition', (uuid) => {
                    sendEmail(email, 'Waiting Expedition of order number: ' + uuid + '.\n\nTitle: ' + title + '.\nQuantity: ' + quantity
                        + '.\nPreco Por Livro: ' + book.price + '.\nPreco Total: ' + (book.price * quantity), (response) => {
                            // connecting to the Warehouse Server
                            amqp.connect('amqp://tdin:tdin@172.30.7.167', function (err, conn) {
                                console.log(err);
                                conn.createChannel(function (err, ch) {
                                    console.log(err);
                                    var q = 'order';
                                    ch.assertQueue(q, { durable: false });
                                    var json = {
                                        'user': {
                                            'email': email,
                                            'name': user.name,
                                            'address': user.address,
                                        },
                                        'book': {
                                            'Name': title,
                                            'PageNumber': 0,
                                            'Author': '',
                                            'Stars': 0,
                                            'Price': book.price,
                                            'ISBN': 0,
                                        },
                                        'NumBooks': Number(quantity) + 10,
                                        'OrderCode': uuid
                                    };
                                    // Note: on Node 6 Buffer.from(msg) should be used
                                    ch.sendToQueue(q, new Buffer(JSON.stringify(json)));
                                    console.log(" [x] Sent a New Order to warehouse");
                                    callback(uuid);
                                });
                            });
                        });
                });
            }
        });
    });
}

function getAllOrdersByEmail(email, callback) {
    const stmt = db.prepare('SELECT * FROM Encomenda WHERE email = ?');
    stmt.all(email, (err, orders) => {
        callback(orders);
    });
}

// to be called when the gui client is called in the warehouse
function updateOrderByWarehouse(uuid, callback) {
    const date = new Date();
    date.setDate(date.getDate() + 2);
    const temp = 'Dispatch Will Ocurr At ' + date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
    const stmt = db.prepare('UPDATE Encomenda SET state = ? WHERE uuid = ?');
    stmt.get([temp, uuid], (err, row) => {
        callback(row);
    });
}

// to be called when the gui client is called in the store
function updateOrderByStore(uuid, callback) {
    const date = new Date();
    const temp = 'Dispatched At ' + date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
    // get title of book
    const stmt = db.prepare('SELECT * FROM Encomenda WHERE uuid = ?');
    stmt.get(uuid, (err, order) => {
        // get stock of book
        stmt = db.prepare('SELECT * FROM Book WHERE title = ?');
        stmt.get(order.title, (err, book) => {
            // update stock of book
            stmt = db.prepare('UPDATE Book SET stock = ? WHERE title = ?');
            stmt.get([order.quantity + 10, order.title], (err, row) => {
                // check all other orders
                stmt = db.prepare('SELECT * FROM Encomenda WHERE title = ? AND quantity <= ?');
                stmt.all([order.title, order.quantity + 10], (err, rows) => {
                    async.each(rows, (row, next) => {
                        stmt = db.prepare('UPDATE Encomenda SET state = ? WHERE uuid = ?');
                        stmt.get([temp, uuid], (err, r) => {
                            // é preciso chamar socket???
                            sendEmail(order.email, 'Your order number: ' + uuid + ' will be dispatched today.\n\nTitle: ' + order.title + '.\nQuantity: ' + order.quantity
                                + '.\nPreco Por Livro: ' + book.price + '.\nPreco Total: ' + (book.price * order.quantity), (response) => {
                                    next();
                                });
                        });
                    }, (err) => {
                        // update the order's state
                        if (!order.status.includes('Dispatched')) {
                            stmt = db.prepare('UPDATE Encomenda SET state = ? WHERE uuid = ?');
                            stmt.get([temp, uuid], (err, row) => {
                                sendEmail(order.email, 'Your order number: ' + uuid + ' will be dispatched today.\n\nTitle: ' + order.title + '.\nQuantity: ' + order.quantity
                                    + '.\nPreco Por Livro: ' + book.price + '.\nPreco Total: ' + (book.price * order.quantity), (response) => {
                                        callback(response);
                                    });
                            });
                        }
                    });
                });
            });
        });
    });
}

function discountStockOnBook(quantity, quantityBook, title, callback) {
    const stmt = db.prepare('UPDATE Book SET stock = ? WHERE title = ?');
    stmt.get([quantityBook - quantity, title], (err, row) => {
        callback(row);
    });
}

function getOrder(uuid, callback) {
    const stmt = db.prepare('SELECT * FROM Encomenda, Book WHERE uuid = ?');
    stmt.get(uuid, (err, row) => {
        callback(row);
    });
}

function createOrder(email, title, quantity, state, callback) {
    const stmt = db.prepare('INSERT INTO Encomenda (uuid, quantity, state, email, title) VALUES(?,?,?,?,?);');
    const uuid = uuidV4();
    stmt.get([uuid, quantity, state, email, title], (err, row) => {
        if (row === undefined)
            callback(uuid);
        else callback('error');
    });
}

function sendEmail(receiver, message, next) {

    var transporter = nodemailer.createTransport({
        service: 'Gmail',
        auth: {
            user: 'jrcm.emails@gmail.com', // Your email id
            pass: 'grandejefe1' // Your password
        },
        secure: false,
        tls: { rejectUnauthorized: false }
    });

    // setup e-mail data with unicode symbols
    var mailOptions = {
        from: '"TDIN - Projeto 2" <jrcm.emails@gmail.com>', // sender address
        to: receiver, // list of receivers
        subject: 'Encomenda feita', // Subject line
        text: message, // plaintext body
    };

    transporter.sendMail(mailOptions, function (error, info) {
        if (error) {
            next("error");
        }
        if (typeof next === 'function') {
            console.log('Message sent: ' + info.response);
            next('success');
        }
    });

}

module.exports = {
    placeOrder,
    getAllOrdersByEmail,
    updateOrderByWarehouse,
    updateOrderByStore,
    getOrder,
};