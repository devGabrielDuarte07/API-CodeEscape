using CodeEscape.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : BaseController
    {
        private readonly RankingService _rankingService;

        public RankingController(RankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [HttpGet("sala/{roomId}")]
        public IActionResult RankingPorSala(int roomId)
        {
            return Resultado(_rankingService.RankingPorSala(roomId));
        }
    }
}
