# 📝 How to Run COMPLETE_SETUP.sql - Visual Guide

## 🎯 You Have 3 Easy Options

---

## ✅ **OPTION 1: Visual Studio (EASIEST!)**

Since you already have the file open in Visual Studio:

### Step 1: Look at the top toolbar
```
You should see: [▶ Execute] button at the top
```

### Step 2: Click the green "Execute" button
- Or press `Ctrl + Shift + E`

### Step 3: If prompted for connection
```
Server name: (localdb)\MSSQLLocalDB
Authentication: Windows Authentication
```
Click **Connect**

### Step 4: Wait for success messages
```
✅ Database created successfully
✅ All tables created successfully
✅ Roles created
✅ Admin and Employees created
```

**DONE!** ✅

---

## ✅ **OPTION 2: SQL Server Management Studio (SSMS)**

### Step 1: Open SSMS
- Press `Windows Key`
- Type: `SQL Server Management Studio`
- Click to open

### Step 2: Connect to Server
```
Server name: (localdb)\MSSQLLocalDB
OR try: localhost
OR try: .\SQLEXPRESS

Authentication: Windows Authentication
```
Click **Connect**

### Step 3: Open the SQL file
1. Click **File** menu
2. Click **Open**
3. Click **File...**
4. Navigate to: `C:\Users\PC\source\repos\BankAndFinance\`
5. Select: `COMPLETE_SETUP.sql`
6. Click **Open**

### Step 4: Execute
- Press **F5**
- OR click the green **Execute** button

### Step 5: Check Results
Look at the **Messages** tab at the bottom:
```
✅ Database created successfully
✅ All tables created successfully
✅ Roles created
✅ Admin and Employees created
✅ Billers created

========================================
✅ DATABASE SETUP COMPLETE!
========================================

📝 Login Credentials:
   Admin: admin@bfas.com / admin123
   Employee 1: employee1@bfas.com / employee123
```

**DONE!** ✅

---

## ✅ **OPTION 3: Copy-Paste Method**

If options 1 & 2 don't work:

### Step 1: Select All Text
In Visual Studio with `COMPLETE_SETUP.sql` open:
- Press `Ctrl + A` (Select All)
- Press `Ctrl + C` (Copy)

### Step 2: Open SSMS
- Open SQL Server Management Studio
- Connect to your server
- Click **New Query** button (or press `Ctrl + N`)

### Step 3: Paste & Execute
- Press `Ctrl + V` (Paste)
- Press `F5` (Execute)

**DONE!** ✅

---

## 🔍 How to Verify It Worked

### Option A: In SSMS
1. Look at **Object Explorer** (left panel)
2. Expand **Databases**
3. You should see **BFASDatabase**
4. Expand it → Expand **Tables**
5. You should see all tables:
   - roles
   - users
   - bank_accounts
   - transactions
   - cards
   - beneficiaries
   - savings_goals
   - notifications
   - qr_payments
   - rewards
   - etc.

### Option B: Run a Test Query
```sql
USE BFASDatabase;
SELECT * FROM users;
```

You should see:
- 1 Admin account
- 3 Employee accounts

---

## ❌ Troubleshooting

### Issue: "Cannot connect to server"

**Try these server names one by one:**
1. `(localdb)\MSSQLLocalDB`
2. `localhost`
3. `.\SQLEXPRESS`
4. `.`
5. Your computer name: `YOUR-PC-NAME\SQLEXPRESS`

### Issue: "Database 'BFASDatabase' already exists"

**Solution 1:** The script will drop and recreate it automatically.

**Solution 2:** If it fails, manually drop first:
```sql
USE master;
DROP DATABASE BFASDatabase;
-- Then run COMPLETE_SETUP.sql again
```

### Issue: "SQL Server is not running"

**Solution:**
1. Press `Windows + R`
2. Type: `services.msc`
3. Press Enter
4. Find `SQL Server (MSSQLSERVER)` or `SQL Server (SQLEXPRESS)`
5. Right-click → Start

---

## 🎉 After Successful Execution

### Test Your Accounts

1. **Run your ASP.NET application** (Press F5 in Visual Studio)

2. **Login as Admin:**
   ```
   Email: admin@bfas.com
   Password: admin123
   ```

3. **Login as Employee:**
   ```
   Email: employee1@bfas.com
   Password: employee123
   ```

4. **Register a Customer Account:**
   - Click "Register"
   - Fill in the form
   - Login with your new credentials

---

## 📊 What the Script Does

1. **Drops existing database** (if any)
2. **Creates fresh BFASDatabase**
3. **Creates all tables:**
   - Core tables (users, accounts, transactions)
   - NEW: cards, beneficiaries, savings_goals
   - NEW: notifications, qr_payments, rewards
4. **Seeds data:**
   - 3 roles (Admin, Employee, Customer)
   - 1 admin account
   - 3 employee accounts
   - 6 sample billers

---

## ⏰ Time Required

- **Execution time:** 5-10 seconds
- **Your time:** 2-3 minutes (first time)

---

## 🚨 IMPORTANT NOTES

1. **Windows Authentication** is the default and easiest
2. **Backup existing data** if you have any (script drops database!)
3. **All passwords are hashed** for security
4. **Tables include profile_picture** field for future use

---

## 🎯 Next Steps After SQL Runs Successfully

1. ✅ Run your ASP.NET application
2. ✅ Login as admin
3. ✅ Test the new Windsurf colors (Already applied!)
4. ✅ Test responsive design on mobile
5. ✅ Start implementing features

---

## 💡 Pro Tip

**Save this server name** for future use:
```
The one that worked for you: ________________
```

Write it down so you don't have to test again!

---

**Ready? Let's run that SQL!** 🚀

Just pick ONE of the 3 options above and execute! You'll be done in 2 minutes!
