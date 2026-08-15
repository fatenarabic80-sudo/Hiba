using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers.Api;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Browse the product catalog with optional search, category, and country filters.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts([FromQuery] string? searchTerm, [FromQuery] int? categoryId, [FromQuery] int? countryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 12)
    {
        var result = await _productService.GetCatalogAsync(new ProductFilter
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            CountryId = countryId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return Ok(result);
    }

    /// <summary>Gets full details for a single product, including reviews.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetDetailAsync(id);
        return product is null ? NotFound() : Ok(product);
    }
}
