-- Run this script in SSMS to add the missing 'status' column to bank_accounts table
-- This will update your existing database without losing data

USE BFASDatabase;
GO

-- Check if the column exists before adding it
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID('bank_accounts') 
    AND name = 'status'
)
BEGIN
    -- Add status column to bank_accounts table
    ALTER TABLE bank_accounts
    ADD status NVARCHAR(20) DEFAULT 'Active';
    
    PRINT 'Successfully added status column to bank_accounts table';
END
ELSE
BEGIN
    PRINT 'Status column already exists in bank_accounts table';
END
GO

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'bank_accounts'
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Database update completed successfully!';
