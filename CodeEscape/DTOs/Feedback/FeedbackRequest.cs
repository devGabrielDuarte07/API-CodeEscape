namespace CodeEscape.DTOs.Feedback
{
    public class FeedbackRequest
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string Categoria { get; set; }
        public string Mensagem { get; set; }

    }
}
