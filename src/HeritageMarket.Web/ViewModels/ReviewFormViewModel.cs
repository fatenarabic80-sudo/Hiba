using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.ViewModels;

public class ReviewFormViewModel
{
    [Required]
    public int ProductId { get; set; }

    [Required, Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; } = 5;

    [StringLength(1000)]
    public string? Comment { get; set; }
}
