namespace CodeEscape.DTOs.GameSession
{
    public class StartGameSessionResponse
    {
        public int? GameSessionId { get; set; }
        public int? Pontuacao { get; set; }
        public int? EnigmaAtual {  get; set; }
        public bool? Continuando { get; set; }
    }
}
