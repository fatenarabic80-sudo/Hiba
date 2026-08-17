using HeritageMarket.Application.DTOs;
using HeritageMarket.Domain.Enums;

namespace HeritageMarket.Web.ViewModels;

public class BooksGateViewModel
{
    public bool IsAuthenticated { get; set; }
    public BookAccessStatus? Status { get; set; }
    public string? AdminNote { get; set; }
    public IReadOnlyList<CountryDto> Countries { get; set; } = Array.Empty<CountryDto>();
    public BookAccessRequestFormViewModel Form { get; set; } = new();
}
