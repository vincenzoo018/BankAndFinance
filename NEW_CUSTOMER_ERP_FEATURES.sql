-- ========================================
-- CUSTOMER ERP FEATURES - DATABASE SETUP
-- Advanced Banking Features for Customers
-- ========================================

USE BFASDatabase;
GO

PRINT '';
PRINT '========================================';
PRINT 'CUSTOMER ERP FEATURES - Setup';
PRINT '========================================';
PRINT '';

-- ========================================
-- 1. LOAN APPLICATIONS TABLE
-- ========================================
PRINT 'Creating loan_applications table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'loan_applications')
BEGIN
    CREATE TABLE loan_applications (
        application_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        application_number NVARCHAR(50) NOT NULL UNIQUE,
        loan_type NVARCHAR(50) NOT NULL,
        requested_amount DECIMAL(18,2) NOT NULL,
        purpose NVARCHAR(1000) NOT NULL,
        employment_status NVARCHAR(50),
        monthly_income DECIMAL(18,2),
        credit_score INT,
        collateral_description NVARCHAR(1000),
        collateral_value DECIMAL(18,2),
        term_months INT NOT NULL,
        status NVARCHAR(20) DEFAULT 'Pending',
        application_date DATETIME DEFAULT GETDATE(),
        reviewed_by INT,
        review_date DATETIME,
        review_notes NVARCHAR(1000),
        approved_amount DECIMAL(18,2),
        interest_rate DECIMAL(5,2)
    );
    PRINT '✓ Loan applications table created';
END
ELSE PRINT '✓ Loan applications table already exists';
GO

-- ========================================
-- 2. INVESTMENTS TABLE
-- ========================================
PRINT 'Creating investments table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'investments')
BEGIN
    CREATE TABLE investments (
        investment_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        investment_number NVARCHAR(50) NOT NULL UNIQUE,
        investment_type NVARCHAR(50) NOT NULL,
        investment_name NVARCHAR(200) NOT NULL,
        principal_amount DECIMAL(18,2) NOT NULL,
        current_value DECIMAL(18,2) NOT NULL,
        expected_return_rate DECIMAL(5,2) NOT NULL,
        actual_return_rate DECIMAL(5,2),
        purchase_date DATETIME NOT NULL,
        maturity_date DATETIME,
        risk_level NVARCHAR(20) DEFAULT 'Medium',
        status NVARCHAR(20) DEFAULT 'Active',
        dividends_earned DECIMAL(18,2) DEFAULT 0,
        last_updated DATETIME DEFAULT GETDATE(),
        notes NVARCHAR(500)
    );
    PRINT '✓ Investments table created';
END
ELSE PRINT '✓ Investments table already exists';
GO

-- ========================================
-- 3. INSURANCES TABLE
-- ========================================
PRINT 'Creating insurances table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'insurances')
BEGIN
    CREATE TABLE insurances (
        insurance_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        policy_number NVARCHAR(50) NOT NULL UNIQUE,
        insurance_type NVARCHAR(50) NOT NULL,
        policy_name NVARCHAR(200) NOT NULL,
        coverage_amount DECIMAL(18,2) NOT NULL,
        premium_amount DECIMAL(18,2) NOT NULL,
        premium_frequency NVARCHAR(20) DEFAULT 'Monthly',
        start_date DATETIME NOT NULL,
        end_date DATETIME NOT NULL,
        next_payment_date DATETIME NOT NULL,
        status NVARCHAR(20) DEFAULT 'Active',
        beneficiary_name NVARCHAR(200),
        beneficiary_relationship NVARCHAR(50),
        total_paid DECIMAL(18,2) DEFAULT 0,
        claims_filed INT DEFAULT 0,
        last_claim_date DATETIME,
        notes NVARCHAR(1000),
        created_at DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Insurances table created';
END
ELSE PRINT '✓ Insurances table already exists';
GO

-- ========================================
-- 4. STATEMENTS TABLE
-- ========================================
PRINT 'Creating statements table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'statements')
BEGIN
    CREATE TABLE statements (
        statement_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        account_id INT NOT NULL FOREIGN KEY REFERENCES bank_accounts(account_id),
        statement_number NVARCHAR(50) NOT NULL UNIQUE,
        statement_type NVARCHAR(50) DEFAULT 'Monthly',
        period_start DATETIME NOT NULL,
        period_end DATETIME NOT NULL,
        opening_balance DECIMAL(18,2) NOT NULL,
        closing_balance DECIMAL(18,2) NOT NULL,
        total_deposits DECIMAL(18,2) NOT NULL,
        total_withdrawals DECIMAL(18,2) NOT NULL,
        transaction_count INT NOT NULL,
        interest_earned DECIMAL(18,2) DEFAULT 0,
        fees_charged DECIMAL(18,2) DEFAULT 0,
        generated_date DATETIME DEFAULT GETDATE(),
        file_path NVARCHAR(500),
        is_downloaded BIT DEFAULT 0,
        download_count INT DEFAULT 0,
        last_download_date DATETIME
    );
    PRINT '✓ Statements table created';
END
ELSE PRINT '✓ Statements table already exists';
GO

-- ========================================
-- 5. CREDIT SCORES TABLE
-- ========================================
PRINT 'Creating credit_scores table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'credit_scores')
BEGIN
    CREATE TABLE credit_scores (
        score_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        score INT NOT NULL,
        score_date DATETIME DEFAULT GETDATE(),
        credit_rating NVARCHAR(20) NOT NULL,
        payment_history_score INT NOT NULL,
        credit_utilization DECIMAL(5,2) NOT NULL,
        credit_age_months INT NOT NULL,
        total_accounts INT NOT NULL,
        hard_inquiries INT NOT NULL,
        derogatory_marks INT NOT NULL,
        total_debt DECIMAL(18,2) NOT NULL,
        available_credit DECIMAL(18,2) NOT NULL,
        previous_score INT,
        score_change INT DEFAULT 0,
        factors_affecting NVARCHAR(1000),
        recommendations NVARCHAR(1000),
        next_update_date DATETIME NOT NULL
    );
    PRINT '✓ Credit scores table created';
END
ELSE PRINT '✓ Credit scores table already exists';
GO

-- ========================================
-- 6. CREATE INDEXES
-- ========================================
PRINT 'Creating indexes...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_loan_applications_user_id')
    CREATE INDEX IX_loan_applications_user_id ON loan_applications(user_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_investments_user_id')
    CREATE INDEX IX_investments_user_id ON investments(user_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_insurances_user_id')
    CREATE INDEX IX_insurances_user_id ON insurances(user_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_statements_user_id')
    CREATE INDEX IX_statements_user_id ON statements(user_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_credit_scores_user_id')
    CREATE INDEX IX_credit_scores_user_id ON credit_scores(user_id);

PRINT '✓ Indexes created';
GO

-- ========================================
-- 7. SAMPLE DATA FOR TESTING
-- ========================================
PRINT 'Adding sample data...';

-- Sample Loan Application (assuming user_id 3 exists)
IF NOT EXISTS (SELECT * FROM loan_applications WHERE application_number = 'LA2025001')
BEGIN
    INSERT INTO loan_applications (user_id, application_number, loan_type, requested_amount, 
                                   purpose, employment_status, monthly_income, credit_score, 
                                   term_months, status)
    VALUES (3, 'LA2025001', 'Personal', 50000.00, 'Home Renovation', 
            'Employed', 8000.00, 720, 36, 'Pending');
    PRINT '✓ Sample loan application added';
END
GO

-- Sample Investment
IF NOT EXISTS (SELECT * FROM investments WHERE investment_number = 'INV2025001')
BEGIN
    INSERT INTO investments (user_id, investment_number, investment_type, investment_name,
                            principal_amount, current_value, expected_return_rate, 
                            purchase_date, risk_level, status)
    VALUES (3, 'INV2025001', 'Mutual Fund', 'Growth Fund Plus', 
            10000.00, 10500.00, 12.50, GETDATE(), 'Medium', 'Active');
    PRINT '✓ Sample investment added';
END
GO

-- Sample Insurance
IF NOT EXISTS (SELECT * FROM insurances WHERE policy_number = 'POL2025001')
BEGIN
    INSERT INTO insurances (user_id, policy_number, insurance_type, policy_name,
                           coverage_amount, premium_amount, premium_frequency,
                           start_date, end_date, next_payment_date, status)
    VALUES (3, 'POL2025001', 'Life', 'Life Protection Plan', 
            500000.00, 500.00, 'Monthly', GETDATE(), 
            DATEADD(YEAR, 20, GETDATE()), DATEADD(MONTH, 1, GETDATE()), 'Active');
    PRINT '✓ Sample insurance added';
END
GO

-- Sample Credit Score
IF NOT EXISTS (SELECT * FROM credit_scores WHERE user_id = 3)
BEGIN
    INSERT INTO credit_scores (user_id, score, credit_rating, payment_history_score,
                              credit_utilization, credit_age_months, total_accounts,
                              hard_inquiries, derogatory_marks, total_debt, available_credit,
                              next_update_date)
    VALUES (3, 750, 'Excellent', 95, 25.50, 48, 5, 1, 0, 15000.00, 35000.00, 
            DATEADD(MONTH, 1, GETDATE()));
    PRINT '✓ Sample credit score added';
END
GO

-- ========================================
-- COMPLETION
-- ========================================
PRINT '';
PRINT '========================================';
PRINT 'CUSTOMER ERP FEATURES - COMPLETE! ✓';
PRINT '========================================';
PRINT '';
PRINT 'New Tables Created:';
PRINT '  ✓ loan_applications';
PRINT '  ✓ investments';
PRINT '  ✓ insurances';
PRINT '  ✓ statements';
PRINT '  ✓ credit_scores';
PRINT '';
PRINT 'Customer Features Now Available:';
PRINT '  ✓ Apply for Loans';
PRINT '  ✓ Investment Portfolio';
PRINT '  ✓ Insurance Management';
PRINT '  ✓ Account Statements';
PRINT '  ✓ Credit Score Tracking';
PRINT '';
PRINT 'Sample Data: Added for testing';
PRINT 'Indexes: Created for performance';
PRINT '';
PRINT 'STATUS: READY FOR USE! ✓';
PRINT '========================================';
GO
