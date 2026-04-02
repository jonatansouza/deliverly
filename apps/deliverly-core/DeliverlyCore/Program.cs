using DeliverlyCore;
using DeliverlyCore.Consumers;
using DeliverlyCore.Infra;
using DeliverlyCore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
builder.Services.AddInfrastructure();

// Kafka handlers — add a new line here for each new topic
builder.Services.AddSingleton<IKafkaMessageHandler, TicketCreateHandler>();

builder.Services.AddHostedService<KafkaConsumerWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
