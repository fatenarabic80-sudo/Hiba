using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Helpers;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ICountryService _countryService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ICountryService countryService,
        IWebHostEnvironment environment,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _countryService = countryService;
        _environment = environment;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, int pageNumber = 1)
    {
        var products = await _productService.GetForAdminAsync(pageNumber, 10, search);
        ViewData["Search"] = search;
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var model = new ProductFormViewModel();
        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        try
        {
            var imageUrl = await FileUploadHelper.SaveImageAsync(model.ImageFile, _environment, "products");

            await _productService.CreateAsync(new ProductEditDto
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                SKU = model.SKU,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
                CountryId = model.CountryId,
                Gender = model.Gender,
                Sizes = model.Sizes,
                ImageUrl = imageUrl
            });

            TempData["StatusMessage"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create product.");
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateDropdownsAsync(model);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetForEditAsync(id);
        if (product is null) return NotFound();

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            SKU = product.SKU,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CountryId = product.CountryId,
            Gender = product.Gender,
            Sizes = product.Sizes,
            ExistingImageUrl = product.ImageUrl
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        try
        {
            var imageUrl = await FileUploadHelper.SaveImageAsync(model.ImageFile, _environment, "products");

            await _productService.UpdateAsync(new ProductEditDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                SKU = model.SKU,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
                CountryId = model.CountryId,
                Gender = model.Gender,
                Sizes = model.Sizes,
                ImageUrl = imageUrl ?? model.ExistingImageUrl
            });

            TempData["StatusMessage"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update product {Id}.", id);
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateDropdownsAsync(model);
            return View(model);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        TempData["StatusMessage"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(ProductFormViewModel model)
    {
        var categories = await _categoryService.GetAllAsync();
        var countries = await _countryService.GetAllAsync();

        model.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        model.Countries = countries.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
    }
}
