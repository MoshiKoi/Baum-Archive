using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Data.Sqlite;

namespace Baum.Data;

public class ProjectContext : DbContext
{
    string ConnectionString { get; init; }

    public DbSet<Language> Languages { get; set; } = default!;
    public DbSet<Word> Words { get; set; } = default!;

    public ProjectContext(string connectionString)
    {
        ConnectionString = connectionString;
    }
    public ProjectContext(FileInfo fileInfo)
    {
        ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = fileInfo.FullName,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(ConnectionString);
}

class ProjectContextFactory : IDesignTimeDbContextFactory<ProjectContext>
{
    public ProjectContext CreateDbContext(string[] opts)
    {
        return new ProjectContext("dummy");
    }
}