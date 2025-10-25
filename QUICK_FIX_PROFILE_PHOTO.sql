-- QUICK FIX: Run this immediately in SQL Server Management Studio
-- This will fix the "Invalid column name 'profile_photo'" error

USE BFASDatabase;
GO

-- Add profile_photo column
ALTER TABLE users
ADD profile_photo NVARCHAR(255) NULL;
GO

PRINT 'Profile photo column added successfully!';
PRINT 'You can now login without errors.';
GO
