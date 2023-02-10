using Flurl.Http.Configuration;
using KingTech.P1Reader;
using KingTech.P1Reader.Broker;
using KingTech.P1Reader.Message;
using KingTech.P1Reader.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add controller endpoints.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register P1 receiver.
builder.Configure<P1ReceiverSettings>(new P1ReceiverSettings());
builder.Services.AddSingleton<IP1Receiver, BrokerP1Receiver>();
builder.Services.AddSingleton<IMessageBroker<P1Message>, GenericMessageBroker<P1Message>>();
// Register prometheus metrics service.
builder.Services.AddSingleton<MetricService>();
// Register DSMR reader publisher.
builder.Configure<DsmrReaderPublisherSettings>(new DsmrReaderPublisherSettings());
builder.Services.AddSingleton<IFlurlClientFactory, DefaultFlurlClientFactory>();
builder.Services.AddSingleton<DsmrReaderPublisherService>();

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

//Set start and stop events.
var lifeTime = app.Services.GetService<IHostApplicationLifetime>();
lifeTime?.ApplicationStarted.Register(() => {
    app.Services.GetService<IP1Receiver>()?.Start();
    app.Services.GetService<MetricService>()?.Start();
    app.Services.GetService<DsmrReaderPublisherService>()?.Start();
});
lifeTime?.ApplicationStopping.Register(() => {
    app.Services.GetService<IP1Receiver>()?.Stop();
    app.Services.GetService<MetricService>()?.Stop();
    app.Services.GetService<DsmrReaderPublisherService>()?.Stop();
});

//Start web application.
app.Run();