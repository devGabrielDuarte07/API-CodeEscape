using CodeEscape.Enums;

namespace CodeEscape.DTOs.Room
{
    public class DadosRoomResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CapaUrl { get; set; }
        public string Criador { get; set; }
        public int QuantidadeEnigma { get; set; }
        public DificuldadeRoom Dificuldade { get; set; }
    }
}
