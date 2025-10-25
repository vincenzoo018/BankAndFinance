-- Add profile_photo column to users table
-- Run this script to update the database schema

USE BFASDatabase;
GO

-- Check if column already exists
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'users' 
    AND COLUMN_NAME = 'profile_photo'
)
BEGIN
    ALTER TABLE users
    ADD profile_photo NVARCHAR(255) NULL;
    
    PRINT 'Column profile_photo added to users table successfully!';
END
ELSE
BEGIN
    PRINT 'Column profile_photo already exists in users table.';
END
GO

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'profile_photo';
GO

PRINT '';
PRINT '========================================';
PRINT 'Profile Photo Feature - Database Update Complete!';
PRINT '========================================';
PRINT '';
PRINT 'Features:';
PRINT '  - Users can now upload profile photos';
PRINT '  - Photos stored in /wwwroot/uploads/profiles/';
PRINT '  - Max file size: 5MB';
PRINT '  - Supported formats: JPG, PNG, GIF';
PRINT '';
PRINT 'Next steps:';
PRINT '  1. Ensure /wwwroot/uploads/profiles/ folder exists';
PRINT '  2. Login and go to Profile page';
PRINT '  3. Click Edit Profile';
PRINT '  4. Upload a profile photo';
PRINT '========================================';
GO
