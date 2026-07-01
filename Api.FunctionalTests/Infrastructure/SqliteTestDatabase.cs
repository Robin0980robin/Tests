using Api.Domain.Entities;
using Api.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Api.FunctionalTests.Infrastructure;

public sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new(
        "Data Source=file:functional-tests?mode=memory&cache=shared"
    );

    public SqliteConnection Connection => _connection;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        await ResetAsync();
    }

    public async Task ResetAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.Usuarios.AddRangeAsync(UsuarioBogusFactory.CreateUsuarios(10));
        await context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new ApplicationDbContext(options);
    }
}
