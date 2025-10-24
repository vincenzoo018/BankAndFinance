using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Services;

namespace BankAndFinance.Controllers
{
    public class AuthController : Controller
    {
        private readonly BFASDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AuthController(BFASDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to appropriate dashboard
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                return RedirectToDashboard(role);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter both email and password";
                return View();
            }

            // First, find user by email and active status
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Status == "Active");

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Verify the password using the password hasher
            if (!_passwordHasher.VerifyPassword(password, user.Password))
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Set session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role!.RoleName);

            // Log the login
            var auditLog = new AuditLog
            {
                UserId = user.UserId,
                Action = "Login",
                Module = "Authentication",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            return RedirectToDashboard(user.Role.RoleName);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            // If already logged in, redirect to appropriate dashboard
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                return RedirectToDashboard(role);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword, 
                                                    string? phone, DateTime? dateOfBirth, string? address)
        {
            // Validate passwords match
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email address is already registered";
                return View();
            }

            // Create new user
            var user = new User
            {
                RoleId = 3, // Customer role
                FullName = fullName,
                Email = email,
                Password = _passwordHasher.HashPassword(password),
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create customer profile
            var profile = new CustomerProfile
            {
                UserId = user.UserId,
                Phone = phone,
                Address = address,
                DateOfBirth = dateOfBirth
            };

            _context.CustomerProfiles.Add(profile);

            // Create a bank account for the new customer
            var accountNumber = GenerateAccountNumber();
            var bankAccount = new BankAccount
            {
                UserId = user.UserId,
                AccountNumber = accountNumber,
                AccountType = "Savings",
                Balance = 0,
                Status = "Active",
                CreatedAt = DateTime.Now
            };

            _context.BankAccounts.Add(bankAccount);

            // Log the registration
            var auditLog = new AuditLog
            {
                UserId = user.UserId,
                Action = "Account Registration",
                Module = "Authentication",
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Registration successful! Please login with your credentials.";
            return RedirectToAction("Login");
        }

        private string GenerateAccountNumber()
        {
            // Generate account number format: BFAS-YYYYMMDD-XXXXX
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var randomPart = new Random().Next(10000, 99999);
            return $"BFAS-{datePart}-{randomPart}";
        }

        private IActionResult RedirectToDashboard(string? roleName)
        {
            return roleName switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Employee" => RedirectToAction("Dashboard", "Employee"),
                "Customer" => RedirectToAction("Dashboard", "Customer"),
                _ => RedirectToAction("Login")
            };
        }
    }
}
