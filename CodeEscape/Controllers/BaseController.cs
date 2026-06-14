using CodeEscape.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Resultado<T>(ResultadoPadrao<T> resultado)
        {
            return StatusCode(resultado.StatusCode, resultado);
        }
    }
}
