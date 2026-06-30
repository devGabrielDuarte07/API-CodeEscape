using CodeEscape.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail; 

namespace CodeEscape.Service
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarEmail(string destinario, string assunto, string mensagem)
        {

            var envio = new MailMessage();
            envio.From = new MailAddress(_settings.Email, _settings.DisplayName);
            envio.To.Add(destinario);
            envio.Subject = assunto;
            envio.Body = mensagem;
            envio.IsBodyHtml = true;

            var smtp = new SmtpClient(_settings.Host, _settings.Port);
            smtp.Credentials = new NetworkCredential(_settings.Email, _settings.Password);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(envio);
        }

    }
}
