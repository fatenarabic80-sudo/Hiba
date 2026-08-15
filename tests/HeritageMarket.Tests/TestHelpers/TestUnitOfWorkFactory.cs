using HeritageMarket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Tests.TestHelpers;

public static class TestUnitOfWorkFactory {
    public static UnitOfWork Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return new UnitOfWork(context);
    }
}
