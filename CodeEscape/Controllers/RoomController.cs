using CodeEscape.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : BaseController
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        { 
            _roomService = roomService;
        }

        [HttpGet]
        public IActionResult ListarSalas()
        {
            return Resultado(_roomService.ListarSalas());
        }

        [HttpGet("{id}")]
        public IActionResult ListarSalaPorId(int id)
        {
            return Resultado(_roomService.ListarSalasPorId(id));
        }
    }      
}
