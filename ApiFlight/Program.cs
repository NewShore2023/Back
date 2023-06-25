using Business.Domain;
using Business.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion de interfaces
builder.Services.AddScoped<IFlight, FlightServices>();
//Conbfiguracion de cors
builder.Services.AddCors(options =>
{
	options.AddPolicy("politicasCors", app =>
	{
		app.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod();
	});
}
	);
//configuracion de Logger
Host.CreateDefaultBuilder(args)
		   .ConfigureWebHostDefaults(webBuilder =>
		   {
			   builder.Services.AddLogging(loggingBuilder =>
			   {
				   loggingBuilder.ClearProviders(); // Elimina los proveedores de registro existentes
				   loggingBuilder.AddSerilog(dispose: true); // Agrega Serilog como el proveedor de registro
			   });
		   })
		   .UseSerilog();
Log.Logger = new LoggerConfiguration()
	   .WriteTo.File("logs/archivo.log")
			.WriteTo.Console() // Configura el sink de salida a la consola
			.CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("politicasCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
