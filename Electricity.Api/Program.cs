using Electricity.Api.Extensions;
using Electricity.Application;
using Electricity.Persistence;

//LoggingExtensions.ConfigureInitialLogger();

var builder = WebApplication.CreateBuilder(args);

//builder.UseCustomSerilog();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

//builder.Services.AddLogging(logging => logging.AddConsole());
builder.Logging.Configure(o =>
{

});

builder.Services.AddAuthorization();

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseCustomRequestLogging();

app.UseSwaggerDocumentation();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
