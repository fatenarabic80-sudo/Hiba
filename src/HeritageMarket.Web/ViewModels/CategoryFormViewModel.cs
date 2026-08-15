using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.ViewModels;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(300)]
    public string? IconUrl { get; set; }
}
