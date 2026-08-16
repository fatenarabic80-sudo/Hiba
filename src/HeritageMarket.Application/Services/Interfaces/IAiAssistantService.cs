using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IAiAssistantService
{
    Task<AiChatResponse> AskAsync(AiChatRequest request, CancellationToken cancellationToken = default);
}
