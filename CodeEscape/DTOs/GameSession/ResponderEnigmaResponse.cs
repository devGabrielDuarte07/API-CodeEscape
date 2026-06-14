namespace CodeEscape.DTOs.GameSession
{
    public class ResponderEnigmaResponse
    {
        public bool? Acertou { get; set; }
        public int? Pontuacao { get; set; }
        public int? EnigmaAtual { get; set; }
        public bool? Finalizada { get; set; }
    }
}
