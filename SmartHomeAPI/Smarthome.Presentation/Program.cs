using Application.Application;
using Smarthome.Infrastructure;
using Smarthome.Presentation.Middleware;
using System.Text.Json.Serialization;

var allowFrontEnd = "_allowFrontEnd";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(
        opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: allowFrontEnd,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowFrontEnd);

app.UseAuthorization();

app.UseExceptionMiddleware();

app.UseHttpLogging();

app.MapControllers();

app.Run();
