namespace CodeEscape.DTOs.GameSession
{
    public class MinhasPartidasResponse
    {
        public int GameSessionId { get; set; }

        public string Sala { get; set; }

        public int? Pontuacao { get; set; }

        public bool Finalizada { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public double? TempoSegundos { get; set; }
    }
}
