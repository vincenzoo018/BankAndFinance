# Banking, Finance, and Accounting System (BFAS)

## 📋 Project Overview

The **Banking, Finance, and Accounting System (BFAS)** is a comprehensive financial management system built with **ASP.NET Core MVC** and **Entity Framework Core**. It integrates banking operations, internal finance management, and the full accounting cycle.

### Key Features:
- ✅ **Role-Based Authentication** (Admin, Employee, Customer)
- ✅ **Banking Operations** (Deposits, Withdrawals, Transfers, Bill Payments)
- ✅ **Finance Management** (Accounts Payable & Receivable)
- ✅ **Accounting Module** (Journal Entries, Ledger, Trial Balance, Financial Statements)
- ✅ **Audit Logs** for security and accountability
- ✅ **Modern, Responsive UI** matching company standards

---

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Razor Views with Custom CSS
- **Authentication**: Session-based authentication with role authorization

---

## 📁 Project Structure

```
BankAndFinance/
├── Controllers/           # MVC Controllers (Auth, Admin, Employee, Customer)
├── Models/               # Database entities matching ERD
├── Data/                 # DbContext and database configuration
├── Views/                # Razor views for each role
│   ├── Admin/           # Admin dashboard and management pages
│   ├── Employee/        # Employee finance and accounting pages
│   ├── Customer/        # Customer banking operations
│   ├── Auth/            # Login and authentication pages
│   └── Shared/          # Layout files for each role
├── Helpers/             # Authorization attributes
├── wwwroot/             # Static files (CSS, JS, images)
│   └── css/
│       └── bfas-style.css
└── appsettings.json     # Configuration including connection string
```

---

## 🚀 Setup Instructions

### Prerequisites
- Visual Studio 2022 or later
- .NET 8.0 SDK
- SQL Server (LocalDB or Express)

### Step 1: Clone/Open the Project
Open the project in Visual Studio.

### Step 2: Update Database Connection String
Edit `appsettings.json` and update the connection string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BFASDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Step 3: Install NuGet Packages
The required packages are already specified in `BankAndFinance.csproj`. Restore them:

```bash
dotnet restore
```

Or in Visual Studio: **Tools > NuGet Package Manager > Restore NuGet Packages**

### Step 4: Create Database with Migrations

Open **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console) and run:

```powershell
Add-Migration InitialCreate
Update-Database
```

This will:
- Create the database with all tables
- Seed initial data (Roles, Users, Bank Accounts, Billers, Branches)

### Step 5: Run the Application

Press **F5** or click **Run** in Visual Studio. The application will start at `https://localhost:5001` (or your configured port).

---

## 👥 Default Login Credentials

### Admin Account
- **Email**: admin@bfassystem.com
- **Password**: Admin123
- **Access**: Full system access

### Employee Account
- **Email**: employee@bfassystem.com
- **Password**: Emp123
- **Access**: Finance and Accounting modules

### Customer Account
- **Email**: customer1@mail.com
- **Password**: Cust123
- **Access**: Banking operations only

---

## 🔐 Role-Based Access Control

### Admin Role
- Dashboard with system overview
- **User Management**: View users, roles, customer profiles
- **Banking Management**: View all accounts, transactions, transfers, payments, billers
- **Finance Management**: Accounts Payable & Receivable
- **Accounting Management**: Journal Entries, Ledger, Trial Balance, Financial Statements
- **Administration**: Audit Logs, Branch Management, System Settings

### Employee Role
- Dashboard with finance/accounting overview
- **Finance Management**: Create and manage Accounts Payable & Receivable
- **Accounting Management**: Create Journal Entries, view Ledgers, Trial Balance, Financial Statements

### Customer Role
- Personal dashboard with account balance
- **Banking Operations**:
  - Deposit money
  - Withdraw money
  - Transfer funds to other accounts
  - Pay bills to registered billers
- View transaction history
- View profile information

---

## 📊 Database Schema

The system implements all 18 tables from the ERD:

1. **roles** - System roles (Admin, Employee, Customer)
2. **users** - User accounts
3. **bank_accounts** - Customer bank accounts
4. **transactions** - All banking transactions
5. **transfers** - Fund transfers between accounts
6. **billers** - Registered bill payment companies
7. **payments** - Bill payment records
8. **accounts_payable** - Money owed by the organization
9. **accounts_receivable** - Money owed to the organization
10. **journal_entries** - Accounting journal entries
11. **journal_entry_lines** - Debit/credit lines for journal entries
12. **ledger_accounts** - General ledger accounts
13. **trial_balance** - Trial balance data
14. **financial_statements** - Generated financial statements
15. **audit_logs** - System activity logs
16. **branch** - Bank branch information
17. **account_number_generator** - Auto-generate account numbers
18. **customer_profiles** - Extended customer information

---

## 🎨 UI Features

- **Modern, Clean Design** with professional color scheme
- **Responsive Layout** that works on all devices
- **Sidebar Navigation** with dropdown menus for organized access
- **Dashboard Cards** with statistics and quick actions
- **Data Tables** with filtering and sorting capabilities
- **Form Validation** for data integrity
- **Status Badges** for visual feedback
- **Consistent Typography** and spacing

---

## 🔒 Security Features

1. **Session-Based Authentication**: Secure session management
2. **Role Authorization**: Custom authorization attribute to restrict access
3. **Password Protection**: User passwords (should be hashed in production)
4. **Audit Logging**: All actions are logged with user, timestamp, and module
5. **Input Validation**: Form validation on both client and server side

---

## 📝 Future Enhancements

- Password hashing with BCrypt or Identity
- Email notifications for transactions
- Multi-factor authentication
- Report generation (PDF export)
- Advanced search and filtering
- Real-time balance updates with SignalR
- Mobile app integration
- API endpoints for third-party integration

---

## 🐛 Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Verify connection string in `appsettings.json`
- Check firewall settings

### Migration Errors
```powershell
# Remove existing migrations
Remove-Migration

# Recreate migration
Add-Migration InitialCreate
Update-Database
```

### Session Not Working
- Ensure `UseSession()` is called before `UseAuthorization()` in `Program.cs`
- Clear browser cookies and try again

---

## 📞 Support

For issues or questions about the system:
- Review the code documentation
- Check audit logs for system activities
- Verify user roles and permissions

---

## 📄 License

This project is developed for educational and demonstration purposes.

---

## 👨‍💻 Development

**Project Created**: October 2025  
**Framework**: ASP.NET Core 8.0  
**Database**: Entity Framework Core with SQL Server  
**Architecture**: MVC Pattern with Repository Pattern principles

---

## ✅ Completed Features Checklist

- [x] Project structure and configuration
- [x] Database models matching ERD (18 tables)
- [x] DbContext with relationships and seed data
- [x] Role-based authentication system
- [x] Admin dashboard and all management pages
- [x] Employee dashboard with finance/accounting features
- [x] Customer dashboard with banking operations
- [x] Modern UI with consistent design
- [x] Audit logging system
- [x] Session management
- [x] Form validation
- [x] Responsive layouts

---

**Built with ❤️ for the Banking, Finance, and Accounting System**
