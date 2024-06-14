using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Local Repo
builder.Services.AddSingleton<IVehicleService, VehicleService>(_ => new VehicleService(config.GetConnectionString("LocalDb")));
builder.Services.AddSingleton<IUserService, UserService>(_ => new UserService(config.GetConnectionString("LocalDb")));

// Logger setup
var log = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();
    
Log.Logger = log;

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