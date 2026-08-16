namespace HeritageMarket.Infrastructure.AI;

public class AiAssistantSettings
{
    public const string SectionName = "AiAssistant";

    /// <summary>Anthropic API key. Leave empty to run in fallback-only (rule-based) mode.</summary>
    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "claude-haiku-4-5-20251001";
    public int MaxTokens { get; set; } = 300;
    public int MaxHistoryMessages { get; set; } = 6;
    public int MaxMessageLength { get; set; } = 800;

    public bool IsLiveEnabled => !string.IsNullOrWhiteSpace(ApiKey);
}
