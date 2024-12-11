DROP TABLE IF EXISTS Stock;
DROP TABLE IF EXISTS Cars;
DROP TABLE IF EXISTS Dealers;

-- Cars Table
CREATE TABLE Cars (
    CarID INTEGER PRIMARY KEY AUTOINCREMENT,
    Make TEXT NOT NULL,
    Model TEXT NOT NULL,
    Year INTEGER NOT NULL,
    Colours TEXT NOT NULL,
    Body TEXT NOT NULL,
    Transmission TEXT NOT NULL,
    FuelType TEXT NOT NULL,
    Seats INTEGER NOT NULL,
    Doors INTEGER NOT NULL
);
    
-- Dealers Table
CREATE TABLE Dealers (
    DealerID INTEGER PRIMARY KEY AUTOINCREMENT,
    DealerName TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL
);

-- Starting DealerIDs off at 1000
INSERT INTO sqlite_sequence (name, seq) VALUES ('Dealers', 999);

-- Sessions Table
CREATE TABLE Sessions (
    DealerID INTEGER PRIMARY KEY,
    JWTToken TEXT NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    FOREIGN KEY (DealerID) REFERENCES Dealers(DealerID)
);
    
-- Stock Table
CREATE TABLE Stock (
    CarID INTEGER NOT NULL,
    DealerID INTEGER NOT NULL,
    StockLevel INTEGER NOT NULL,
    PRIMARY KEY (CarID, DealerID),
    FOREIGN KEY (CarID) REFERENCES Cars (CarID),
    FOREIGN KEY (DealerID) REFERENCES Dealers (DealerID)
);
