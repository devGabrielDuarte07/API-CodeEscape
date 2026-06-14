using System;
using System.Collections.Generic;

namespace CodeEscape.Models;

public partial class TabelaDesafio
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public int RoomId { get; set; }

    public string Pergunta { get; set; } = null!;

    public string Dica { get; set; } = null!;

    public string Resposta { get; set; } = null!;

    public int? Ordem { get; set; }

    public bool IsAtivo { get; set; }

    public virtual TabelaRoom Room { get; set; } = null!;
}
