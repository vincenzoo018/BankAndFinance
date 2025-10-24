-- Seed Admin and Employee Accounts
-- Run this script after creating the database

USE BFASDatabase;
GO

-- Insert Roles (if not exists)
IF NOT EXISTS (SELECT * FROM roles WHERE role_id = 1)
BEGIN
    INSERT INTO roles (role_name) VALUES ('Admin');
END

IF NOT EXISTS (SELECT * FROM roles WHERE role_id = 2)
BEGIN
    INSERT INTO roles (role_name) VALUES ('Employee');
END

IF NOT EXISTS (SELECT * FROM roles WHERE role_id = 3)
BEGIN
    INSERT INTO roles (role_name) VALUES ('Customer');
END
GO

-- Hash for 'admin123': jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=
-- Hash for 'employee123': 6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=

-- Insert Admin Account
IF NOT EXISTS (SELECT * FROM users WHERE email = 'admin@bfas.com')
BEGIN
    INSERT INTO users (role_id, full_name, email, password, status, created_at)
    VALUES (1, 'System Administrator', 'admin@bfas.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Active', GETDATE());
    PRINT 'Admin account created: admin@bfas.com / admin123';
END
ELSE
BEGIN
    PRINT 'Admin account already exists';
END
GO

-- Insert Employee Accounts
IF NOT EXISTS (SELECT * FROM users WHERE email = 'employee1@bfas.com')
BEGIN
    INSERT INTO users (role_id, full_name, email, password, status, created_at)
    VALUES (2, 'John Smith', 'employee1@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE());
    PRINT 'Employee 1 created: employee1@bfas.com / employee123';
END

IF NOT EXISTS (SELECT * FROM users WHERE email = 'employee2@bfas.com')
BEGIN
    INSERT INTO users (role_id, full_name, email, password, status, created_at)
    VALUES (2, 'Sarah Johnson', 'employee2@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE());
    PRINT 'Employee 2 created: employee2@bfas.com / employee123';
END

IF NOT EXISTS (SELECT * FROM users WHERE email = 'employee3@bfas.com')
BEGIN
    INSERT INTO users (role_id, full_name, email, password, status, created_at)
    VALUES (2, 'Michael Davis', 'employee3@bfas.com', '6p+dVEyPnRy8j9HXs6KmNQ+gWh9M8IqXjuKbvZ1Y3BU=', 'Active', GETDATE());
    PRINT 'Employee 3 created: employee3@bfas.com / employee123';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Seed Data Summary:';
PRINT '========================================';
PRINT 'Admin Account:';
PRINT '  Email: admin@bfas.com';
PRINT '  Password: admin123';
PRINT '';
PRINT 'Employee Accounts:';
PRINT '  1. email: employee1@bfas.com / password: employee123';
PRINT '  2. email: employee2@bfas.com / password: employee123';
PRINT '  3. email: employee3@bfas.com / password: employee123';
PRINT '========================================';
GO
