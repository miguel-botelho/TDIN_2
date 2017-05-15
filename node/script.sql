CREATE TABLE User(
    email TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    password TEXT NOT NULL,
    address TEXT NOT NULL
);

CREATE TABLE Book(
    title TEXT PRIMARY KEY,
    stock INTEGER,
    price INTEGER NOT NULL
);

CREATE TABLE Encomenda(
    uuid TEXT PRIMARY KEY,
    quantity INTEGER NOT NULL,
    state TEXT NOT NULL,
    email TEXT REFERENCES User(email),
    title TEXT REFERENCES Book(title)
);