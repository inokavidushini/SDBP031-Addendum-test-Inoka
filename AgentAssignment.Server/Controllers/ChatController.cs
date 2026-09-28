using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AgentAssignment.Server.Contracts;

namespace AgentAssignment.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IChatCompletionService _chatService;

        public ChatController(IChatCompletionService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("/v1/chat/completions")]
        public async Task<IActionResult> Complete(ChatCompletionRequest request)
        {
            if (request.Messages == null || request.Messages.Count == 0)
            {
                return BadRequest(new ApiErrorResponse(
        new ApiErrorBody(
            "Messages are required.",
            "invalid_request"
        )
    ));
            }

            if (!_chatService.IsReady)
            {
                return StatusCode(503, new ApiErrorResponse(
    new ApiErrorBody(
        "The service is still starting.",
        "service_unavailable"
    )
));
            }
            try
            {
                var response = await _chatService.CompleteAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse(
    new ApiErrorBody(
        ex.Message,
        "internal_error"
    )
));
            }
        }
            [HttpGet("/health")]
            public IActionResult Health()
            {
                if (_chatService.IsReady)
                {
                    return Ok(new { status = "ready" });
                }
                return StatusCode(503, new { status = "starting" });
            }
        }
    }


