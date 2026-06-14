using CodeEscape.DTOs.GameSession;
using CodeEscape.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionsController : BaseController
    {
        private readonly GameSessionsService _gameSessionsService;

        public GameSessionsController(GameSessionsService gameSessionsService)
        {
            _gameSessionsService = gameSessionsService;
        }

        [Authorize]
        [HttpPost("Start")]
        public IActionResult Start(StartGameSessionRequest dto)
        {
            return Resultado(_gameSessionsService.Start(dto));
        }

        [Authorize]
        [HttpPost("{id}/enigma-atual")]
        public IActionResult EnigmaAtual(int id)
        {
            return Resultado(_gameSessionsService.ObterEnigmaAtual(id));
        }

        [Authorize]
        [HttpPost("Responder")]
        public IActionResult ResponderEnigma(ResponderEnigmaRequest dto)
        {
            return Resultado(_gameSessionsService.ResponderEnigma(dto));
        }                      
    }
}
