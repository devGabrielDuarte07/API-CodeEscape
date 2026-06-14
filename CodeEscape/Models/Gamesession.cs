using System;
using System.Collections.Generic;

namespace CodeEscape.Models;

public partial class Gamesession
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? RoomId { get; set; }

    public int? Pontuacao { get; set; }

    public int? EnigmaAtual { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public bool Finalizada { get; set; }

    public bool IsAtivo { get; set; }

    public virtual TabelaRoom? Room { get; set; }

    public virtual TabelaUsuario? User { get; set; }
}
