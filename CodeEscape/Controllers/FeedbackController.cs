using CodeEscape.DTOs.Feedback;
using CodeEscape.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeEscape.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;
        public FeedbackController(FeedbackService feedbackServiced)
        {
            _feedbackService = feedbackServiced;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarFeedback([FromBody] FeedbackRequest request)
        {
            var resultado = await _feedbackService.EnviarFeedback(request);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
