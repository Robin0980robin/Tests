using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(builder =>
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);
            builder.HasQueryFilter(u => !u.EstaEliminado);

            builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();

            builder.Property(u => u.Apellido).HasMaxLength(100).IsRequired();

            builder.Property(u => u.EstaEliminado).IsRequired();

            builder.Property(u => u.EliminadoEn);

            // Conversión de tipo para el objeto de valor de Vogen (Email)
            builder
                .Property(u => u.Email)
                .HasConversion(email => email.Value, value => Email.From(value))
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
        });
    }
}
