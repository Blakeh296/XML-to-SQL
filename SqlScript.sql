	USE [master]
	GO

		PRINT ''

IF EXISTS (SELECT * FROM Sys.databases WHERE Name = 'ImportMockaroo') DROP DATABASE ImportMockaroo

		PRINT 'ImportMackaroo DROPPED'
												-- Create Database --
	CREATE DATABASE ImportMockaroo
	GO

		PRINT 'ImportMockaroo Created'
		PRINT ''

USE ImportMockaroo
GO
												-- Create A Table --
	CREATE TABLE Applicants (
		ApplicantID INT IDENTITY (1,1) NOT NULL,
		FirstName NVARCHAR(50) NOT NULL,
		LastName NVARCHAR(50) NOT NULL,
		SSN NVARCHAR(12) NOT NULL,
		Email NVARCHAR(50) NOT NULL,
		Gender NVARCHAR(10) NOT NULL,
		PRIMARY KEY (ApplicantID)	)
		GO 

		PRINT 'TBL Applicatns' 
		GO

												-- Create A Stored Procedure --
	CREATE PROCEDURE InsertApplicant
		@FirstName NVARCHAR(50),
		@LastName NVARCHAR(50),
		@SSN VARCHAR(12),
		@Email VARCHAR(50),
		@Gender VARCHAR(10),
		@AppID INT OUTPUT
	AS
	BEGIN
		INSERT INTO		Applicants
						(FirstName, LastName, SSN, Email, Gender)
		VALUES			(@FirstName, @LastName, @SSN, @Email, @Gender)
		SET @AppID = SCOPE_IDENTITY();
	END
	GO
		PRINT 'SP InsertApplicant'
