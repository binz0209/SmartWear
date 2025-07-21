using Microsoft.EntityFrameworkCore;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Data;
using Repositories.Interfaces;

namespace Repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _context.AuditLogs.ToListAsync();
        }

        public async Task<AuditLog> GetAuditLogByIdAsync(Guid id)
        {
            return await _context.AuditLogs.FindAsync(id);
        }

        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuditLogAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Update(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuditLogAsync(Guid id)
        {
            var auditLog = await GetAuditLogByIdAsync(id);
            if (auditLog != null)
            {
                _context.AuditLogs.Remove(auditLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
