using BankAndFinance.Data;
using BankAndFinance.Models;
using Microsoft.AspNetCore.Http;

namespace BankAndFinance.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string module);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly BFASDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(BFASDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string module)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userId = session?.GetInt32("UserId");

            if (userId.HasValue)
            {
                var auditLog = new AuditLog
                {
                    UserId = userId.Value,
                    Action = action,
                    Module = module,
                    Timestamp = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
