USE BFASDatabase;
GO

-- =============================================
-- Update cards table with new columns
-- =============================================
PRINT 'Updating cards table...';

-- Add balance column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'balance')
BEGIN
    ALTER TABLE cards ADD balance DECIMAL(18,2) NOT NULL DEFAULT 0;
    PRINT '✓ Added balance column';
END

-- Add is_primary column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'is_primary')
BEGIN
    ALTER TABLE cards ADD is_primary BIT NOT NULL DEFAULT 0;
    PRINT '✓ Added is_primary column';
END

-- Add updated_at column
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'cards' AND COLUMN_NAME = 'updated_at')
BEGIN
    ALTER TABLE cards ADD updated_at DATETIME NOT NULL DEFAULT GETDATE();
    PRINT '✓ Added updated_at column';
END
GO

-- =============================================
-- Create card_requests table
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'card_requests')
BEGIN
    CREATE TABLE card_requests (
        request_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        account_id INT NOT NULL,
        card_type NVARCHAR(20) NOT NULL DEFAULT 'Debit',
        requested_limit DECIMAL(18,2) NULL,
        reason NVARCHAR(500) NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        approved_by INT NULL,
        approval_date DATETIME NULL,
        rejection_reason NVARCHAR(500) NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (user_id) REFERENCES users(user_id),
        FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id),
        FOREIGN KEY (approved_by) REFERENCES users(user_id)
    );
    PRINT '✓ Created card_requests table';
END
GO

-- =============================================
-- Create card_transfers table
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'card_transfers')
BEGIN
    CREATE TABLE card_transfers (
        transfer_id INT PRIMARY KEY IDENTITY(1,1),
        from_card_id INT NOT NULL,
        to_card_id INT NOT NULL,
        amount DECIMAL(18,2) NOT NULL,
        description NVARCHAR(500) NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'Completed',
        transfer_date DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (from_card_id) REFERENCES cards(card_id),
        FOREIGN KEY (to_card_id) REFERENCES cards(card_id)
    );
    PRINT '✓ Created card_transfers table';
END
GO

-- =============================================
-- Create scheduled_payments table
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'scheduled_payments')
BEGIN
    CREATE TABLE scheduled_payments (
        schedule_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        card_id INT NOT NULL,
        biller_id INT NOT NULL,
        amount DECIMAL(18,2) NOT NULL,
        frequency NVARCHAR(20) NOT NULL DEFAULT 'Monthly',
        next_payment_date DATETIME NOT NULL,
        last_payment_date DATETIME NULL,
        status NVARCHAR(20) NOT NULL DEFAULT 'Active',
        description NVARCHAR(500) NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (user_id) REFERENCES users(user_id),
        FOREIGN KEY (card_id) REFERENCES cards(card_id),
        FOREIGN KEY (biller_id) REFERENCES billers(biller_id)
    );
    PRINT '✓ Created scheduled_payments table';
END
GO

-- =============================================
-- Create system_settings table
-- =============================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'system_settings')
BEGIN
    CREATE TABLE system_settings (
        setting_id INT PRIMARY KEY IDENTITY(1,1),
        setting_key NVARCHAR(100) NOT NULL UNIQUE,
        setting_value NVARCHAR(500) NOT NULL,
        setting_type NVARCHAR(50) NOT NULL DEFAULT 'String',
        description NVARCHAR(500) NULL,
        category NVARCHAR(100) NULL,
        updated_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_by INT NULL,
        FOREIGN KEY (updated_by) REFERENCES users(user_id)
    );
    PRINT '✓ Created system_settings table';
    
    -- Insert default settings
    INSERT INTO system_settings (setting_key, setting_value, setting_type, description, category) VALUES
    ('TransactionFee', '0.50', 'Decimal', 'Fee per transaction', 'Fees'),
    ('WithdrawalFee', '1.00', 'Decimal', 'Fee per withdrawal', 'Fees'),
    ('TransferFee', '2.00', 'Decimal', 'Fee per transfer', 'Fees'),
    ('MinimumBalance', '100.00', 'Decimal', 'Minimum account balance', 'Limits'),
    ('MaxDailyWithdrawal', '5000.00', 'Decimal', 'Maximum daily withdrawal limit', 'Limits'),
    ('InterestRate', '2.5', 'Decimal', 'Annual interest rate (%)', 'Interest'),
    ('CardRequestApprovalRequired', 'true', 'Boolean', 'Require admin approval for card requests', 'Settings');
    
    PRINT '✓ Inserted default system settings';
END
GO

-- Set the first card of each account as primary if no primary exists
UPDATE c1
SET c1.is_primary = 1
FROM cards c1
INNER JOIN (
    SELECT account_id, MIN(card_id) as first_card_id
    FROM cards
    WHERE card_status = 'Active'
    GROUP BY account_id
) c2 ON c1.account_id = c2.account_id AND c1.card_id = c2.first_card_id
WHERE NOT EXISTS (
    SELECT 1 FROM cards c3 
    WHERE c3.account_id = c1.account_id AND c3.is_primary = 1
);
PRINT '✓ Set primary cards';
GO

PRINT '';
PRINT '=========================================';
PRINT 'Database schema updated successfully!';
PRINT '=========================================';
PRINT '';
PRINT 'New tables created:';
PRINT '  ✓ card_requests';
PRINT '  ✓ card_transfers';
PRINT '  ✓ scheduled_payments';
PRINT '  ✓ system_settings';
PRINT '';
PRINT 'Cards table updated with:';
PRINT '  ✓ balance column';
PRINT '  ✓ is_primary column';
PRINT '  ✓ updated_at column';
PRINT '';
GO
