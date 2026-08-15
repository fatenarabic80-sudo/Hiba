namespace HeritageMarket.Web.Areas.Admin.ViewModels;

public class AdminUserListItemViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public bool IsLockedOut { get; set; }
    public DateTime CreatedAt { get; set; }
}
