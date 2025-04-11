using System;
using System.Threading.Tasks;
using Npgsql;

namespace OpsVerse.Backend.Services
{
    public class LoggingService
    {
        private readonly string _connectionString;

        public LoggingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task LogMessageAsync(Guid? nodeId, string level, string message, object metadata = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = @"
                        INSERT INTO system_logs (log_id, node_id, log_level, message, metadata)
                        VALUES (@logId, @nodeId, @level, @message, @metadata)";
                    
                    cmd.Parameters.AddWithValue("logId", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("nodeId", nodeId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("level", level);
                    cmd.Parameters.AddWithValue("message", message);
                    cmd.Parameters.AddWithValue("metadata", metadata != null 
                        ? Npgsql.NpgsqlTypes.NpgsqlDbType.Jsonb, System.Text.Json.JsonSerializer.Serialize(metadata)
                        : DBNull.Value);
                    
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Helper methods for different log levels
        public Task LogInfoAsync(Guid? nodeId, string message, object metadata = null) =>
            LogMessageAsync(nodeId, "INFO", message, metadata);
            
        public Task LogWarningAsync(Guid? nodeId, string message, object metadata = null) =>
            LogMessageAsync(nodeId, "WARNING", message, metadata);
            
        public Task LogErrorAsync(Guid? nodeId, string message, object metadata = null) =>
            LogMessageAsync(nodeId, "ERROR", message, metadata);
            
        public Task LogDebugAsync(Guid? nodeId, string message, object metadata = null) =>
            LogMessageAsync(nodeId, "DEBUG", message, metadata);
    }
}