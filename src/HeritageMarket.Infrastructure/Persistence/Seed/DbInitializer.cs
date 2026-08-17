using HeritageMarket.Domain.Entities;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    private const string HomeCategory = "Home & Decoration";
    private const string AccessoriesCategory = "Accessories";
    private const string PhoneCoversCategory = "Phone Covers";
    private const string WearCategory = "Wear & Traditional Clothing";
    private const string BooksCategory = "Heritage Books";

    private static readonly Dictionary<string, string> CategorySkuCode = new()
    {
        [HomeCategory] = "HOME",
        [AccessoriesCategory] = "ACC",
        [PhoneCoversCategory] = "PHONE",
        [WearCategory] = "WEAR",
        [BooksCategory] = "BOOK"
    };

    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        foreach (var roleName in new[] { IdentityRoles.Admin, IdentityRoles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        const string adminEmail = "admin@heritagemarket.local";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Platform Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = HomeCategory, Description = "Heritage-inspired decor for the home.", IconUrl = "https://picsum.photos/seed/cat-home-decor/600/760" },
                new Category { Name = AccessoriesCategory, Description = "Traditional accessories with a modern edge.", IconUrl = "https://picsum.photos/seed/cat-accessories/600/760" },
                new Category { Name = PhoneCoversCategory, Description = "Heritage-pattern phone covers.", IconUrl = "https://picsum.photos/seed/cat-phone-covers/600/760" },
                new Category { Name = WearCategory, Description = "Garments inspired by traditional dress.", IconUrl = "https://picsum.photos/seed/cat-wear/600/760" },
                new Category { Name = BooksCategory, Description = "Famous heritage literature — unlocked after a short chat with our Heritage Guide.", IconUrl = "https://picsum.photos/seed/cat-books/600/760" }
            );
            await context.SaveChangesAsync();
        }

        if (!await context.Countries.AnyAsync())
        {
            foreach (var seedCountry in HeritageCatalogSeedData.Countries)
            {
                context.Countries.Add(new Country
                {
                    Name = seedCountry.Name,
                    Code = seedCountry.Code,
                    Region = seedCountry.Region,
                    Description = seedCountry.Description,
                    FlagImageUrl = $"https://picsum.photos/seed/country-{seedCountry.Code.ToLowerInvariant()}/400/500"
                });
            }
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var countryIds = await context.Countries.ToDictionaryAsync(c => c.Code, c => c.Id);
            var categoryIds = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            foreach (var seedCountry in HeritageCatalogSeedData.Countries)
            {
                var countryId = countryIds[seedCountry.Code];

                AddProducts(context, seedCountry.Home, HomeCategory, categoryIds, countryId, seedCountry.Code, 25);
                AddProducts(context, seedCountry.Accessories, AccessoriesCategory, categoryIds, countryId, seedCountry.Code, 30);
                AddProducts(context, seedCountry.PhoneCovers, PhoneCoversCategory, categoryIds, countryId, seedCountry.Code, 40);
                AddProducts(context, seedCountry.Wear, WearCategory, categoryIds, countryId, seedCountry.Code, 15);

                var bookIndex = 0;
                foreach (var book in seedCountry.Books)
                {
                    bookIndex++;
                    context.Products.Add(new Product
                    {
                        Name = $"{book.Title} — {book.Author}",
                        Description = book.Description,
                        Price = book.Price,
                        StockQuantity = 20,
                        SKU = $"{seedCountry.Code}-BOOK-{bookIndex:00}",
                        ImageUrl = $"https://picsum.photos/seed/prod-{seedCountry.Code.ToLowerInvariant()}-book-{bookIndex}/600/600",
                        CategoryId = categoryIds[BooksCategory],
                        CountryId = countryId
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }

    private static void AddProducts(
        ApplicationDbContext context,
        SeedProduct[] products,
        string categoryName,
        IReadOnlyDictionary<string, int> categoryIds,
        int countryId,
        string countryCode,
        int stock)
    {
        var skuCode = CategorySkuCode[categoryName];
        var index = 0;

        foreach (var product in products)
        {
            index++;
            context.Products.Add(new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = stock,
                SKU = $"{countryCode}-{skuCode}-{index:00}",
                ImageUrl = $"https://picsum.photos/seed/prod-{countryCode.ToLowerInvariant()}-{skuCode.ToLowerInvariant()}-{index}/600/600",
                CategoryId = categoryIds[categoryName],
                CountryId = countryId
            });
        }
    }
}
