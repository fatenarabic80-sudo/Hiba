using System.ComponentModel.DataAnnotations;

namespace HeritageMarket.Web.ViewModels;

public class BookAccessRequestFormViewModel
{
    [Required(ErrorMessage = "Tell us a little about why you like reading."), StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pick a country whose books you'd like to read.")]
    public string PreferredCountry { get; set; } = string.Empty;
}
