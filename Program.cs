using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register EF Core In-Memory database.
builder.Services.AddDbContext<LocationContext>(options =>
    options.UseInMemoryDatabase("Locations"));

// Add API explorer and Swagger generator.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// POST endpoint: receives and stores location data.
app.MapPost("/api/locations", async (LocationRecord record, LocationContext db) =>
{
    db.LocationRecords.Add(record);
    await db.SaveChangesAsync();
    return Results.Created($"/api/locations/{record.Id}", record);
});

// GET endpoint: returns all stored location records.
app.MapGet("/api/locations", async (LocationContext db) =>
{
    var records = await db.LocationRecords.ToListAsync();
    return Results.Ok(records);
});

app.Run();

// Data model representing a location record.
public class LocationRecord
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}

// DbContext for the in-memory store.
public class LocationContext : DbContext
{
    public LocationContext(DbContextOptions<LocationContext> options)
        : base(options)
    {
    }

    public DbSet<LocationRecord> LocationRecords { get; set; } = null!;
}
