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

    public ulong Finalizada { get; set; }

    public ulong IsAtivo { get; set; }

    public virtual TabelaRoom? Room { get; set; }

    public virtual TabelaUsuario? User { get; set; }
}
