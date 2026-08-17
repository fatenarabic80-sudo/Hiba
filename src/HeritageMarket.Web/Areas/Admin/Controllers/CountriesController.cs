using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class CountriesController : Controller
{
    private readonly ICountryService _countryService;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
    {
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var countries = await _countryService.GetAllAsync();
        return View(countries);
    }

    public IActionResult Create() => View(new CountryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CountryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _countryService.CreateAsync(new CountryDto { Name = model.Name, Code = model.Code, Region = model.Region, Description = model.Description, FlagImageUrl = model.FlagImageUrl, LandmarkImageUrl = model.LandmarkImageUrl });
        TempData["StatusMessage"] = "Country created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var country = await _countryService.GetByIdAsync(id);
        if (country is null) return NotFound();

        return View(new CountryFormViewModel { Id = country.Id, Name = country.Name, Code = country.Code, Region = country.Region, Description = country.Description, FlagImageUrl = country.FlagImageUrl, LandmarkImageUrl = country.LandmarkImageUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CountryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _countryService.UpdateAsync(new CountryDto { Id = model.Id, Name = model.Name, Code = model.Code, Region = model.Region, Description = model.Description, FlagImageUrl = model.FlagImageUrl, LandmarkImageUrl = model.LandmarkImageUrl });
            TempData["StatusMessage"] = "Country updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update country {Id}", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _countryService.DeleteAsync(id);
            TempData["StatusMessage"] = "Country deleted.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Could not delete country: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
