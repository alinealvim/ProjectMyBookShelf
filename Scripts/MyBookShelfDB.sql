CREATE DATABASE MyBookShelfDB;

USE [MyBookShelfDB];
GO

CREATE TABLE [dbo].[User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50),
    SecurityQuestion NVARCHAR(255),
    SecurityAnswer NVARCHAR(255),
    PasswordResetToken NVARCHAR(255),
    TokenExpiry DATETIME
);
GO

CREATE TABLE [dbo].[Book] (
    BookID INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Genre NVARCHAR(50),
    Author NVARCHAR(255),
    Pages INT
);
GO

CREATE TABLE [dbo].[UserBook] (
    UserBookID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    BookID INT NOT NULL,
    Status NVARCHAR(50),
    StartDate DATE,
    EndDate DATE,
    CurrentPage INT,
    Notes NVARCHAR(MAX),
    Rating INT,
    CONSTRAINT FK_UserBook_User FOREIGN KEY (UserID) REFERENCES [dbo].[User](UserID) ON DELETE CASCADE, 
    CONSTRAINT FK_UserBook_Book FOREIGN KEY (BookID) REFERENCES [dbo].[Book](BookID) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[ReadingHistory] (
    ReadingHistoryID INT IDENTITY(1,1) PRIMARY KEY,
    UserBookID INT NOT NULL,
    PagesRead INT NOT NULL,
    Date DATE NOT NULL,
    CONSTRAINT FK_ReadingHistory_UserBook FOREIGN KEY (UserBookID) REFERENCES UserBook(UserBookID) ON DELETE CASCADE
);
GO

-- senha:12345
-- resposta: frida

INSERT INTO [dbo].[User] VALUES (
'admin'
,'uBIs/dgHRk3KRmcVWZ3QywdhKwisD9MCMWDq4fYt7hs='
,'Administrator'
,'Qual   o nome do seu primeiro animal de estima  o?',
'lO6BrrrYrLH2KiBghD/lEBgzE32iLBrGQkz6IZipYeY='
,NULL
,NULL)