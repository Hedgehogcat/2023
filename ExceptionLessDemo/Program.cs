using Exceptionless;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;


var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Exceptionless()
            .CreateLogger();

Log.Information("»’÷æ");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
ExceptionlessClient.Default.Configuration.ApiKey = builder.Configuration.GetSection("Exceptionless:ApiKey").Value;
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
