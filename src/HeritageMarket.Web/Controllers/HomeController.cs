using System.Diagnostics;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Web.Models;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        ILogger<HomeController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel
        {
            FeaturedProducts = await _productService.GetFeaturedAsync(8),
            Categories = await _categoryService.GetAllAsync(),
            Countries = await _countryService.GetAllAsync()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ActionName("NotFound")]
    public IActionResult PageNotFound()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
