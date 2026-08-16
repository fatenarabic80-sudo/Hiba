using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.Areas.Admin.ViewModels;

public class AdminUserEditViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }
}
