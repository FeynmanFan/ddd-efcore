namespace ddd_efcore.Auditing
{
    public interface IAuditLogger
    {
        void Log(AuditLogEntry entry);
        IReadOnlyList<AuditLogEntry> GetLogs();
    }

    public class InMemoryAuditLogger : IAuditLogger
    {
        private readonly List<AuditLogEntry> _logs = new();

        public void Log(AuditLogEntry entry)
        {
            _logs.Add(entry);
        }

        public IReadOnlyList<AuditLogEntry> GetLogs()
        {
            return _logs.AsReadOnly();
        }
    }
}