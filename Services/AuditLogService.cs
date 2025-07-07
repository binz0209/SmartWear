using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync()
        {
            return await _auditLogRepository.GetAllAuditLogsAsync();
        }

        public async Task<AuditLog> GetAuditLogByIdAsync(Guid id)
        {
            return await _auditLogRepository.GetAuditLogByIdAsync(id);
        }

        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _auditLogRepository.AddAuditLogAsync(auditLog);
        }

        public async Task UpdateAuditLogAsync(AuditLog auditLog)
        {
            await _auditLogRepository.UpdateAuditLogAsync(auditLog);
        }

        public async Task DeleteAuditLogAsync(Guid id)
        {
            await _auditLogRepository.DeleteAuditLogAsync(id);
        }
    }
}
