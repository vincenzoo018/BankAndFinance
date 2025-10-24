# 🚀 BFAS Command Reference

## Essential Commands for Development

### Package Manager Console Commands
(Open: Tools > NuGet Package Manager > Package Manager Console)

#### Initial Database Setup
```powershell
# Create initial migration
Add-Migration InitialCreate

# Apply migration and create database
Update-Database
```

#### Database Management
```powershell
# Create a new migration after model changes
Add-Migration [MigrationName]

# Apply pending migrations
Update-Database

# Remove last migration (if not applied)
Remove-Migration

# Update to specific migration
Update-Database -Migration [MigrationName]

# Rollback all migrations
Update-Database -Migration 0

# View migration SQL without applying
Script-Migration

# Drop database and recreate
Drop-Database
Update-Database
```

#### Troubleshooting Commands
```powershell
# List all migrations
Get-Migration

# View DbContext information
Get-DbContext

# Scaffold DbContext from existing database (if needed)
Scaffold-DbContext "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
```

---

### .NET CLI Commands
(Run in terminal/command prompt in project directory)

#### Build & Run
```bash
# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Run with watch (auto-reload)
dotnet watch run

# Clean build artifacts
dotnet clean
```

#### Database Commands (EF Core Tools)
```bash
# Install EF Core tools globally (one-time)
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop
```

#### Testing
```bash
# Run tests (if tests exist)
dotnet test

# Run with detailed output
dotnet test --verbosity detailed
```

---

### SQL Server Commands
(Run in SQL Server Management Studio or Azure Data Studio)

#### Database Queries

**View all users:**
```sql
SELECT u.user_id, u.full_name, u.email, r.role_name, u.status
FROM users u
INNER JOIN roles r ON u.role_id = r.role_id;
```

**View all bank accounts:**
```sql
SELECT ba.account_id, ba.account_number, u.full_name, ba.balance
FROM bank_accounts ba
INNER JOIN users u ON ba.user_id = u.user_id;
```

**View all transactions:**
```sql
SELECT t.transaction_id, ba.account_number, t.transaction_type, 
       t.amount, t.transaction_date, t.status
FROM transactions t
INNER JOIN bank_accounts ba ON t.account_id = ba.account_id
ORDER BY t.transaction_date DESC;
```

**View audit logs:**
```sql
SELECT al.log_id, u.full_name, al.action, al.module, al.timestamp
FROM audit_logs al
INNER JOIN users u ON al.user_id = u.user_id
ORDER BY al.timestamp DESC;
```

**Check account balance:**
```sql
SELECT account_number, balance
FROM bank_accounts
WHERE account_number = 'BFAS-100000000001';
```

#### Data Manipulation

**Add new user:**
```sql
INSERT INTO users (role_id, full_name, email, password, status, created_at)
VALUES (3, 'New Customer', 'newcustomer@mail.com', 'Pass123', 'Active', GETDATE());
```

**Add new biller:**
```sql
INSERT INTO billers (biller_name, biller_type, created_at)
VALUES ('Cable Company', 'Telecom', GETDATE());
```

**Update user status:**
```sql
UPDATE users
SET status = 'Inactive'
WHERE user_id = 4;
```

**Reset account balance (for testing):**
```sql
UPDATE bank_accounts
SET balance = 5000.00
WHERE account_id = 1;
```

#### Database Maintenance

**Backup database:**
```sql
BACKUP DATABASE BFASDatabase 
TO DISK = 'C:\Backup\BFASDatabase.bak';
```

**Check database size:**
```sql
SELECT 
    DB_NAME(database_id) AS DatabaseName,
    (size * 8) / 1024 AS SizeMB
FROM sys.master_files
WHERE DB_NAME(database_id) = 'BFASDatabase';
```

**View table row counts:**
```sql
SELECT 
    t.NAME AS TableName,
    p.rows AS RowCounts
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.OBJECT_ID
WHERE p.index_id < 2
ORDER BY p.rows DESC;
```

---

### Git Commands (Version Control)

#### Basic Workflow
```bash
# Check status
git status

# Stage all changes
git add .

# Commit changes
git commit -m "Your commit message"

# Push to remote
git push origin main

# Pull latest changes
git pull origin main
```

#### Branch Management
```bash
# Create new branch
git checkout -b feature/new-feature

# Switch branches
git checkout main

# Merge branch
git merge feature/new-feature

# Delete branch
git branch -d feature/new-feature
```

---

### Visual Studio Shortcuts

#### Development
- **F5** - Start debugging
- **Ctrl + F5** - Start without debugging
- **Shift + F5** - Stop debugging
- **F9** - Toggle breakpoint
- **F10** - Step over
- **F11** - Step into
- **Ctrl + .** - Quick actions (add using, etc.)

#### Navigation
- **Ctrl + ,** - Go to all (files, classes, methods)
- **F12** - Go to definition
- **Ctrl + F12** - Go to implementation
- **Ctrl + -** - Navigate backward
- **Ctrl + Shift + -** - Navigate forward

#### Editing
- **Ctrl + K, Ctrl + D** - Format document
- **Ctrl + K, Ctrl + C** - Comment selection
- **Ctrl + K, Ctrl + U** - Uncomment selection
- **Ctrl + Space** - IntelliSense

---

### NuGet Package Management

#### Via Package Manager Console
```powershell
# Install package
Install-Package PackageName

# Update package
Update-Package PackageName

# Uninstall package
Uninstall-Package PackageName

# List installed packages
Get-Package

# Update all packages
Update-Package
```

#### Via .NET CLI
```bash
# Add package
dotnet add package PackageName

# Remove package
dotnet remove package PackageName

# List packages
dotnet list package

# Update package
dotnet add package PackageName --version X.X.X
```

---

### Development Workflow

#### Daily Development Routine
```bash
# 1. Pull latest changes
git pull origin main

# 2. Restore packages (if needed)
dotnet restore

# 3. Apply any new migrations
dotnet ef database update

# 4. Run the application
dotnet watch run

# 5. Make changes and test

# 6. Commit your work
git add .
git commit -m "Description of changes"
git push origin main
```

#### After Changing Models
```powershell
# 1. Create migration
Add-Migration [DescriptiveName]

# 2. Review generated migration file

# 3. Apply to database
Update-Database

# 4. Test the changes
```

#### Quick Reset for Testing
```powershell
# Drop and recreate database
Drop-Database
Update-Database

# This will:
# - Delete all data
# - Recreate tables
# - Re-seed initial data
```

---

### Common Scenarios

#### Scenario 1: Fresh Start
```powershell
# In Package Manager Console
Drop-Database
Add-Migration InitialCreate
Update-Database
```

#### Scenario 2: Model Changed
```powershell
# In Package Manager Console
Add-Migration [ChangeName]
Update-Database
```

#### Scenario 3: Migration Error
```powershell
# Remove bad migration
Remove-Migration

# Fix model issue

# Try again
Add-Migration [ChangeName]
Update-Database
```

#### Scenario 4: Connection String Changed
```bash
# 1. Update appsettings.json

# 2. Drop and recreate
Drop-Database
Update-Database
```

---

### Performance & Monitoring

#### Check Application Performance
```bash
# Run in release mode for accurate performance
dotnet run --configuration Release
```

#### Monitor SQL Queries (EF Core)
Add to `Program.cs`:
```csharp
builder.Services.AddDbContext<BFASDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(); // Development only!
    options.LogTo(Console.WriteLine);
});
```

---

### Quick Reference Card

| Task | Command |
|------|---------|
| Create database | `Update-Database` |
| Add migration | `Add-Migration [Name]` |
| Run app | `F5` or `dotnet run` |
| Restore packages | `dotnet restore` |
| Build project | `dotnet build` |
| Format code | `Ctrl + K, Ctrl + D` |
| Go to definition | `F12` |
| Drop database | `Drop-Database` |

---

### Environment Variables (Optional)

For production deployment:
```bash
# Set environment
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Set connection string
$env:ConnectionStrings__DefaultConnection = "YourConnectionString"
```

---

### Troubleshooting Commands

**Clear NuGet cache:**
```bash
dotnet nuget locals all --clear
```

**Rebuild solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

**Check .NET version:**
```bash
dotnet --version
dotnet --list-sdks
```

**Check installed tools:**
```bash
dotnet tool list --global
```

---

**Keep this file handy for quick reference during development!** 📌
