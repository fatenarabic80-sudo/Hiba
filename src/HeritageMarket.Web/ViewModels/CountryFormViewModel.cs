using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.ViewModels;

public class CountryFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Code { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(300)]
    public string? FlagImageUrl { get; set; }
}
