-- Run this script in SSMS if migrations fail
-- This will create the database and all tables manually

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BFASDatabase')
BEGIN
    CREATE DATABASE BFASDatabase;
END
GO

USE BFASDatabase;
GO

-- Create tables
CREATE TABLE roles (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    role_name NVARCHAR(50) NOT NULL
);

CREATE TABLE users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    role_id INT NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

CREATE TABLE bank_accounts (
    account_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    account_number NVARCHAR(50) NOT NULL,
    account_type NVARCHAR(20) DEFAULT 'Savings',
    balance DECIMAL(18,2) DEFAULT 0,
    status NVARCHAR(20) DEFAULT 'Active',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

CREATE TABLE transactions (
    transaction_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    transaction_type NVARCHAR(50) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    transaction_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(20) DEFAULT 'Pending',
    reference_number NVARCHAR(50),
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id)
);

CREATE TABLE transfers (
    transfer_id INT PRIMARY KEY IDENTITY(1,1),
    sender_account_id INT NOT NULL,
    receiver_account_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    transfer_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (sender_account_id) REFERENCES bank_accounts(account_id),
    FOREIGN KEY (receiver_account_id) REFERENCES bank_accounts(account_id)
);

CREATE TABLE billers (
    biller_id INT PRIMARY KEY IDENTITY(1,1),
    biller_name NVARCHAR(100) NOT NULL,
    biller_type NVARCHAR(50),
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE payments (
    payment_id INT PRIMARY KEY IDENTITY(1,1),
    account_id INT NOT NULL,
    biller_id INT NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    payment_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(20) DEFAULT 'Pending',
    reference_number NVARCHAR(50),
    FOREIGN KEY (account_id) REFERENCES bank_accounts(account_id),
    FOREIGN KEY (biller_id) REFERENCES billers(biller_id)
);

CREATE TABLE accounts_payable (
    ap_id INT PRIMARY KEY IDENTITY(1,1),
    payee_name NVARCHAR(100) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    due_date DATETIME NOT NULL,
    status NVARCHAR(20) DEFAULT 'Pending',
    created_by INT NOT NULL,
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

CREATE TABLE accounts_receivable (
    ar_id INT PRIMARY KEY IDENTITY(1,1),
    payer_name NVARCHAR(100) NOT NULL,
    amount DECIMAL(18,2) NOT NULL,
    due_date DATETIME NOT NULL,
    status NVARCHAR(20) DEFAULT 'Pending',
    created_by INT NOT NULL,
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

CREATE TABLE journal_entries (
    journal_id INT PRIMARY KEY IDENTITY(1,1),
    date DATETIME DEFAULT GETDATE(),
    description NVARCHAR(500) NOT NULL,
    reference_number NVARCHAR(50),
    created_by INT NOT NULL,
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);

CREATE TABLE journal_entry_lines (
    line_id INT PRIMARY KEY IDENTITY(1,1),
    journal_id INT NOT NULL,
    account_name NVARCHAR(100) NOT NULL,
    debit DECIMAL(18,2) DEFAULT 0,
    credit DECIMAL(18,2) DEFAULT 0,
    FOREIGN KEY (journal_id) REFERENCES journal_entries(journal_id)
);

CREATE TABLE ledger_accounts (
    ledger_id INT PRIMARY KEY IDENTITY(1,1),
    account_name NVARCHAR(100) NOT NULL,
    balance DECIMAL(18,2) DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE trial_balance (
    trial_id INT PRIMARY KEY IDENTITY(1,1),
    account_name NVARCHAR(100) NOT NULL,
    debit DECIMAL(18,2) DEFAULT 0,
    credit DECIMAL(18,2) DEFAULT 0,
    period NVARCHAR(50)
);

CREATE TABLE financial_statements (
    fs_id INT PRIMARY KEY IDENTITY(1,1),
    statement_type NVARCHAR(50) NOT NULL,
    period NVARCHAR(50),
    total_amount DECIMAL(18,2) DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE audit_logs (
    log_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    action NVARCHAR(100) NOT NULL,
    module NVARCHAR(100),
    timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

CREATE TABLE branch (
    branch_id INT PRIMARY KEY IDENTITY(1,1),
    branch_name NVARCHAR(100) NOT NULL,
    location NVARCHAR(255)
);

CREATE TABLE account_number_generator (
    id INT PRIMARY KEY IDENTITY(1,1),
    last_number BIGINT DEFAULT 100000000000
);

CREATE TABLE customer_profiles (
    profile_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    address NVARCHAR(255),
    phone NVARCHAR(20),
    date_of_birth DATETIME,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Insert seed data
INSERT INTO roles (role_name) VALUES ('Admin'), ('Employee'), ('Customer');

INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES 
    (1, 'Admin User', 'admin@bfassystem.com', 'Admin123', 'Active', '2025-10-20'),
    (2, 'Employee User', 'employee@bfassystem.com', 'Emp123', 'Active', '2025-10-20'),
    (3, 'Customer One', 'customer1@mail.com', 'Cust123', 'Active', '2025-10-20');

INSERT INTO account_number_generator (last_number) VALUES (100000000000);

INSERT INTO bank_accounts (user_id, account_number, account_type, balance, created_at)
VALUES (3, 'BFAS-100000000001', 'Savings', 5000, '2025-10-20');

INSERT INTO billers (biller_name, biller_type, created_at)
VALUES 
    ('Electric Company', 'Utility', GETDATE()),
    ('Water Company', 'Utility', GETDATE()),
    ('Internet Provider', 'Telecom', GETDATE());

INSERT INTO branch (branch_name, location)
VALUES 
    ('Main Branch', 'Downtown City Center'),
    ('North Branch', 'North District');

GO

PRINT 'Database created successfully!';
