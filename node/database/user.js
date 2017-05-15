const sqlite3 = require('sqlite3').verbose();
const db = new sqlite3.Database('database/database.db');

function createUser(name, password, address, email, callback) {
    const stmt = db.prepare('INSERT INTO User(email, name, password, address) VALUES(?,?,?,?);');
    stmt.get([email, name, password, address], (err, row) => {
        stmt.finalize();
        callback(row);
    });
}

module.exports = {
    createUser,
};
