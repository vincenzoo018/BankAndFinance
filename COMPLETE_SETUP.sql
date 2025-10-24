-- ===============================================
-- BFAS Complete Database Setup Script
-- Run this entire script in SSMS
-- ===============================================

USE master;
GO

-- Drop database if exists (WARNING: This deletes all data!)
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'BFASDatabase')
BEGIN
    ALTER DATABASE BFASDatabase SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BFASDatabase;
END
GO

-- Create database
CREATE DATABASE BFASDatabase;
GO

USE BFASDatabase;
GO

PRINT '✅ Database created successfully';
PRINT '';

-- ===============================================
-- CREATE TABLES
-- ===============================================

-- Roles Table
CREATE TABLE roles (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    role_name NVARCHAR(50) NOT NULL
);

-- Users Table
CREATE TABLE users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    role_id INT NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    profile_picture NVARCHAR(255),
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

-- Bank Accounts Table
CREATE TABLE bank_accounts (
    account_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    account_number NVARCHAR(50) NOT NULL UNIQUE,
    account_type NVARCHAR(20) DEFAULT 'Savings',
    balance DECIMAL(18,2) DEFAULT 0,
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Transactions Table
CREATE TABLE transactions (
    transaction_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    transaction_type NVARCHAR(50) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    transaction_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(20) DEFAULT 'Completed',
    reference_number NVARCHAR(50),
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id)
);

-- Customer Profiles Table
CREATE TABLE customer_profiles (
    profile_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    phone NVARCHAR(20),
    address NVARCHAR(255),
    date_of_birth DATE,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Billers Table
CREATE TABLE billers (
    biller_id INT PRIMARY KEY IDENTITY(1,1),
    biller_name NVARCHAR(100) NOT NULL,
    biller_type NVARCHAR(50) NOT NULL,
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE()
);

-- Bill Payments Table
CREATE TABLE bill_payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    biller_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    payment_date DATETIME DEFAULT GETDATE(),
    reference_number NVARCHAR(50),
    status NVARCHAR(20) DEFAULT 'Completed',
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id),
    FOREIGN KEY (biller_id) REFERENCES billers(biller_id)
);

-- Transfers Table
CREATE TABLE transfers (
    transfer_id INT PRIMARY KEY IDENTITY(1,1),
    sender_account_id INT NOT NULL,
    receiver_account_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    transfer_date DATETIME DEFAULT GETDATE(),
    reference_number NVARCHAR(50),
    status NVARCHAR(20) DEFAULT 'Completed',
    FOREIGN KEY (sender_account_id) REFERENCES bank_accounts(account_id),
    FOREIGN KEY (receiver_account_id) REFERENCES bank_accounts(account_id)
);

-- Audit Logs Table
CREATE TABLE audit_logs (
    log_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    action NVARCHAR(255) NOT NULL,
    module NVARCHAR(100) NOT NULL,
    timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Cards Table (NEW)
CREATE TABLE cards (
    card_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    card_number NVARCHAR(19) NOT NULL,
    card_type NVARCHAR(20) NOT NULL, -- Debit, Credit
    card_status NVARCHAR(20) DEFAULT 'Active',
    cvv NVARCHAR(4),
    expiry_date DATE,
    card_limit DECIMAL(18,2),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id)
);

-- Beneficiaries Table (NEW)
CREATE TABLE beneficiaries (
    beneficiary_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    beneficiary_name NVARCHAR(100) NOT NULL,
    account_number NVARCHAR(50) NOT NULL,
    bank_name NVARCHAR(100),
    nickname NVARCHAR(50),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Savings Goals Table (NEW)
CREATE TABLE savings_goals (
    goal_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    goal_name NVARCHAR(100) NOT NULL,
    target_amount DECIMAL(18,2) NOT NULL,
    current_amount DECIMAL(18,2) DEFAULT 0,
    target_date DATE,
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id)
);

-- Notifications Table (NEW)
CREATE TABLE notifications (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    title NVARCHAR(100) NOT NULL,
    message NVARCHAR(500) NOT NULL,
    type NVARCHAR(20), -- Info, Success, Warning, Error
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- QR Payments Table (NEW)
CREATE TABLE qr_payments (
    qr_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    qr_code NVARCHAR(255) NOT NULL,
    amount DECIMAL(18,2),
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    expires_at DATETIME,
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id)
);

-- Rewards Table (NEW)
CREATE TABLE rewards (
    reward_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    points INT DEFAULT 0,
    tier NVARCHAR(20) DEFAULT 'Bronze', -- Bronze, Silver, Gold, Platinum
    cashback_earned DECIMAL(18,2) DEFAULT 0,
    last_updated DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

PRINT '✅ All tables created successfully';
PRINT '';

-- ===============================================
-- SEED DATA
-- ===============================================

-- Insert Roles
INSERT INTO roles (role_name) VALUES 
    ('Admin'),
    ('Employee'),
    ('Customer');

PRINT '✅ Roles created';

-- Hash for 'admin123': jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
-- Hash for 'employee123': 6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=

-- Insert Admin
INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES (1, 'System Administrator', 'admin@bfas.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Active', GETDATE());

-- Insert Employees
INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES 
    (2, 'John Smith', 'employee1@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE()),
    (2, 'Sarah Johnson', 'employee2@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE()),
    (2, 'Michael Davis', 'employee3@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE());

PRINT '✅ Admin and Employees created';

-- Insert Sample Billers
INSERT INTO billers (biller_name, biller_type, status)
VALUES 
    ('Electric Company', 'Utilities', 'Active'),
    ('Water District', 'Utilities', 'Active'),
    ('Internet Provider', 'Telecommunications', 'Active'),
    ('Mobile Network', 'Telecommunications', 'Active'),
    ('Credit Card', 'Financial', 'Active'),
    ('Insurance Company', 'Insurance', 'Active');

PRINT '✅ Billers created';
PRINT '';
PRINT '========================================';
PRINT '✅ DATABASE SETUP COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT '📝 Login Credentials:';
PRINT '   Admin: admin@bfas.com / admin123';
PRINT '   Employee 1: employee1@bfas.com / employee123';
PRINT '   Employee 2: employee2@bfas.com / employee123';
PRINT '   Employee 3: employee3@bfas.com / employee123';
PRINT '';
PRINT '🚀 Next Steps:';
PRINT '   1. Run your ASP.NET Core application';
PRINT '   2. Login with admin credentials';
PRINT '   3. Register a customer account';
PRINT '========================================';
GO
