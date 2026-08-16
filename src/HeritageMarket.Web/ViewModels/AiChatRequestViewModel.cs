using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.ViewModels;

public class AiChatMessageViewModel
{
    public string Role { get; set; } = "user";
    public string Content { get; set; } = string.Empty;
}

public class AiChatRequestViewModel
{
    [Required, StringLength(800)]
    public string Message { get; set; } = string.Empty;

    public List<AiChatMessageViewModel> History { get; set; } = new();
}
