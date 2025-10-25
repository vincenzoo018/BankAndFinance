# 🚨 EMERGENCY FIX - Profile Photo Error

## ❌ **ERROR**
```
SqlException: Invalid column name 'profile_photo'.
```

## ✅ **IMMEDIATE FIX**

### **Step 1: Run This SQL Command NOW**

Open **SQL Server Management Studio** and run:

```sql
USE BFASDatabase;
GO

ALTER TABLE users
ADD profile_photo NVARCHAR(255) NULL;
GO
```

**OR** execute the file: `QUICK_FIX_PROFILE_PHOTO.sql`

---

### **Step 2: Restart Application**
1. Stop the application (if running)
2. Press F5 to run again
3. Try logging in - **SHOULD WORK NOW!**

---

## 📋 **STEP-BY-STEP IN SSMS**

1. Open **SQL Server Management Studio**
2. Connect to your SQL Server instance
3. Click **New Query**
4. Paste this code:
   ```sql
   USE BFASDatabase;
   GO
   
   ALTER TABLE users
   ADD profile_photo NVARCHAR(255) NULL;
   GO
   
   SELECT 'SUCCESS! Profile photo column added.' AS Message;
   ```
5. Click **Execute** (F5)
6. Should see: "SUCCESS! Profile photo column added."
7. Close SSMS
8. Go back to Visual Studio
9. Run your application (F5)
10. Login - **WORKS!** ✅

---

## 🎯 **ALTERNATIVE: Entity Framework Migration**

If you prefer using migrations:

```bash
# In Package Manager Console
Add-Migration AddProfilePhotoToUser
Update-Database
```

---

## ✅ **VERIFICATION**

After running the SQL:

```sql
-- Verify column exists
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'users' AND COLUMN_NAME = 'profile_photo';
```

Should show:
```
COLUMN_NAME      DATA_TYPE
profile_photo    nvarchar
```

---

## 🚀 **DONE!**

Your login should work perfectly now!

**Test it**:
- Email: `admin@bfas.com`
- Password: `admin123`

**Should login successfully!** ✅
