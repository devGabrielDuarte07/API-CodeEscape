using System;
using System.Collections.Generic;

namespace CodeEscape.Models;

public partial class Gamesessiondica
{
    public int Id { get; set; }

    public int GameSessionId { get; set; }

    public int? OrdemEnigma { get; set; }

    public DateTime? DataUso { get; set; }

    public virtual Gamesession GameSession { get; set; } = null!;
}
