using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankAndFinance.Data;
using BankAndFinance.Models;
using BankAndFinance.Helpers;

namespace BankAndFinance.Controllers
{
    [AuthorizeRole("Admin", "Employee")]
    public class ProjectController : Controller
    {
        private readonly BFASDbContext _context;

        public ProjectController(BFASDbContext context)
        {
            _context = context;
        }

        // GET: Project/Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            var projects = await _context.Set<Project>()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.TotalProjects = projects.Count;
            ViewBag.ActiveProjects = projects.Count(p => p.Status == "In Progress");
            ViewBag.TotalBudget = projects.Where(p => p.Budget.HasValue).Sum(p => p.Budget.Value);

            return View(projects);
        }

        // GET: Project/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Balance = await GetUserBalanceAsync();
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                project.ProjectCode = GenerateProjectCode();
                project.CreatedAt = DateTime.Now;
                project.UpdatedAt = DateTime.Now;
                _context.Add(project);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Project created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(project);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Set<Project>().FindAsync(id);
            if (project == null) return NotFound();

            ViewBag.Balance = await GetUserBalanceAsync();
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    project.UpdatedAt = DateTime.Now;
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Project updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Balance = await GetUserBalanceAsync();
            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Set<Project>().FindAsync(id);
            if (project != null)
            {
                _context.Set<Project>().Remove(project);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Project deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Set<Project>().Any(e => e.ProjectId == id);
        }

        private string GenerateProjectCode()
        {
            var random = new Random();
            return $"PRJ{DateTime.Now.Year}{random.Next(1000, 9999)}";
        }

        private async Task<decimal> GetUserBalanceAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(ba => ba.UserId == userId);
            return account?.Balance ?? 0;
        }
    }
}
