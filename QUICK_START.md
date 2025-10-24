# ⚡ BFAS Quick Start Guide

## 🎯 Get Your System Running in 3 Steps!

---

## Step 1: Create the Database ⏱️ (2 minutes)

### Open Package Manager Console
In Visual Studio:
- **View** → **Other Windows** → **Package Manager Console**
- Or: **Tools** → **NuGet Package Manager** → **Package Manager Console**

### Run These Commands:
```powershell
Add-Migration InitialCreate
Update-Database
```

**What happens?**
- Creates `BFASDatabase` in your SQL Server
- Creates all 18 tables
- Inserts seed data (3 users, 1 bank account, 3 billers, etc.)

**Expected Output:**
```
Build started...
Build succeeded.
Done.
```

---

## Step 2: Run the Application ⏱️ (30 seconds)

### Start the App
Press **F5** or click the green **▶ Run** button in Visual Studio

**What happens?**
- Application compiles
- Web server starts
- Browser opens automatically
- You'll see the login page

---

## Step 3: Login and Explore ⏱️ (5 minutes)

### 🔐 Test Accounts

#### **Admin Account** (Full Access)
```
Email: admin@bfassystem.com
Password: Admin123
```
**What you can do:**
- View all system data
- Manage users, accounts, transactions
- View audit logs
- Access finance & accounting modules

#### **Employee Account** (Finance & Accounting)
```
Email: employee@bfassystem.com
Password: Emp123
```
**What you can do:**
- Create/manage Accounts Payable & Receivable
- Create Journal Entries
- View financial reports

#### **Customer Account** (Banking)
```
Email: customer1@mail.com
Password: Cust123
```
**What you can do:**
- Deposit money
- Withdraw money
- Transfer funds
- Pay bills
- View transaction history
- **Starting Balance: $5,000.00**

---

## 🎮 Test Drive the System

### Try These Actions:

#### As Customer:
1. ✅ **Login** as `customer1@mail.com`
2. ✅ **Deposit** $100
3. ✅ **Withdraw** $50
4. ✅ **Transfer** $25 to another account (use `BFAS-100000000001`)
5. ✅ **Pay Bill** - Electric Company ($30)
6. ✅ **View Transactions** - See your history

#### As Employee:
1. ✅ **Login** as `employee@bfassystem.com`
2. ✅ **Create Account Payable** - Add a vendor you need to pay
3. ✅ **Create Account Receivable** - Add money owed to you
4. ✅ **Create Journal Entry** - Record an accounting transaction
5. ✅ **View Trial Balance** - See accounting balances

#### As Admin:
1. ✅ **Login** as `admin@bfassystem.com`
2. ✅ **Dashboard** - View system statistics
3. ✅ **View Users** - See all system users
4. ✅ **View Transactions** - See all banking activities
5. ✅ **Audit Logs** - See who did what and when

---

## 🔍 Verify Everything is Working

### Check 1: Database Created ✅
**Open SSMS (SQL Server Management Studio)**
1. Connect to: `localhost\SQLEXPRESS`
2. Expand **Databases**
3. You should see: **BFASDatabase**
4. Expand **Tables** - you should see 18 tables

### Check 2: Seed Data Loaded ✅
**Run in SSMS:**
```sql
USE BFASDatabase;

-- Check users
SELECT * FROM users;
-- Should show: Admin, Employee, Customer

-- Check bank account
SELECT * FROM bank_accounts;
-- Should show: 1 account with $5,000 balance

-- Check billers
SELECT * FROM billers;
-- Should show: Electric, Water, Internet
```

### Check 3: Application Running ✅
1. Browser opens at `https://localhost:xxxx`
2. Login page displays
3. Can login with any test account
4. Dashboard loads after login

---

## 🐛 Troubleshooting

### ❌ Problem: "Build failed" during migration
**Solution:**
```powershell
dotnet clean
dotnet restore
dotnet build
Add-Migration InitialCreate
Update-Database
```

### ❌ Problem: "Cannot connect to database"
**Solution:**
1. Open **SQL Server Configuration Manager**
2. Check if **SQL Server (SQLEXPRESS)** is **Running**
3. If not, right-click → **Start**
4. Try migration again

### ❌ Problem: "Database already exists"
**Solution:**
```powershell
Drop-Database
Update-Database
```

### ❌ Problem: Login fails
**Solution:**
- Passwords are **case-sensitive**
- Try: `Admin123` (capital A)
- Clear browser cookies
- Check if database seeded correctly

### ❌ Problem: "Session expired"
**Solution:**
- Sessions timeout after 30 minutes
- Just login again
- This is normal behavior

---

## 📱 What to Test After Setup

### Banking Operations (Customer):
- [ ] Deposit $100
- [ ] Withdraw $50
- [ ] Transfer $25
- [ ] Pay a bill
- [ ] View transaction history
- [ ] Check updated balance

### Finance Module (Employee):
- [ ] Create 1 Account Payable
- [ ] Create 1 Account Receivable
- [ ] View pending payments
- [ ] Create a Journal Entry

### Accounting Module (Employee):
- [ ] View Ledger Accounts
- [ ] Check Trial Balance
- [ ] View Financial Statements

### Admin Functions:
- [ ] View all users
- [ ] View all transactions
- [ ] Check audit logs
- [ ] View bank accounts

---

## 🎨 Customize Your System

### Change Colors:
Edit `wwwroot/css/bfas-style.css`
```css
/* Primary Color (currently green) */
.logo {
    color: #3dd598; /* Change this to your color */
}
```

### Change Session Timeout:
Edit `Program.cs`
```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Change from 30 to 60
});
```

### Add More Demo Users:
Run in SSMS:
```sql
INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES (3, 'John Doe', 'john@mail.com', 'Pass123', 'Active', GETDATE());
```

---

## 📊 View Database in Visual Studio

Don't have SSMS? No problem!

1. **View** → **SQL Server Object Explorer**
2. Expand **(localdb)\MSSQLLocalDB** or your SQL Server instance
3. Expand **Databases** → **BFASDatabase**
4. Right-click any table → **View Data**

---

## 🚀 What You've Built

### ✅ Complete Features:
- ✅ Role-based authentication (Admin, Employee, Customer)
- ✅ Banking operations (Deposit, Withdraw, Transfer, Bill Pay)
- ✅ Finance management (AP, AR)
- ✅ Accounting system (Journal, Ledger, Trial Balance, Financial Statements)
- ✅ Audit logging (every action tracked)
- ✅ Modern UI with responsive design
- ✅ **NEW**: Service layer architecture
- ✅ **NEW**: Transaction rollback protection
- ✅ **NEW**: Password hashing
- ✅ **NEW**: Automatic audit logging

### 📁 Your Files:
- **40+ Views** - All UI pages ready
- **4 Controllers** - Auth, Admin, Employee, Customer
- **18 Models** - Complete database schema
- **3 Services** - Banking, Password, Audit
- **Complete Documentation** - README, Guides, Commands

---

## 💡 Next Actions

### For Learning:
1. 📖 Read `FOLDER_STRUCTURE.md` - Understand file organization
2. 📖 Read `ENHANCEMENTS.md` - See what was improved
3. 📖 Read `COMMANDS.md` - Learn useful commands

### For Development:
1. 🎨 Customize the UI colors
2. ➕ Add more demo data
3. 🧪 Test all features
4. 📝 Add your own features

### For Production:
1. 🔒 Upgrade to BCrypt password hashing
2. 🔐 Add SSL certificate
3. 📧 Implement email notifications
4. 📊 Add reporting features

---

## 🆘 Need Help?

### Resources:
- **README.md** - Complete documentation
- **SETUP_GUIDE.md** - Detailed setup
- **FOLDER_STRUCTURE.md** - File organization
- **ENHANCEMENTS.md** - Recent improvements
- **COMMANDS.md** - Command reference

### Common Links:
- **SQL Server Express**: https://www.microsoft.com/sql-server/sql-server-downloads
- **SSMS Download**: https://aka.ms/ssmsfullsetup
- **ASP.NET Docs**: https://docs.microsoft.com/aspnet/core

---

## ✅ Success Checklist

Before you say "It's working!":

- [ ] Database created successfully
- [ ] All 18 tables exist
- [ ] Can login as Admin
- [ ] Can login as Employee
- [ ] Can login as Customer
- [ ] Deposit works
- [ ] Withdraw works
- [ ] Transfer works
- [ ] Bill payment works
- [ ] Transactions appear in history
- [ ] Audit logs are recording actions
- [ ] Balance updates correctly

---

## 🎉 You're Ready!

Your **Banking, Finance, and Accounting System** is now:
- ✅ **Running** on SQL Server Express
- ✅ **Enhanced** with service layer
- ✅ **Secure** with password hashing
- ✅ **Audited** with automatic logging
- ✅ **Ready** for development or demo

**Enjoy exploring your system!** 🚀

---

**Questions? Check the other documentation files or review the code comments!**
