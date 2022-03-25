using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateBootstrapLogger();

try
{
  Log.Information("Starting web host");
  var builder = WebApplication.CreateBuilder(args);
  builder.Services.AddRazorPages();
  builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
  );
  // Add services to the container.

  builder.Services.AddControllers();
  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  var app = builder.Build();

  // Configure the HTTP request pipeline.

  app.UseSerilogRequestLogging(configure =>{configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";});
  if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
  app.UseHttpsRedirection();
  app.UseAuthorization();
  app.MapControllers();
  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Host terminated unexpectedly");
  return 1;
}
finally
{
  Log.CloseAndFlush();
}
return 0;