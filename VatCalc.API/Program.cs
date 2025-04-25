var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config => {
	config.RegisterServicesFromAssembly(assembly);
});

var app = builder.Build();

app.MapCarter();

app.Run();

public partial class Program { } //for system tests