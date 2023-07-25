using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(logging => logging.AddOpenTelemetry(openTelemetryLoggerOptions =>
{
    openTelemetryLoggerOptions.SetResourceBuilder(
        ResourceBuilder.CreateEmpty()
            // Replace "GettingStarted" with the name of your application
            .AddService("GettingStarted")
            .AddAttributes(new Dictionary<string, object>
            {
                // Add any desired resource attributes here
                ["deployment.environment"] = "development"
            }));

    // Some important options to improve data quality
    openTelemetryLoggerOptions.IncludeScopes = true;
    openTelemetryLoggerOptions.IncludeFormattedMessage = true;

    openTelemetryLoggerOptions.AddOtlpExporter(exporter =>
    {
        // The full endpoint path is required here, when using
        // the `HttpProtobuf` protocol option.
        exporter.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
        // Optional `X-Seq-ApiKey` header for authentication, if required
        exporter.Headers = "X-Seq-ApiKey=abcde12345";
    });
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
