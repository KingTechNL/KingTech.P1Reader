using KingTech.P1Reader;
using KingTech.P1Reader.NuGet;
using KingTech.P1Reader.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configure<P1ReceiverSettings>();
builder.Services.AddSingleton<IP1Receiver, P1Receiver>();
builder.Services.AddSingleton<MetricService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Prometheus metrics
//Make sure this call is made before the call to UseEndPoints.
app.UseMetricServer();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//Start listening
app.Services.GetService<IP1Receiver>()?.Start();
app.Services.GetService<MetricService>()?.Start();

//Start web application.
app.Run();

//Shutdown.
app.Services.GetService<IP1Receiver>()?.Stop();
app.Services.GetService<MetricService>()?.Stop();