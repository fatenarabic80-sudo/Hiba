namespace HeritageMarket.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? FlagImageUrl { get; set; }
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
