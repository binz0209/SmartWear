using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync();
        Task<AuditLog> GetAuditLogByIdAsync(Guid id);
        Task AddAuditLogAsync(AuditLog auditLog);
        Task UpdateAuditLogAsync(AuditLog auditLog);
        Task DeleteAuditLogAsync(Guid id);
    }
}
