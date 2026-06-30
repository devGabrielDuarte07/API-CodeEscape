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
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService) 
        {
            _usuarioService = usuarioService;
        }


        [HttpPost]
        public IActionResult CriarUsuario(CriarUsuarioRequest dto)
        {
            return Resultado(_usuarioService.CriarUsuario(dto));
        }

        [Authorize]
        [HttpGet("perfil")]
        public IActionResult Perfil()
        {
            return Resultado(_usuarioService.DadosPerfil());
        }

        [Authorize]
        [HttpPut("avatar")]
        public IActionResult AtualizarAvatar(AtualizarAvatarRequest dto)
        {
            return Resultado(_usuarioService.AtualizarAvatar(dto));
        }
    }
}