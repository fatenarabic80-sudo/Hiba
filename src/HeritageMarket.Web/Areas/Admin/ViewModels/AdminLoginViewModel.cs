using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.Areas.Admin.ViewModels;

public class AdminLoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
