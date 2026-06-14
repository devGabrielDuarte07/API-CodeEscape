using System;
using System.Collections.Generic;

namespace CodeEscape.Models;

public partial class TabelaUsuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Senha { get; set; } = null!;

    public string Perfil { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public bool IsAtivo { get; set; }

    public DateTime? CriadoEm { get; set; }

    public virtual ICollection<Gamesession> Gamesessions { get; set; } = new List<Gamesession>();

    public virtual ICollection<TabelaRoom> TabelaRooms { get; set; } = new List<TabelaRoom>();
}
