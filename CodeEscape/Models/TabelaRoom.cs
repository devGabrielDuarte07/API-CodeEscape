using System;
using System.Collections.Generic;

namespace CodeEscape.Models;

public partial class TabelaRoom
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string? CapaUrl { get; set; }

    public int CriadorId { get; set; }

    public ulong IsAtiva { get; set; }

    public DateTime? CriadaEm { get; set; }

    public virtual TabelaUsuario Criador { get; set; } = null!;

    public virtual ICollection<Gamesession> Gamesessions { get; set; } = new List<Gamesession>();

    public virtual ICollection<TabelaDesafio> TabelaDesafios { get; set; } = new List<TabelaDesafio>();
}
