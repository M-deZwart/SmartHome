using MongoDB.Driver;
using SmartHomeAPI;

var builder = WebApplication.CreateBuilder(args);

// Configuration-settings
var configuration = builder.Configuration;
var databaseType = configuration.GetValue<string>("DatabaseType");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddApplicationServices();

if (databaseType == "Mongo")
{
    var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB");
    var mongoClient = new MongoClient(mongoConnectionString);
    var mongoDatabase = mongoClient.GetDatabase("smarthome-db");
    builder.Services.AddSingleton(mongoDatabase);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


