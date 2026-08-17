using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeritageMarket.Web.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required, Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Required, Range(0, 100000)]
    public int StockQuantity { get; set; }

    [Required, StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "Please select a category.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Please select a country.")]
    public int CountryId { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(100)]
    public string? Sizes { get; set; }

    public string? ExistingImageUrl { get; set; }

    public IFormFile? ImageFile { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Countries { get; set; } = Enumerable.Empty<SelectListItem>();
}
