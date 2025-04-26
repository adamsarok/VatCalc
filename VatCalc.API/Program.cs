using HealthChecks.UI.Client;
using VatCalc.API;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddCarter();
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => {
	config.RegisterServicesFromAssembly(assembly);
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => {
	options.AddPolicy("AllowSpecificOrigin",
		builder => builder.AllowAnyOrigin()
						  .AllowAnyHeader()
						  .AllowAnyMethod());
});
builder.Services.AddHealthChecks();

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
}
app.MapCarter();
app.UseExceptionHandler();
app.UseCors(x => x.AllowAnyHeader()
	.AllowAnyMethod()
	.AllowCredentials()
	.WithOrigins("http://localhost"));
app.UseHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions {
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

public partial class Program { } //to enable system tests with WebApplicationFactory<Program>