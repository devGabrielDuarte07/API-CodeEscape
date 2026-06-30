using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CodeEscape.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tabela_usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<string>(type: "text", nullable: false, defaultValue: "J"),
                    AvatarUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabela_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tabela_room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CapaUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CriadorId = table.Column<int>(type: "integer", nullable: false),
                    IsAtiva = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabela_room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tabela_room_tabela_usuario_CriadorId",
                        column: x => x.CriadorId,
                        principalTable: "tabela_usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "gamesession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    RoomId = table.Column<int>(type: "integer", nullable: true),
                    Pontuacao = table.Column<int>(type: "integer", nullable: true),
                    EnigmaAtual = table.Column<int>(type: "integer", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Finalizada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamesession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gamesession_tabela_room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "tabela_room",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_gamesession_tabela_usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "tabela_usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tabela_desafios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RoomId = table.Column<int>(type: "integer", nullable: false),
                    Pergunta = table.Column<string>(type: "text", nullable: false),
                    Dica = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Resposta = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabela_desafios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tabela_desafios_tabela_room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "tabela_room",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "gamesessiondica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameSessionId = table.Column<int>(type: "integer", nullable: false),
                    OrdemEnigma = table.Column<int>(type: "integer", nullable: true),
                    DataUso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamesessiondica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gamesessiondica_gamesession_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "gamesession",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_gamesession_RoomId",
                table: "gamesession",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_gamesession_UserId",
                table: "gamesession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_gamesessiondica_GameSessionId",
                table: "gamesessiondica",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_tabela_desafios_RoomId",
                table: "tabela_desafios",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_tabela_room_CriadorId",
                table: "tabela_room",
                column: "CriadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tabela_usuario_Email",
                table: "tabela_usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tabela_usuario_Username",
                table: "tabela_usuario",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gamesessiondica");

            migrationBuilder.DropTable(
                name: "tabela_desafios");

            migrationBuilder.DropTable(
                name: "gamesession");

            migrationBuilder.DropTable(
                name: "tabela_room");

            migrationBuilder.DropTable(
                name: "tabela_usuario");
        }
    }
}
