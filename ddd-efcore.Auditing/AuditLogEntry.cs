namespace ddd_efcore.Auditing
{
    public class AuditLogEntry
    {
        public string EntityType { get; }
        public Guid EntityId { get; }
        public string Action { get; } // e.g., "Created", "Updated"
        public string UserId { get; }
        public DateTime Timestamp { get; }
        public string Changes { get; } // JSON summary of changes

        public AuditLogEntry(string entityType, Guid entityId, string action, string userId, DateTime timestamp, string changes = null)
        {
            EntityType = entityType;
            EntityId = entityId;
            Action = action;
            UserId = userId;
            Timestamp = timestamp;
            Changes = changes;
        }

        public override string ToString() =>
            $"{EntityType} {Action} by {UserId} at {Timestamp:yyyy-MM-dd HH:mm:ss}" +
            (string.IsNullOrEmpty(Changes) ? "" : $", Changes: {Changes}");
    }
}
