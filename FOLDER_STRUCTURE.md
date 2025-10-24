# 📁 BFAS Folder Structure & Organization

## Complete File Organization Guide

This document explains where every file is located and what it does.

---

## 🏗️ Root Level Files

```
BankAndFinance/
├── BankAndFinance.csproj      # Project configuration & NuGet packages
├── BankAndFinance.sln         # Visual Studio solution file
├── Program.cs                 # Application entry point & configuration
├── appsettings.json          # Database connection & settings
├── appsettings.Development.json
├── README.md                 # Main documentation
├── SETUP_GUIDE.md           # Quick setup instructions
└── FOLDER_STRUCTURE.md      # This file
```

### Key Files:
- **Program.cs**: Configures services (DbContext, Session, MVC)
- **appsettings.json**: Contains database connection string

---

## 📂 Controllers/ - Business Logic Layer

```
Controllers/
├── AuthController.cs          # Login, Logout, Access Control
├── AdminController.cs         # Admin Dashboard & Management
├── EmployeeController.cs      # Finance & Accounting Operations
└── CustomerController.cs      # Banking Operations
```

### AuthController.cs
**Purpose**: Authentication and authorization
**Methods**:
- `Login()` - GET/POST for user login
- `Logout()` - Clear session and redirect
- `AccessDenied()` - Show access denied page

### AdminController.cs
**Purpose**: Full system management (Admin role only)
**Methods**:
- `Dashboard()` - System overview
- `Users()`, `Roles()`, `CustomerProfiles()` - User management
- `BankAccounts()`, `Transactions()`, `Transfers()`, `Payments()`, `Billers()` - Banking
- `AccountsPayable()`, `AccountsReceivable()` - Finance
- `JournalEntries()`, `LedgerAccounts()`, `TrialBalance()`, `FinancialStatements()` - Accounting
- `AuditLogs()`, `Branches()`, `Settings()` - Administration

### EmployeeController.cs
**Purpose**: Finance and accounting operations (Employee role)
**Methods**:
- `Dashboard()` - Finance/Accounting overview
- `AccountsPayable()`, `CreateAccountPayable()` - Payables management
- `AccountsReceivable()`, `CreateAccountReceivable()` - Receivables management
- `JournalEntries()`, `CreateJournalEntry()` - Accounting entries
- `LedgerAccounts()`, `TrialBalance()`, `FinancialStatements()` - Reports

### CustomerController.cs
**Purpose**: Banking operations (Customer role)
**Methods**:
- `Dashboard()` - Account overview
- `Transactions()` - Transaction history
- `Deposit()` - Deposit money
- `Withdraw()` - Withdraw money
- `Transfer()` - Transfer funds
- `PayBills()` - Pay bills
- `Profile()` - View profile

---

## 🗃️ Models/ - Database Entity Classes

```
Models/
├── Role.cs                    # System roles table
├── User.cs                    # User accounts table
├── BankAccount.cs            # Bank accounts table
├── Transaction.cs            # Transactions table
├── Transfer.cs               # Fund transfers table
├── Biller.cs                 # Billers table
├── Payment.cs                # Payments table
├── AccountsPayable.cs        # Accounts payable table
├── AccountsReceivable.cs     # Accounts receivable table
├── JournalEntry.cs           # Journal entries table
├── JournalEntryLine.cs       # Journal entry lines table
├── LedgerAccount.cs          # Ledger accounts table
├── TrialBalance.cs           # Trial balance table
├── FinancialStatement.cs     # Financial statements table
├── AuditLog.cs               # Audit logs table
├── Branch.cs                 # Branches table
├── AccountNumberGenerator.cs # Account number sequence
└── CustomerProfile.cs        # Customer profiles table
```

Each model file contains:
- Table name annotation (`[Table("table_name")]`)
- Primary key (`[Key]`)
- Column mappings (`[Column("column_name")]`)
- Data validation attributes
- Navigation properties for relationships

---

## 💾 Data/ - Database Configuration

```
Data/
└── BFASDbContext.cs          # Entity Framework DbContext
```

**BFASDbContext.cs** contains:
- DbSet for each table
- Relationship configurations
- Seed data for initial setup
- Database constraints

---

## 🎨 Views/ - User Interface (Razor Views)

### Views/Admin/ - Admin Interface
```
Views/Admin/
├── Dashboard.cshtml          # Admin dashboard
├── Users.cshtml             # User management
├── Roles.cshtml             # Role management
├── CustomerProfiles.cshtml  # Customer profiles
├── BankAccounts.cshtml      # Bank accounts list
├── Transactions.cshtml      # All transactions
├── Transfers.cshtml         # Fund transfers
├── Payments.cshtml          # Bill payments
├── Billers.cshtml           # Billers management
├── AccountsPayable.cshtml   # Payables list
├── AccountsReceivable.cshtml # Receivables list
├── JournalEntries.cshtml    # Journal entries
├── LedgerAccounts.cshtml    # Ledger accounts
├── TrialBalance.cshtml      # Trial balance report
├── FinancialStatements.cshtml # Financial statements
├── AuditLogs.cshtml         # System audit logs
├── Branches.cshtml          # Branch management
└── Settings.cshtml          # System settings
```

### Views/Employee/ - Employee Interface
```
Views/Employee/
├── Dashboard.cshtml                # Employee dashboard
├── AccountsPayable.cshtml         # View payables
├── CreateAccountPayable.cshtml    # Create payable form
├── AccountsReceivable.cshtml      # View receivables
├── CreateAccountReceivable.cshtml # Create receivable form
├── JournalEntries.cshtml          # View journal entries
├── CreateJournalEntry.cshtml      # Create journal entry form
├── LedgerAccounts.cshtml          # View ledger
├── TrialBalance.cshtml            # View trial balance
└── FinancialStatements.cshtml     # View financial statements
```

### Views/Customer/ - Customer Interface
```
Views/Customer/
├── Dashboard.cshtml          # Customer dashboard
├── Transactions.cshtml       # Transaction history
├── Deposit.cshtml           # Deposit money form
├── Withdraw.cshtml          # Withdraw money form
├── Transfer.cshtml          # Transfer funds form
├── PayBills.cshtml          # Pay bills form
└── Profile.cshtml           # Customer profile
```

### Views/Auth/ - Authentication
```
Views/Auth/
├── Login.cshtml             # Login page
└── AccessDenied.cshtml      # Access denied page
```

### Views/Shared/ - Layouts
```
Views/Shared/
├── _AdminLayout.cshtml      # Admin layout with sidebar
├── _EmployeeLayout.cshtml   # Employee layout with sidebar
└── _CustomerLayout.cshtml   # Customer layout with sidebar
```

### Root View Files
```
Views/
├── _ViewStart.cshtml        # Layout selection logic
└── _ViewImports.cshtml      # Global imports
```

---

## 🛠️ Helpers/ - Utility Classes

```
Helpers/
└── AuthorizeRoleAttribute.cs # Custom authorization attribute
```

**AuthorizeRoleAttribute.cs**:
- Custom action filter for role-based access
- Checks session for user authentication
- Redirects unauthorized users

---

## 🎨 wwwroot/ - Static Files

```
wwwroot/
├── css/
│   └── bfas-style.css       # All custom CSS styles
├── js/                       # JavaScript files (if any)
├── images/                   # Image files
└── lib/                      # Third-party libraries
```

**bfas-style.css** contains:
- Sidebar navigation styles
- Dashboard card styles
- Table styles
- Form styles
- Button styles
- Color scheme
- Responsive design

---

## 🔧 Configuration Files

```
Properties/
└── launchSettings.json       # Development server settings

obj/                          # Build artifacts (generated)
bin/                          # Compiled output (generated)
```

---

## 📊 Database Structure (Once Created)

After running migrations, SQL Server will contain:

```
BFASDatabase
├── Tables/
│   ├── roles
│   ├── users
│   ├── bank_accounts
│   ├── transactions
│   ├── transfers
│   ├── billers
│   ├── payments
│   ├── accounts_payable
│   ├── accounts_receivable
│   ├── journal_entries
│   ├── journal_entry_lines
│   ├── ledger_accounts
│   ├── trial_balance
│   ├── financial_statements
│   ├── audit_logs
│   ├── branch
│   ├── account_number_generator
│   ├── customer_profiles
│   └── __EFMigrationsHistory (tracks migrations)
```

---

## 🎯 File Naming Conventions

### Controllers
- Named: `[Name]Controller.cs`
- Example: `AdminController.cs`, `CustomerController.cs`

### Models
- Named: `[EntityName].cs`
- Example: `User.cs`, `BankAccount.cs`

### Views
- Folder matches Controller name (without "Controller")
- File matches Action method name
- Example: `Views/Admin/Dashboard.cshtml` for `AdminController.Dashboard()`

### Layouts
- Prefixed with underscore: `_[Name]Layout.cshtml`
- Example: `_AdminLayout.cshtml`

---

## 🔄 Request Flow

1. **User Request** → `Program.cs` routes to Controller
2. **Controller** → Processes logic, queries database via `BFASDbContext`
3. **Model** → Entity Framework translates to SQL
4. **Database** → Returns data
5. **Controller** → Passes data to View via ViewBag/Model
6. **View** → Renders HTML using Layout
7. **Response** → Sent back to browser

---

## 📝 Adding New Features

### To add a new page:

1. **Create Action in Controller**:
```csharp
// In AdminController.cs
public async Task<IActionResult> NewFeature()
{
    return View();
}
```

2. **Create View**:
```
Views/Admin/NewFeature.cshtml
```

3. **Add Navigation Link**:
```html
<!-- In _AdminLayout.cshtml -->
<a class="nav-item" href="@Url.Action("NewFeature", "Admin")">
    <span class="nav-icon">🆕</span>
    <span>New Feature</span>
</a>
```

---

## 🎨 Styling Guide

### Colors Used:
- Primary Green: `#3dd598`
- Dark Background: `#1e293b`
- Light Background: `#f5f7fa`
- Text Dark: `#1e293b`
- Text Light: `#64748b`

### Components:
- Cards: `.card`
- Buttons: `.btn-submit`, `.btn-cancel`
- Tables: `table`, `thead`, `tbody`
- Status Badges: `.status-badge`
- Forms: `.form-input`, `.form-select`

---

## 📚 Learning Path

1. **Start with**: `Program.cs` to understand configuration
2. **Then study**: `BFASDbContext.cs` for database structure
3. **Explore**: Models to see data entities
4. **Review**: Controllers to understand business logic
5. **Examine**: Views to see UI implementation
6. **Customize**: CSS in `bfas-style.css`

---

## ✅ Checklist for Modifications

Before modifying:
- [ ] Understand which layer (Controller/Model/View)
- [ ] Check related files (e.g., View needs Controller action)
- [ ] Test after changes
- [ ] Update documentation if needed
- [ ] Check authorization requirements
- [ ] Verify database impact

---

**This structure follows ASP.NET Core MVC best practices and ensures clean separation of concerns.**
