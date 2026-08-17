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

    private static readonly Dictionary<string, string> CategoryImageKeyword = new()
    {
        [HomeCategory] = "handicraft,homedecor",
        [AccessoriesCategory] = "jewelry,accessory",
        [PhoneCoversCategory] = "phonecase",
        [WearCategory] = "traditionaldress,textile",
        [BooksCategory] = "book,literature"
    };

    private static string ProductImageUrl(string categoryName, string countryName, int lockSeed) =>
        $"https://loremflickr.com/600/600/{CategoryImageKeyword[categoryName]},{Uri.EscapeDataString(countryName.Replace(" ", string.Empty))}?lock={lockSeed}";

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
                new Category { Name = HomeCategory, Description = "Heritage-inspired decor for the home.", IconUrl = "https://loremflickr.com/600/760/handicraft,decor?lock=101" },
                new Category { Name = AccessoriesCategory, Description = "Traditional accessories with a modern edge.", IconUrl = "https://loremflickr.com/600/760/jewelry,artisan?lock=102" },
                new Category { Name = PhoneCoversCategory, Description = "Heritage-pattern phone covers.", IconUrl = "https://loremflickr.com/600/760/phonecase,pattern?lock=103" },
                new Category { Name = WearCategory, Description = "Garments inspired by traditional dress.", IconUrl = "https://loremflickr.com/600/760/traditional,clothing?lock=104" },
                new Category { Name = BooksCategory, Description = "Famous heritage literature from every corner of the world — chat with our Heritage Guide to explore a country's stories.", IconUrl = "https://loremflickr.com/600/760/books,library?lock=105" }
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
                    FlagImageUrl = $"https://flagcdn.com/w320/{seedCountry.Code.ToLowerInvariant()}.png"
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

                AddProducts(context, seedCountry.Home, HomeCategory, categoryIds, countryId, seedCountry.Code, seedCountry.Name, 25);
                AddProducts(context, seedCountry.Accessories, AccessoriesCategory, categoryIds, countryId, seedCountry.Code, seedCountry.Name, 30);
                AddProducts(context, seedCountry.PhoneCovers, PhoneCoversCategory, categoryIds, countryId, seedCountry.Code, seedCountry.Name, 40);
                AddProducts(context, seedCountry.Wear, WearCategory, categoryIds, countryId, seedCountry.Code, seedCountry.Name, 15);

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
                        ImageUrl = ProductImageUrl(BooksCategory, seedCountry.Name, StableSeed(seedCountry.Code, "BOOK", bookIndex)),
                        CategoryId = categoryIds[BooksCategory],
                        CountryId = countryId
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }

    private static int StableSeed(string countryCode, string skuCode, int index)
    {
        unchecked
        {
            var hash = 17;
            foreach (var c in countryCode + skuCode)
                hash = hash * 31 + c;
            hash = hash * 31 + index;
            return Math.Abs(hash);
        }
    }

    private static void AddProducts(
        ApplicationDbContext context,
        SeedProduct[] products,
        string categoryName,
        IReadOnlyDictionary<string, int> categoryIds,
        int countryId,
        string countryCode,
        string countryName,
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
                ImageUrl = ProductImageUrl(categoryName, countryName, StableSeed(countryCode, skuCode, index)),
                CategoryId = categoryIds[categoryName],
                CountryId = countryId,
                Gender = product.Gender,
                Sizes = product.Sizes
            });
        }
    }
}
