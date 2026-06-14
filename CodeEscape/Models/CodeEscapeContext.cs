using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

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

    public virtual DbSet<TabelaDesafio> TabelaDesafios { get; set; }

    public virtual DbSet<TabelaRoom> TabelaRooms { get; set; }

    public virtual DbSet<TabelaUsuario> TabelaUsuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=code_escape;user=root;password=senai", Microsoft.EntityFrameworkCore.ServerVersion.Parse("12.3.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Gamesession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("gamesession");

            entity.HasIndex(e => e.RoomId, "FK_gamesession_tabela_room");

            entity.HasIndex(e => e.UserId, "FK_gamesession_tabela_usuario");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.DataFim).HasColumnType("timestamp");
            entity.Property(e => e.DataInicio).HasColumnType("timestamp");
            entity.Property(e => e.EnigmaAtual).HasColumnType("int(11)");
            entity.Property(e => e.Finalizada)
                .HasDefaultValueSql("b'0'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.IsAtivo)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Pontuacao).HasColumnType("int(11)");
            entity.Property(e => e.RoomId).HasColumnType("int(11)");
            entity.Property(e => e.UserId).HasColumnType("int(11)");

            entity.HasOne(d => d.Room).WithMany(p => p.Gamesessions)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_gamesession_tabela_room");

            entity.HasOne(d => d.User).WithMany(p => p.Gamesessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_gamesession_tabela_usuario");
        });

        modelBuilder.Entity<TabelaDesafio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tabela_desafios");

            entity.HasIndex(e => e.RoomId, "FK_tabela_desafios_tabela_room");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Dica).HasMaxLength(255);
            entity.Property(e => e.IsAtivo)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Ordem)
                .HasColumnType("int(11)")
                .HasColumnName("ordem");
            entity.Property(e => e.Pergunta).HasColumnType("text");
            entity.Property(e => e.Resposta).HasMaxLength(255);
            entity.Property(e => e.RoomId).HasColumnType("int(11)");
            entity.Property(e => e.Titulo).HasMaxLength(255);

            entity.HasOne(d => d.Room).WithMany(p => p.TabelaDesafios)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tabela_desafios_tabela_room");
        });

        modelBuilder.Entity<TabelaRoom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tabela_room");

            entity.HasIndex(e => e.CriadorId, "FK_tabela_room_tabela_usuario");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.CapaUrl).HasMaxLength(255);
            entity.Property(e => e.CriadaEm)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.CriadorId).HasColumnType("int(11)");
            entity.Property(e => e.Descricao)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
            entity.Property(e => e.IsAtiva)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.Criador).WithMany(p => p.TabelaRooms)
                .HasForeignKey(d => d.CriadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tabela_room_tabela_usuario");
        });

        modelBuilder.Entity<TabelaUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tabela_usuario");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CriadoEm)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp");
            entity.Property(e => e.IsAtivo)
                .HasDefaultValueSql("b'1'")
                .HasColumnType("bit(1)");
            entity.Property(e => e.Nome).HasMaxLength(255);
            entity.Property(e => e.Perfil)
                .HasDefaultValueSql("'J'")
                .HasColumnType("enum('J','A')");
            entity.Property(e => e.Senha).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
