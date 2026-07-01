using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data.Repositories;

public class UsuarioRepository(ApplicationDbContext dbContext) : IUsuarioRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<List<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Usuarios.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        await _dbContext.Usuarios.AddAsync(usuario, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
