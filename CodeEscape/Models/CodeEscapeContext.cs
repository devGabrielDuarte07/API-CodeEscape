using Microsoft.EntityFrameworkCore;

namespace CodeEscape.Models;

public partial class CodeEscapeContext : DbContext
{
    public CodeEscapeContext()
    {
    }

    public CodeEscapeContext(DbContextOptions<CodeEscapeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Gamesession> Gamesessions { get; set; }
    public virtual DbSet<Gamesessiondica> Gamesessiondicas { get; set; }
    public virtual DbSet<TabelaDesafio> TabelaDesafios { get; set; }
    public virtual DbSet<TabelaRoom> TabelaRooms { get; set; }
    public virtual DbSet<TabelaUsuario> TabelaUsuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =======================
        // GAME SESSION
        // =======================

        modelBuilder.Entity<Gamesession>(entity =>
        {
            entity.ToTable("gamesession");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Finalizada)
                .HasDefaultValue(false);

            entity.Property(e => e.IsAtivo)
                .HasDefaultValue(true);

            entity.HasOne(d => d.Room)
                .WithMany(p => p.Gamesessions)
                .HasForeignKey(d => d.RoomId);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Gamesessions)
                .HasForeignKey(d => d.UserId);
        });

        // =======================
        // GAME SESSION DICA
        // =======================

        modelBuilder.Entity<Gamesessiondica>(entity =>
        {
            entity.ToTable("gamesessiondica");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.DataUso)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.GameSession)
                .WithMany(p => p.Gamesessiondicas)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // =======================
        // DESAFIOS
        // =======================

        modelBuilder.Entity<TabelaDesafio>(entity =>
        {
            entity.ToTable("tabela_desafios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Titulo)
                .HasMaxLength(255);

            entity.Property(e => e.Resposta)
                .HasMaxLength(255);

            entity.Property(e => e.Dica)
                .HasMaxLength(255);

            entity.Property(e => e.IsAtivo)
                .HasDefaultValue(true);

            entity.HasOne(d => d.Room)
                .WithMany(p => p.TabelaDesafios)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // =======================
        // ROOM
        // =======================

        modelBuilder.Entity<TabelaRoom>(entity =>
        {
            entity.ToTable("tabela_room");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nome)
                .HasMaxLength(100);

            entity.Property(e => e.Descricao)
                .HasMaxLength(255);

            entity.Property(e => e.CapaUrl)
                .HasMaxLength(255);

            entity.Property(e => e.IsAtiva)
                .HasDefaultValue(true);

            entity.Property(e => e.CriadaEm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Criador)
                .WithMany(p => p.TabelaRooms)
                .HasForeignKey(d => d.CriadorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // =======================
        // USUÁRIO
        // =======================

        modelBuilder.Entity<TabelaUsuario>(entity =>
        {
            entity.ToTable("tabela_usuario");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .HasMaxLength(255);

            entity.Property(e => e.Username)
                .HasMaxLength(255);

            entity.Property(e => e.Senha)
                .HasMaxLength(255);

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255);

            entity.Property(e => e.Perfil)
                .HasDefaultValue('J');

            entity.Property(e => e.IsAtivo)
                .HasDefaultValue(true);

            entity.Property(e => e.CriadoEm)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}