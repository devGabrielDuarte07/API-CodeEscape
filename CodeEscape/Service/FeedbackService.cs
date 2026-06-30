using CodeEscape.Common;
using CodeEscape.Configurations;
using CodeEscape.DTOs.Feedback;
using CodeEscape.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Text;

namespace CodeEscape.Service
{
    public class FeedbackService
    {
        private readonly EmailService _emailService;
        private readonly EmailSettings _settings;

        public FeedbackService(
            EmailService emailService,
            IOptions<EmailSettings> options)
        {
            _emailService = emailService;
            _settings = options.Value;
        }

        public async Task<ResultadoPadrao<bool>> EnviarFeedback(FeedbackRequest request)
        {
            var resultado = ValidarFeedback(request);

            if (!resultado.Sucesso)
                return resultado;

            var html = new StringBuilder();

            html.Append("""
                <div style="
                    font-family: Arial, Helvetica, sans-serif;
                    max-width: 700px;
                    margin: auto;
                    border: 1px solid #ddd;
                    border-radius: 12px;
                    overflow: hidden;">

                    <div style="
                        background:#6d28d9;
                        color:white;
                        padding:20px;
                        text-align:center;">

                        <h2 style="margin:0;">
                            💜 Novo Feedback - CodeEscape
                        </h2>

                    </div>

                    <div style="padding:25px;">
            """);

            html.Append($"""
                <p><strong>Nome:</strong> {request.Nome}</p>

                <p><strong>E-mail:</strong> {request.Email}</p>

                <p><strong>Categoria:</strong> {request.Categoria}</p>

                <p><strong>Data:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>

                <hr>

                <h3>Mensagem</h3>

                <div style="
                    background:#f5f5f5;
                    padding:15px;
                    border-radius:8px;
                    white-space:pre-wrap;">
                    {request.Mensagem}
                </div>
            """);

            html.Append("""
                    </div>

                    <div style="
                        background:#f8f8f8;
                        padding:15px;
                        text-align:center;
                        color:#666;
                        font-size:13px;">
                        Feedback enviado pelo site CodeEscape.
                    </div>

                </div>
            """);

            await _emailService.EnviarEmail(
                _settings.Receiver,
                $"Novo Feedback - {request.Categoria}",
                html.ToString()
            );

            return ResultadoPadrao<bool>.Ok(
                true,
                "Feedback enviado com sucesso."
            );
        }

        private ResultadoPadrao<bool> ValidarFeedback(FeedbackRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                try
                {
                    _ = new MailAddress(request.Email);
                }
                catch
                {
                    return ResultadoPadrao<bool>.Falha("Informe um e-mail válido.");
                }
            }

            if (string.IsNullOrWhiteSpace(request.Categoria))
                return ResultadoPadrao<bool>.Falha("Selecione uma categoria.");

            if (string.IsNullOrWhiteSpace(request.Mensagem))
                return ResultadoPadrao<bool>.Falha("Digite sua mensagem.");

            if (request.Mensagem.Length > 1000)
                return ResultadoPadrao<bool>.Falha("A mensagem pode conter no máximo 1000 caracteres.");

            return ResultadoPadrao<bool>.Ok(true);
        }
    }
}