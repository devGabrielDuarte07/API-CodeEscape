using CodeEscape.DTOs.Usuario;
using CodeEscape.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : BaseController
    {
        private readonly CadastroService _cadastroService;

        public UsuarioController(CadastroService cadastroService) 
        { 
            _cadastroService = cadastroService;
        }


        [HttpPost]
        public IActionResult CriarUsuario(CriarUsuarioRequest dto)
        {
            return Resultado(_cadastroService.CriarUsuario(dto));
        }

    }
}