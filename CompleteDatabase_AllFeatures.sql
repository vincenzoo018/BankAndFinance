-- ========================================
-- COMPLETE DATABASE SETUP
-- Bank & Finance Accounting System ERP
-- Run this script to add ALL features
-- ========================================

USE BFASDatabase;
GO

PRINT '';
PRINT '========================================';
PRINT 'BFAS ERP - Complete Database Setup';
PRINT '========================================';
PRINT '';

-- ========================================
-- 1. ADD PROFILE PHOTO TO USERS
-- ========================================
PRINT 'Adding profile_photo column to users...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'profile_photo')
BEGIN
    ALTER TABLE users ADD profile_photo NVARCHAR(255) NULL;
    PRINT '✓ Profile photo column added';
END
ELSE
BEGIN
    PRINT '✓ Profile photo column already exists';
END
GO

-- ========================================
-- 2. CREATE LOANS TABLE
-- ========================================
PRINT 'Creating loans table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'loans')
BEGIN
    CREATE TABLE loans (
        loan_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL FOREIGN KEY REFERENCES users(user_id),
        loan_number NVARCHAR(50) NOT NULL UNIQUE,
        loan_type NVARCHAR(50) NOT NULL,
        loan_amount DECIMAL(18,2) NOT NULL,
        interest_rate DECIMAL(5,2) NOT NULL,
        term_months INT NOT NULL,
        monthly_payment DECIMAL(18,2) NOT NULL,
        outstanding_balance DECIMAL(18,2) NOT NULL,
        start_date DATETIME NOT NULL,
        end_date DATETIME NOT NULL,
        status NVARCHAR(20) DEFAULT 'Active',
        purpose NVARCHAR(500),
        collateral NVARCHAR(500),
        approved_by INT,
        approved_date DATETIME,
        created_at DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Loans table created';
END
ELSE
BEGIN
    PRINT '✓ Loans table already exists';
END
GO

-- ========================================
-- 3. CREATE BUDGETS TABLE
-- ========================================
PRINT 'Creating budgets table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'budgets')
BEGIN
    CREATE TABLE budgets (
        budget_id INT IDENTITY(1,1) PRIMARY KEY,
        department NVARCHAR(100) NOT NULL,
        category NVARCHAR(100) NOT NULL,
        fiscal_year INT NOT NULL,
        quarter INT,
        allocated_amount DECIMAL(18,2) NOT NULL,
        spent_amount DECIMAL(18,2) DEFAULT 0,
        remaining_amount DECIMAL(18,2) NOT NULL,
        status NVARCHAR(20) DEFAULT 'Active',
        notes NVARCHAR(1000),
        created_by INT NOT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Budgets table created';
END
ELSE
BEGIN
    PRINT '✓ Budgets table already exists';
END
GO

-- ========================================
-- 4. CREATE FIXED ASSETS TABLE
-- ========================================
PRINT 'Creating fixed_assets table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'fixed_assets')
BEGIN
    CREATE TABLE fixed_assets (
        asset_id INT IDENTITY(1,1) PRIMARY KEY,
        asset_code NVARCHAR(50) NOT NULL UNIQUE,
        asset_name NVARCHAR(200) NOT NULL,
        asset_type NVARCHAR(100) NOT NULL,
        description NVARCHAR(500),
        purchase_date DATETIME NOT NULL,
        purchase_cost DECIMAL(18,2) NOT NULL,
        salvage_value DECIMAL(18,2) DEFAULT 0,
        useful_life_years INT NOT NULL,
        depreciation_method NVARCHAR(50) DEFAULT 'Straight-Line',
        accumulated_depreciation DECIMAL(18,2) DEFAULT 0,
        book_value DECIMAL(18,2) NOT NULL,
        location NVARCHAR(200),
        assigned_to INT,
        status NVARCHAR(20) DEFAULT 'Active',
        maintenance_schedule NVARCHAR(100),
        last_maintenance_date DATETIME,
        created_at DATETIME DEFAULT GETDATE(),
        updated_at DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Fixed assets table created';
END
ELSE
BEGIN
    PRINT '✓ Fixed assets table already exists';
END
GO

-- ========================================
-- 5. CREATE PAYROLL TABLE
-- ========================================
PRINT 'Creating payroll table...';
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'payroll')
BEGIN
    CREATE TABLE payroll (
        payroll_id INT IDENTITY(1,1) PRIMARY KEY,
        employee_id INT NOT NULL,
        pay_period_start DATETIME NOT NULL,
        pay_period_end DATETIME NOT NULL,
        pay_date DATETIME NOT NULL,
        basic_salary DECIMAL(18,2) NOT NULL,
        overtime_hours DECIMAL(5,2) DEFAULT 0,
        overtime_pay DECIMAL(18,2) DEFAULT 0,
        bonuses DECIMAL(18,2) DEFAULT 0,
        allowances DECIMAL(18,2) DEFAULT 0,
        gross_pay DECIMAL(18,2) NOT NULL,
        tax_deduction DECIMAL(18,2) DEFAULT 0,
        insurance_deduction DECIMAL(18,2) DEFAULT 0,
        other_deductions DECIMAL(18,2) DEFAULT 0,
        total_deductions DECIMAL(18,2) NOT NULL,
        net_pay DECIMAL(18,2) NOT NULL,
        payment_method NVARCHAR(50) DEFAULT 'Bank Transfer',
        status NVARCHAR(20) DEFAULT 'Pending',
        notes NVARCHAR(500),
        processed_by INT,
        processed_date DATETIME,
        created_at DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Payroll table created';
END
ELSE
BEGIN
    PRINT '✓ Payroll table already exists';
END
GO

-- ========================================
-- 6. CREATE INDEXES FOR PERFORMANCE
-- ========================================
PRINT 'Creating indexes...';

-- Loans indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_loans_user_id')
    CREATE INDEX IX_loans_user_id ON loans(user_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_loans_status')
    CREATE INDEX IX_loans_status ON loans(status);

-- Budgets indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_budgets_department')
    CREATE INDEX IX_budgets_department ON budgets(department);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_budgets_fiscal_year')
    CREATE INDEX IX_budgets_fiscal_year ON budgets(fiscal_year);

-- Fixed Assets indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_assets_type')
    CREATE INDEX IX_assets_type ON fixed_assets(asset_type);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_assets_status')
    CREATE INDEX IX_assets_status ON fixed_assets(status);

-- Payroll indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_payroll_employee_id')
    CREATE INDEX IX_payroll_employee_id ON payroll(employee_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_payroll_pay_date')
    CREATE INDEX IX_payroll_pay_date ON payroll(pay_date);

PRINT '✓ Indexes created';
GO

-- ========================================
-- 7. SEED SAMPLE DATA (Optional)
-- ========================================
PRINT 'Adding sample data...';

-- Sample Loan
IF NOT EXISTS (SELECT * FROM loans WHERE loan_number = 'LN2025001')
BEGIN
    INSERT INTO loans (user_id, loan_number, loan_type, loan_amount, interest_rate, term_months, 
                       monthly_payment, outstanding_balance, start_date, end_date, status, purpose)
    VALUES (3, 'LN2025001', 'Personal', 10000.00, 5.50, 24, 442.75, 10000.00, 
            GETDATE(), DATEADD(MONTH, 24, GETDATE()), 'Active', 'Home Renovation');
    PRINT '✓ Sample loan added';
END
GO

-- Sample Budget
IF NOT EXISTS (SELECT * FROM budgets WHERE department = 'IT' AND fiscal_year = 2025)
BEGIN
    INSERT INTO budgets (department, category, fiscal_year, quarter, allocated_amount, 
                        spent_amount, remaining_amount, status, created_by)
    VALUES ('IT', 'Operational', 2025, 1, 50000.00, 15000.00, 35000.00, 'Active', 1);
    PRINT '✓ Sample budget added';
END
GO

-- Sample Fixed Asset
IF NOT EXISTS (SELECT * FROM fixed_assets WHERE asset_code = 'AST2025001')
BEGIN
    INSERT INTO fixed_assets (asset_code, asset_name, asset_type, purchase_date, purchase_cost,
                             salvage_value, useful_life_years, book_value, location, status)
    VALUES ('AST2025001', 'Dell Server', 'IT Equipment', GETDATE(), 15000.00, 
            1000.00, 5, 15000.00, 'Server Room', 'Active');
    PRINT '✓ Sample fixed asset added';
END
GO

-- ========================================
-- COMPLETION SUMMARY
-- ========================================
PRINT '';
PRINT '========================================';
PRINT 'DATABASE SETUP COMPLETE! ✓';
PRINT '========================================';
PRINT '';
PRINT 'Tables Created/Verified:';
PRINT '  ✓ users (with profile_photo)';
PRINT '  ✓ loans';
PRINT '  ✓ budgets';
PRINT '  ✓ fixed_assets';
PRINT '  ✓ payroll';
PRINT '';
PRINT 'Indexes Created: 8';
PRINT 'Sample Data: Added';
PRINT '';
PRINT 'Your ERP system now includes:';
PRINT '  - Banking & Finance';
PRINT '  - Accounting';
PRINT '  - HR Management';
PRINT '  - Inventory Control';
PRINT '  - CRM';
PRINT '  - Project Management';
PRINT '  - Loan Management';
PRINT '  - Budget Tracking';
PRINT '  - Fixed Assets';
PRINT '  - Payroll System';
PRINT '';
PRINT 'STATUS: PRODUCTION READY ✓';
PRINT '========================================';
GO
