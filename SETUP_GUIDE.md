# BFAS Quick Setup Guide

## 🚀 Quick Start (5 Minutes)

### Step 1: Install Dependencies
Make sure you have:
- ✅ Visual Studio 2022 or later
- ✅ .NET 8.0 SDK
- ✅ SQL Server (LocalDB comes with Visual Studio)

### Step 2: Open the Project
1. Open `BankAndFinance.sln` in Visual Studio
2. Wait for NuGet packages to restore automatically

### Step 3: Create the Database
Open **Package Manager Console** (View > Other Windows > Package Manager Console)

Run these commands:
```powershell
Add-Migration InitialCreate
Update-Database
```

### Step 4: Run the Application
Press **F5** or click the green **Run** button

### Step 5: Login
Use these credentials to test:

**Admin:**
- Email: `admin@bfassystem.com`
- Password: `Admin123`

**Employee:**
- Email: `employee@bfassystem.com`
- Password: `Emp123`

**Customer:**
- Email: `customer1@mail.com`
- Password: `Cust123`

---

## 📂 File Organization Guide

### Where to Find Things

#### **Controllers/** - Business Logic
- `AuthController.cs` - Login/Logout functionality
- `AdminController.cs` - Admin dashboard and all management pages
- `EmployeeController.cs` - Finance and accounting operations
- `CustomerController.cs` - Banking operations (deposit, withdraw, transfer)

#### **Models/** - Database Tables
All 18 models matching your ERD:
- `User.cs`, `Role.cs`, `BankAccount.cs`
- `Transaction.cs`, `Transfer.cs`, `Payment.cs`
- `AccountsPayable.cs`, `AccountsReceivable.cs`
- `JournalEntry.cs`, `JournalEntryLine.cs`
- And more...

#### **Views/** - User Interface
```
Views/
├── Admin/              # Admin pages (Users, Transactions, Finance, Accounting)
├── Employee/           # Employee pages (Finance, Accounting)
├── Customer/           # Customer pages (Banking operations)
├── Auth/              # Login and Access Denied pages
└── Shared/            # Layout files
    ├── _AdminLayout.cshtml
    ├── _EmployeeLayout.cshtml
    └── _CustomerLayout.cshtml
```

#### **Data/** - Database Configuration
- `BFASDbContext.cs` - Entity Framework DbContext with all tables and relationships

#### **Helpers/** - Utilities
- `AuthorizeRoleAttribute.cs` - Custom authorization for role-based access

#### **wwwroot/css/** - Styling
- `bfas-style.css` - All custom styles matching your design

---

## 🔧 Common Modifications

### Change Database Connection
Edit `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "YOUR_CONNECTION_STRING_HERE"
}
```

### Add New Admin User
1. Open SQL Server Management Studio
2. Connect to `BFASDatabase`
3. Run:
```sql
INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES (1, 'New Admin', 'newadmin@bfassystem.com', 'Password123', 'Active', GETDATE());
```

### Add New Biller
Through Admin dashboard:
1. Login as Admin
2. Navigate to Banking > Billers
3. (Create functionality to be added)

Or via SQL:
```sql
INSERT INTO billers (biller_name, biller_type, created_at)
VALUES ('New Biller', 'Utility', GETDATE());
```

---

## 📱 Testing the System

### Test Admin Functions
1. Login as Admin
2. View Dashboard - see system statistics
3. Navigate to Users - see all users
4. Check Transactions - view all banking transactions
5. View Audit Logs - see system activities

### Test Employee Functions
1. Login as Employee
2. View Dashboard - see finance/accounting summary
3. Create Account Payable
4. Create Account Receivable
5. Create Journal Entry

### Test Customer Functions
1. Login as Customer
2. View Dashboard - see account balance
3. Deposit money (e.g., $100)
4. Withdraw money (e.g., $50)
5. Transfer to another account
6. Pay a bill
7. View transaction history

---

## 🎯 System Flow

### For Customers:
```
Login → Dashboard → Select Action (Deposit/Withdraw/Transfer/Pay Bills) → Complete Transaction → View in History
```

### For Employees:
```
Login → Dashboard → Manage Finance/Accounting → Create Entries → Review Reports
```

### For Admins:
```
Login → Dashboard → Monitor All Activities → Manage System → View Audit Logs
```

---

## 🐛 Common Issues & Solutions

### Issue: "Database does not exist"
**Solution:**
```powershell
Update-Database
```

### Issue: "Login not working"
**Solution:**
- Check credentials (case-sensitive)
- Clear browser cache
- Verify database seeded correctly

### Issue: "Access Denied"
**Solution:**
- Verify user role in database
- Check if logged in with correct account type
- Some pages are restricted to specific roles

### Issue: "Session expired"
**Solution:**
- Session timeout is 30 minutes
- Simply login again

---

## 📊 Database Tables Quick Reference

| Table | Purpose | Key Fields |
|-------|---------|------------|
| **users** | User accounts | email, password, role_id |
| **bank_accounts** | Customer accounts | account_number, balance |
| **transactions** | All transactions | type, amount, status |
| **accounts_payable** | Money we owe | payee_name, amount, due_date |
| **accounts_receivable** | Money owed to us | payer_name, amount, due_date |
| **journal_entries** | Accounting entries | description, date |
| **audit_logs** | System activity | user_id, action, timestamp |

---

## 🎨 UI Customization

### Change Primary Color
Edit `wwwroot/css/bfas-style.css`:
```css
/* Find #3dd598 and replace with your color */
.logo {
    color: #3dd598; /* Change this */
}
```

### Change Sidebar Width
```css
.sidebar {
    width: 280px; /* Adjust this */
}
```

---

## 📈 Next Steps After Setup

1. **Explore Admin Dashboard** - Familiarize yourself with all management features
2. **Test Banking Operations** - Try all customer functions
3. **Create Sample Data** - Add more accounts, transactions
4. **Review Audit Logs** - See how all actions are tracked
5. **Customize** - Modify colors, add features as needed

---

## 💡 Tips for Development

- **Hot Reload**: Changes to Razor views are reflected immediately
- **Database Changes**: Run `Add-Migration` then `Update-Database`
- **Debugging**: Set breakpoints in Controllers to trace execution
- **Styling**: Edit CSS in `wwwroot/css/bfas-style.css`
- **Testing**: Use different browsers to test session management

---

## 📞 Need Help?

1. Check the main README.md for detailed documentation
2. Review the code comments in Controllers
3. Inspect the database schema in SQL Server
4. Test with the provided demo accounts first

---

**Happy Coding! 🎉**
