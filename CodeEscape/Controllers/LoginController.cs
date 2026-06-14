using CodeEscape.DTOs.Auth;
using CodeEscape.Models;
using CodeEscape.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : BaseController
    {

        private readonly LoginService _loginService;
     
        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public IActionResult Login(LoginRequest dto)
        {
            return Resultado(_loginService.Login(dto));
        }

    
    }
}
