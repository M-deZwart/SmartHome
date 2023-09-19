using Microsoft.EntityFrameworkCore;
using Domain.Domain.Entities;
using Infrastructure.Infrastructure.EF;

public class CommonTestBase : IDisposable
{
    protected readonly SmartHomeContext Context;
    protected readonly Sensor Sensor;
    protected const string SENSOR_TITLE = "Bedroom";

    public CommonTestBase()
    {
        Context = CreateTestContext();
        Sensor = new Sensor(SENSOR_TITLE);
        Context.Sensors.Add(Sensor);
        Context.SaveChanges();
    }

    private DbContextOptions<SmartHomeContext> CreateNewInMemoryDatabase()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SmartHomeContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return optionsBuilder.Options;
    }

    private SmartHomeContext CreateTestContext()
    {
        var options = CreateNewInMemoryDatabase();
        var context = new SmartHomeContext(options);

        return context;
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}