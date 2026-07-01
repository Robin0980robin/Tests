using NUnit.Framework;

namespace Api.FunctionalTests.Infrastructure;

public abstract class ApiFunctionalTestBase
{
    protected readonly SqliteTestDatabase Database = new();
    protected ApiWebApplicationFactory Factory = null!;
    protected HttpClient Client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        await Database.InitializeAsync();
        Factory = new ApiWebApplicationFactory(Database.Connection);
        Client = Factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        Client.Dispose();
        Factory.Dispose();
        await Database.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await Database.ResetAsync();
    }
}
