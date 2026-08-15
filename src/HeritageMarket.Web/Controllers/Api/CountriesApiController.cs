using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers.Api;

[ApiController]
[Route("api/countries")]
[Produces("application/json")]
public class CountriesApiController : ControllerBase
{
    private readonly ICountryService _countryService;

    public CountriesApiController(ICountryService countryService)
    {
        _countryService = countryService;
    }

    /// <summary>Lists all heritage countries represented in the catalog.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CountryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountries() => Ok(await _countryService.GetAllAsync());
}
