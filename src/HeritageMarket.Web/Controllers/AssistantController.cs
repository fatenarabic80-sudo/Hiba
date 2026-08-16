using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HeritageMarket.Web.Controllers;

[AllowAnonymous]
[EnableRateLimiting("assistant")]
public class AssistantController : Controller
{
    private readonly IAiAssistantService _aiAssistantService;

    public AssistantController(IAiAssistantService aiAssistantService)
    {
        _aiAssistantService = aiAssistantService;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask([FromBody] AiChatRequestViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Message))
            return BadRequest(new { message = "A non-empty message is required." });

        var request = new AiChatRequest
        {
            Message = model.Message,
            History = model.History
                .TakeLast(6)
                .Select(h => new AiChatMessageDto { Role = h.Role == "assistant" ? "assistant" : "user", Content = h.Content })
                .ToList()
        };

        var response = await _aiAssistantService.AskAsync(request, cancellationToken);
        return Json(new { reply = response.Reply, live = response.IsLiveAnswer });
    }
}
