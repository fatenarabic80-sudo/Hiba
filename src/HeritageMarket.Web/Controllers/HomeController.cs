using System.Diagnostics;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Models;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        IWishlistService wishlistService,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _wishlistService = wishlistService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        var booksCategoryId = categories.FirstOrDefault(c => c.Name == ProductsController.HeritageBooksCategoryName)?.Id;

        var model = new HomeIndexViewModel
        {
            FeaturedProducts = await _productService.GetFeaturedAsync(8, booksCategoryId),
            Categories = categories,
            Countries = await _countryService.GetAllAsync()
        };

        if (User.Identity?.IsAuthenticated ?? false)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is not null)
                ViewData["WishlistedIds"] = await _wishlistService.GetWishlistedProductIdsAsync(userId);
        }

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
