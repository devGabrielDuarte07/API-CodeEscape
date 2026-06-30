using CodeEscape.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : BaseController
    {

        private readonly UploadService _uploadService;

        public UploadController(UploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        public async Task<IActionResult> SalvarArquivo(IFormFile arquivo)
        {
            return Resultado(await _uploadService.SalvarAvatar(arquivo));
        }
    }
}
