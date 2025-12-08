CREATE TABLE Products (
    ProductId UNIQUEIDENTIFIER PRIMARY KEY,
    ProductName NVARCHAR(255) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL
);

select Count(*) FROM Products
truncate table Products


CREATE TABLE Users (
    UserId        INT IDENTITY PRIMARY KEY,
    Username      NVARCHAR(100) NOT NULL UNIQUE,
    Email         NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash VARBINARY(256) NOT NULL,
    PasswordSalt VARBINARY(256) NOT NULL,
    IsActive     BIT NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE Roles (
    RoleId   INT IDENTITY PRIMARY KEY,
    Name     NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200)
);

CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
);


INSERT INTO Roles (Name) VALUES
('SuperAdmin'),
('User');