using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration setup
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Logger setup
var log = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

Log.Logger = log;

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Local Repo
builder.Services.AddSingleton<IUserService, UserService>(_ => new UserService(config.GetConnectionString("LocalDb")));
builder.Services.AddSingleton<IBillboardListingService, BillboardListingService>(_ => new BillboardListingService(config.GetConnectionString("LocalDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();