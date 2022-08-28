using System.Data.Common;
using Microsoft.Extensions.Options;
using Npgsql;
using TheKarixPL.TriodeFM.Models;

namespace TheKarixPL.TriodeFM.Factories;

public class DatabaseConnectionFactory
{
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger _logger;

    public DatabaseConnectionFactory(IOptions<DatabaseOptions> databaseOptions, ILogger<DatabaseConnectionFactory> logger)
    {
        _databaseOptions = databaseOptions.Value;
        _logger = logger;
    }

    public DbConnection CreateConnection()
    {
        var conn = new NpgsqlConnection(_databaseOptions.ConnectionString);
        conn.Open();
        return conn;
    }

    public async Task<DbConnection> CreateConnectionAsync()
    {
        var conn = new NpgsqlConnection(_databaseOptions.ConnectionString);
        await conn.OpenAsync();
        return conn;
    }
}