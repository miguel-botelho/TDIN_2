const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('database/database.db');

function createBook(title, stock, price, callback) {
    const stmt = db.prepare('INSERT INTO Book (title, stock, price) VALUES(?,?,?);');
    stmt.get([title, stock, price], (err, row) => {
        callback(row);
    });
}

function getAllBooks(callback) {
    const stmt = db.prepare('SELECT * FROM Book;');
    stmt.all((err, rows) => {
        callback(rows);
    });
}

module.exports = {
    createBook,
    getAllBooks,
};
