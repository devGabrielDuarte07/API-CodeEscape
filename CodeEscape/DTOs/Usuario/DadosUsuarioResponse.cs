namespace CodeEscape.DTOs.Usuario
{
    public class DadosUsuarioResponse
    {
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? DataCadastro { get; set; }

        public int SalasConcluidas { get; set; }
        public int EnigmasResolvidos { get; set; }
        public int? PontuacaoTotal { get; set; }


        public string SalaMaisJogada { get; set; }
        public double MelhorTempo { get; set; }
    }
}
