using CodeEscape.Common;
using CodeEscape.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Service
{
    public class UploadService
    {
        private readonly IWebHostEnvironment env;


        public UploadService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<ResultadoPadrao<string>> SalvarAvatar([FromForm] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return ResultadoPadrao<string>.Falha("Nenhum arquivo enviado.", 400);
            }

            if (!arquivo.ContentType.StartsWith("image/"))
            {
                return ResultadoPadrao<string>.Falha("O arquivo deve ser uma imagem.", 400);
            }

            var extensao = Path.GetExtension(arquivo.FileName);

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!extensoesPermitidas.Contains(extensao.ToLower()))
            {
                return ResultadoPadrao<string>.Falha("Formato de imagem inválido.", 400);
            }

            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";

            var pasta = Path.Combine(
                env.WebRootPath,
                "uploads",
                "avatars"
            );

            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }

            var caminhoArquivo = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var avatarUrl = $"/uploads/avatars/{nomeArquivo}";

            return ResultadoPadrao<string>.Ok(avatarUrl);
        }
    }
}
