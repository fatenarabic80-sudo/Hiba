namespace HeritageMarket.Application.DTOs;

public class AiChatMessageDto
{
    public string Role { get; set; } = "user"; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
}

public class AiChatRequest
{
    public string Message { get; set; } = string.Empty;

    /// <summary>Prior turns of the conversation, oldest first. Kept short by the caller.</summary>
    public IReadOnlyList<AiChatMessageDto> History { get; set; } = Array.Empty<AiChatMessageDto>();
}

public class AiChatResponse
{
    public string Reply { get; set; } = string.Empty;

    /// <summary>True when the reply came from the live model; false when the local fallback answered instead.</summary>
    public bool IsLiveAnswer { get; set; }
}
