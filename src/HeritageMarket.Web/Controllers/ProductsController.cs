using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

public class ProductsController : Controller
{
    public const string HeritageBooksCategoryName = "Heritage Books";
    public const string WearCategoryName = "Wear & Traditional Clothing";

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly IReviewService _reviewService;
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        IReviewService reviewService,
        IWishlistService wishlistService,
        UserManager<ApplicationUser> userManager,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _reviewService = reviewService;
        _wishlistService = wishlistService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, int? countryId, int pageNumber = 1)
    {
        var categories = await _categoryService.GetAllAsync();

        var isWearCategory = categoryId.HasValue &&
            categories.FirstOrDefault(c => c.Id == categoryId)?.Name == WearCategoryName;

        var filter = new ProductFilter
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            CountryId = countryId,
            PageNumber = pageNumber,
            // Wear is grouped into Women/Men/Kids sections in the view, so it's shown as one
            // unbroken set rather than split awkwardly across pages.
            PageSize = isWearCategory ? 300 : 9
        };

        var model = new ProductCatalogViewModel
        {
            Products = await _productService.GetCatalogAsync(filter),
            Categories = categories,
            Countries = await _countryService.GetAllAsync(),
            Filter = filter
        };

        // Heritage Books behaves like every other category — freely browsable, no login or
        // approval required. The Heritage Guide just pops in with a friendly, non-blocking
        // greeting when you land here (see books-greeting.js), purely for the fun of it.
        ViewData["IsBooksCategory"] = categoryId.HasValue &&
            categories.FirstOrDefault(c => c.Id == categoryId)?.Name == HeritageBooksCategoryName;
        ViewData["IsWearCategory"] = isWearCategory;

        await SetWishlistedIdsAsync();
        return View(model);
    }

    /// <summary>
    /// A clean, filter-free showcase of everything one country offers — every product across every
    /// category, grouped by category — reached from the Home page's country cards.
    /// </summary>
    public async Task<IActionResult> Country(int id)
    {
        var country = await _countryService.GetByIdAsync(id);
        if (country is null) return NotFound();

        var products = await _productService.GetCatalogAsync(new ProductFilter { CountryId = id, PageSize = 500 });

        var model = new CountryShowcaseViewModel
        {
            Country = country,
            Products = products.Items
        };

        await SetWishlistedIdsAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetDetailAsync(id);
        if (product is null) return NotFound();

        var model = new ProductDetailsViewModel
        {
            Product = product,
            ReviewForm = new ReviewFormViewModel { ProductId = id },
            CanReview = User.Identity?.IsAuthenticated ?? false
        };

        await SetWishlistedIdsAsync();
        return View(model);
    }

    private async Task SetWishlistedIdsAsync()
    {
        if (!(User.Identity?.IsAuthenticated ?? false)) return;

        var userId = _userManager.GetUserId(User);
        if (userId is not null)
            ViewData["WishlistedIds"] = await _wishlistService.GetWishlistedProductIdsAsync(userId);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(ReviewFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Please provide a valid rating (1-5).";
            return RedirectToAction(nameof(Details), new { id = form.ProductId });
        }

        var userId = _userManager.GetUserId(User)!;

        try
        {
            var request = form.Adapt<CreateReviewRequest>();
            request.ApplicationUserId = userId;

            await _reviewService.AddReviewAsync(request);
            TempData["StatusMessage"] = "Thank you for your review!";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add review for product {ProductId}", form.ProductId);
            TempData["ErrorMessage"] = "We couldn't save your review. Please try again.";
        }

        return RedirectToAction(nameof(Details), new { id = form.ProductId });
    }
}
