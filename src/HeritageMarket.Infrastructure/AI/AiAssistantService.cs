using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HeritageMarket.Infrastructure.AI;

public class AiAssistantService : IAiAssistantService
{
    private readonly HttpClient _httpClient;
    private readonly IProductService _productService;
    private readonly AiAssistantSettings _settings;
    private readonly ILogger<AiAssistantService> _logger;

    public AiAssistantService(
        HttpClient httpClient,
        IProductService productService,
        IOptions<AiAssistantSettings> settings,
        ILogger<AiAssistantService> logger)
    {
        _httpClient = httpClient;
        _productService = productService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<AiChatResponse> AskAsync(AiChatRequest request, CancellationToken cancellationToken = default)
    {
        var message = (request.Message ?? string.Empty).Trim();
        if (message.Length > _settings.MaxMessageLength)
            message = message[.._settings.MaxMessageLength];

        if (message.Length == 0)
            return new AiChatResponse { Reply = "Ask me anything about our heritage products, shipping, or returns!" };

        if (!_settings.IsLiveEnabled)
            return new AiChatResponse { Reply = FallbackAnswer(message), IsLiveAnswer = false };

        try
        {
            var reply = await AskLiveAsync(message, request.History, cancellationToken);
            return new AiChatResponse { Reply = reply, IsLiveAnswer = true };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Live AI assistant call failed; falling back to rule-based answer.");
            return new AiChatResponse { Reply = FallbackAnswer(message), IsLiveAnswer = false };
        }
    }

    private async Task<string> AskLiveAsync(string message, IReadOnlyList<AiChatMessageDto> history, CancellationToken cancellationToken)
    {
        var systemPrompt = await BuildSystemPromptAsync();

        var messages = new JsonArray();
        foreach (var turn in history.TakeLast(_settings.MaxHistoryMessages))
        {
            var role = turn.Role == "assistant" ? "assistant" : "user";
            messages.Add(new JsonObject { ["role"] = role, ["content"] = turn.Content });
        }
        messages.Add(new JsonObject { ["role"] = "user", ["content"] = message });

        var payload = new JsonObject
        {
            ["model"] = _settings.Model,
            ["max_tokens"] = _settings.MaxTokens,
            ["system"] = systemPrompt,
            ["messages"] = messages
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages")
        {
            Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
        };
        httpRequest.Headers.Add("x-api-key", _settings.ApiKey);
        httpRequest.Headers.Add("anthropic-version", "2023-06-01");

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var json = JsonNode.Parse(body);
        var text = json?["content"]?.AsArray()
            .Where(block => block?["type"]?.GetValue<string>() == "text")
            .Select(block => block!["text"]!.GetValue<string>())
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException("The model returned an empty response.");

        return text.Trim();
    }

    private async Task<string> BuildSystemPromptAsync()
    {
        var catalog = await _productService.GetCatalogAsync(new ProductFilter { PageNumber = 1, PageSize = 50 });
        var catalogSummary = string.Join('\n', catalog.Items.Select(p =>
            $"- {p.Name} ({p.CountryName}, {p.CategoryName}, {p.Price:C}){(p.StockQuantity <= 0 ? " [out of stock]" : "")}"));

        return
            "You are the Heritage Guide, a warm and knowledgeable shopping assistant for Heritage Marketplace, " +
            "an online store selling heritage-inspired Home & Decoration, Accessories, Phone Covers, Wear & " +
            "Traditional Clothing, and Heritage Books from cultures around the world. Answer questions about " +
            "products, their cultural origin, materials, sizing, shipping, returns, and site navigation. Keep " +
            "answers concise (2-4 sentences), warm, and specific. This is the current product catalog:\n" +
            catalogSummary +
            "\n\nGeneral policies: standard shipping takes 5-9 business days; returns are accepted within 30 " +
            "days of delivery. If asked something unrelated to the marketplace, answer briefly and steer back " +
            "to how Heritage Marketplace can help.";
    }

    private static string FallbackAnswer(string message)
    {
        var text = message.ToLowerInvariant();

        if (text.Contains("ship"))
            return "Standard shipping takes 5-9 business days. You'll receive tracking information once your order ships.";
        if (text.Contains("return") || text.Contains("refund"))
            return "You can return unopened, unused items within 30 days of delivery for a full refund — just start a return from your Order Details page.";
        if (text.Contains("handmade") || text.Contains("material") || text.Contains("made"))
            return "Many of our pieces are handcrafted using traditional techniques, so slight variations in pattern or finish are part of the craft, not a flaw. Check each product's description for specific materials.";
        if (text.Contains("size") || text.Contains("fit"))
            return "Sizing varies by product — check the product description for exact details, and feel free to ask about a specific item.";
        if (text.Contains("pay") || text.Contains("checkout") || text.Contains("order"))
            return "You can add items to your cart and check out directly on the site. Once placed, you can track your order status from \"My Orders\".";

        return "I'm currently running in offline mode and couldn't reach the live assistant. I can still help with general questions about shipping, returns, materials, or sizing — or browse our Shop page to explore products by category and country.";
    }
}
