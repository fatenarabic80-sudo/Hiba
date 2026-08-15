namespace HeritageMarket.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int ProductCount { get; set; }
}

public class CountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? FlagImageUrl { get; set; }
    public string? Description { get; set; }
    public int ProductCount { get; set; }
}
