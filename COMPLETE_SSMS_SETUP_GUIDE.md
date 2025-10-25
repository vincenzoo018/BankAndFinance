# 🗄️ COMPLETE SQL SERVER SETUP GUIDE

## 📋 **STEP-BY-STEP INSTRUCTIONS**

This guide will help you set up your entire database from scratch in SQL Server Management Studio (SSMS).

---

## ✅ **STEP 1: OPEN SQL SERVER MANAGEMENT STUDIO**

1. Open **SQL Server Management Studio**
2. Connect to your SQL Server instance
3. You're ready to go!

---

## ✅ **STEP 2: CREATE DATABASE & TABLES**

### **Run Script 1**: `CreateDatabase.sql`

**What it does**:
- Creates `BFASDatabase` database
- Creates all core tables (users, roles, accounts, transactions, etc.)
- Sets up relationships between tables

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `CreateDatabase.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Wait for completion
6. Should see: "Command(s) completed successfully"

**What gets created**:
- ✅ roles table
- ✅ users table
- ✅ bank_accounts table
- ✅ transactions table
- ✅ transfers table
- ✅ billers table
- ✅ payments table
- ✅ accounts_payable table
- ✅ accounts_receivable table
- ✅ And more...

---

## ✅ **STEP 3: ADD PROFILE PHOTO COLUMN**

### **Run Script 2**: `UpdateDatabase_AddProfilePhoto.sql`

**What it does**:
- Adds `profile_photo` column to users table
- Allows users to upload profile pictures

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `UpdateDatabase_AddProfilePhoto.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Should see: "Command(s) completed successfully"

**Result**:
- ✅ profile_photo column added to users table

---

## ✅ **STEP 4: ADD PAYMENT MODE**

### **Run Script 3**: `UpdateDatabase_AddPaymentMode.sql`

**What it does**:
- Adds payment mode options
- Enables different payment methods (Bank Transfer, Check, Cash, etc.)

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `UpdateDatabase_AddPaymentMode.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Should see: "Command(s) completed successfully"

**Result**:
- ✅ Payment modes configured

---

## ✅ **STEP 5: ADD STATUS FIELD**

### **Run Script 4**: `UpdateDatabase_AddStatus.sql`

**What it does**:
- Adds status field to various tables
- Tracks active/inactive status of records

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `UpdateDatabase_AddStatus.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Should see: "Command(s) completed successfully"

**Result**:
- ✅ Status fields added

---

## ✅ **STEP 6: ADD ALL ADVANCED FEATURES**

### **Run Script 5**: `CompleteDatabase_AllFeatures.sql`

**What it does**:
- Adds Loan Management tables
- Adds Budget Tracking tables
- Adds Fixed Assets tables
- Adds Payroll System tables
- Creates all indexes for performance
- Adds sample data for testing

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `CompleteDatabase_AllFeatures.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Wait for completion (may take 30 seconds)
6. Should see: "DATABASE SETUP COMPLETE! ✓"

**What gets created**:
- ✅ loans table
- ✅ budgets table
- ✅ fixed_assets table
- ✅ payroll table
- ✅ All indexes
- ✅ Sample data

---

## ✅ **STEP 7: ADD CUSTOMER ERP FEATURES**

### **Run Script 6**: `NEW_CUSTOMER_ERP_FEATURES.sql`

**What it does**:
- Adds Loan Applications table
- Adds Investments table
- Adds Insurance table
- Adds Statements table
- Adds Credit Score table
- Creates indexes
- Adds sample data

**How to run**:
1. Click **"New Query"** button
2. Copy entire contents of `NEW_CUSTOMER_ERP_FEATURES.sql`
3. Paste into query window
4. Click **"Execute"** (F5)
5. Wait for completion
6. Should see: "CUSTOMER ERP FEATURES - COMPLETE! ✓"

**What gets created**:
- ✅ loan_applications table
- ✅ investments table
- ✅ insurances table
- ✅ statements table
- ✅ credit_scores table
- ✅ All indexes
- ✅ Sample data

---

## ✅ **STEP 8: CREATE LOGIN ACCOUNTS**

### **Run Script 7**: Create Admin & Employee Accounts

**Create New Query and run**:

```sql
USE BFASDatabase;
GO

-- Insert Roles
INSERT INTO roles (role_name) VALUES ('Admin');
INSERT INTO roles (role_name) VALUES ('Employee');
INSERT INTO roles (role_name) VALUES ('Customer');
GO

-- Insert Admin Account
INSERT INTO users (role_id, full_name, email, password, status, created_at)
SELECT role_id, 'Administrator', 'admin@bfas.com', 'admin123', 'Active', GETDATE()
FROM roles WHERE role_name = 'Admin';
GO

-- Insert Employee Account
INSERT INTO users (role_id, full_name, email, password, status, created_at)
SELECT role_id, 'Employee One', 'employee1@bfas.com', 'employee123', 'Active', GETDATE()
FROM roles WHERE role_name = 'Employee';
GO

-- Create sample customer account
INSERT INTO users (role_id, full_name, email, password, status, created_at)
SELECT role_id, 'John Doe', 'john@example.com', 'password123', 'Active', GETDATE()
FROM roles WHERE role_name = 'Customer';
GO

-- Create bank account for customer
INSERT INTO bank_accounts (user_id, account_number, account_type, balance, status, created_at)
VALUES (3, '1234567890', 'Savings', 5000.00, 'Active', GETDATE());
GO

PRINT 'Accounts created successfully!';
PRINT 'Admin: admin@bfas.com / admin123';
PRINT 'Employee: employee1@bfas.com / employee123';
PRINT 'Customer: john@example.com / password123';
GO
```

**How to run**:
1. Click **"New Query"** button
2. Paste the code above
3. Click **"Execute"** (F5)
4. Should see: "Accounts created successfully!"

**Accounts created**:
- ✅ Admin: `admin@bfas.com` / `admin123`
- ✅ Employee: `employee1@bfas.com` / `employee123`
- ✅ Customer: `john@example.com` / `password123`

---

## ✅ **STEP 9: VERIFY EVERYTHING**

### **Run Verification Query**:

```sql
USE BFASDatabase;
GO

-- Check database exists
SELECT 'Database' AS Item, DB_NAME() AS Value;

-- Count tables
SELECT 'Tables' AS Item, COUNT(*) AS Value 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Count users
SELECT 'Users' AS Item, COUNT(*) AS Value FROM users;

-- Count roles
SELECT 'Roles' AS Item, COUNT(*) AS Value FROM roles;

-- List all users
PRINT '';
PRINT 'All Users:';
SELECT user_id, full_name, email, r.role_name, status 
FROM users u
JOIN roles r ON u.role_id = r.role_id
ORDER BY user_id;

-- List all tables
PRINT '';
PRINT 'All Tables:';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;
GO
```

**How to run**:
1. Click **"New Query"** button
2. Paste the code above
3. Click **"Execute"** (F5)
4. Review results

**Expected output**:
```
Item        Value
Database    BFASDatabase
Tables      25+ (or more)
Users       3 (Admin, Employee, Customer)
Roles       3 (Admin, Employee, Customer)
```

---

## 🎯 **COMPLETE SETUP CHECKLIST**

Use this checklist to track your progress:

- [ ] **Step 1**: Open SSMS
- [ ] **Step 2**: Run `CreateDatabase.sql` ✅
- [ ] **Step 3**: Run `UpdateDatabase_AddProfilePhoto.sql` ✅
- [ ] **Step 4**: Run `UpdateDatabase_AddPaymentMode.sql` ✅
- [ ] **Step 5**: Run `UpdateDatabase_AddStatus.sql` ✅
- [ ] **Step 6**: Run `CompleteDatabase_AllFeatures.sql` ✅
- [ ] **Step 7**: Run `NEW_CUSTOMER_ERP_FEATURES.sql` ✅
- [ ] **Step 8**: Create login accounts ✅
- [ ] **Step 9**: Verify everything ✅

---

## 🚀 **AFTER DATABASE SETUP**

Once all scripts are run successfully:

1. **Go back to Visual Studio**
2. **Build solution** (Ctrl+Shift+B)
3. **Press F5** to run application
4. **Login with**:
   - Admin: `admin@bfas.com` / `admin123`
   - Employee: `employee1@bfas.com` / `employee123`
   - Customer: `john@example.com` / `password123`

---

## ⚠️ **TROUBLESHOOTING**

### **Error: "Database already exists"**
```sql
-- Drop existing database
USE master;
GO
DROP DATABASE BFASDatabase;
GO

-- Then run CreateDatabase.sql again
```

### **Error: "Column already exists"**
```sql
-- Check if column exists
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'profile_photo';

-- If it exists, skip that script
```

### **Error: "Foreign key constraint"**
- Make sure you run scripts in order
- Don't skip any steps
- Run verification query to check

### **Error: "Login failed"**
- Make sure you created user accounts (Step 8)
- Check email and password are correct
- Verify user status is 'Active'

---

## 📊 **WHAT YOU'LL HAVE AFTER SETUP**

### **Database Tables**: 25+
- Core banking tables
- Accounting tables
- ERP tables
- Customer feature tables

### **User Accounts**: 3
- 1 Admin account
- 1 Employee account
- 1 Customer account

### **Features**: 35+
- Banking operations
- Accounting
- HR Management
- Inventory
- CRM
- Projects
- Loans
- Investments
- Insurance
- Credit Scores
- And more!

### **Sample Data**: Included
- Sample transactions
- Sample loans
- Sample investments
- Sample insurance
- Sample credit scores

---

## 💡 **TIPS**

1. **Run scripts one at a time** - Don't run multiple scripts at once
2. **Wait for completion** - Each script takes a few seconds
3. **Check for errors** - Read the messages in SSMS output
4. **Keep scripts open** - You might need to run them again
5. **Take screenshots** - Document your setup process

---

## ✅ **YOU'RE READY!**

After following all 9 steps:

✅ Database created  
✅ All tables created  
✅ All features added  
✅ Login accounts created  
✅ Sample data added  
✅ Everything verified  

**Your complete Banking ERP is ready to use!** 🏦💰

---

## 📞 **NEED HELP?**

If you get stuck:
1. Check the error message carefully
2. Review the troubleshooting section
3. Make sure you're running scripts in order
4. Verify each step before moving to next

**Everything should work smoothly!** ✅
