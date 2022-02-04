CREATE DATABASE Helperlend_Schema

USE Helperlend_Schema

GO

CREATE SCHEMA DATABASE_SCHEMA
GO

CREATE TABLE DATABASE_SCHEMA.Customer_Details(
	Customer_Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	C_FirstName varchar(50),
	C_LastName varchar(50),
	C_SurName varchar(50),
	C_Email varchar(50) NOT NULL,
	C_Mobile int NOT NULL,
	C_Password varchar(50) NOT NULL,
)

CREATE TABLE DATABASE_SCHEMA.ServiceProvider_Details(
	ServiceProvider_Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	S_ActiveStatus varchar(50),
	S_FirstName varchar(50) NOT NULL,
	S_LastName varchar(50) NOT NULL,
	S_Email varchar(50),
	S_Mobile int,
	S_Password varchar(50),
	S_DateOfBirth date NOT NULL,
	S_Gender varchar(50),
	S_Nationality varchar(50),
	S_Avtar int
)

CREATE TABLE DATABASE_SCHEMA.Address_Details(
	Address_Id int PRIMARY KEY  NOT NULL,
	Customer_Id int FOREIGN KEY REFERENCES DATABASE_SCHEMA.Customer_Details (Customer_Id),
	ServiceProvider_Id int FOREIGN KEY REFERENCES DATABASE_SCHEMA.ServiceProvider_Details (ServiceProvider_Id),
	StreetName nvarchar(50) NOT NULL,
	HouseNumber int NOT NULL,
	PostalCode int NOT NULL,
	City varchar(50) NOT NULL,
	Mobile int NOT NULL,
)


CREATE TABLE DATABASE_SCHEMA.Book_Services(
	BookService_Id int NOT NULL,
	Customer_Id int FOREIGN KEY REFERENCES  DATABASE_SCHEMA.Customer_Details (Customer_Id),
	PostalCode int NOT NULL,
	CleanerDate date NOT NULL,
	CleanerTime time(7) NOT NULL,
	CleanerDuration time(7) NOT NULL,
	ExtraService int NOT NULL,
	Comment nvarchar(50) NOT NULL,
	Have_Pet int,
	Payment_Amount int NOT NULL,
	Address_Id int FOREIGN KEY REFERENCES DATABASE_SCHEMA.Address_Details(Address_Id),
)


CREATE TABLE DATABASE_SCHEMA.Contact_Us(
	Contact_Id int NOT NULL,
	FirstName varchar(50) NULL,
	LastName varchar(50) NULL,
	Email varchar(50) NOT NULL,
	Mobile int NULL,
	Subject text NOT NULL,
	Message text NULL,
)