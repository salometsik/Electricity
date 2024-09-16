using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Electricity.Api.Extensions
{
    public static class LoggingExtensions
    {
        public static void ConfigureInitialLogger()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            Log.Logger = new LoggerConfiguration().Config(conf).CreateLogger();
        }

        private static LoggerConfiguration Config(this LoggerConfiguration logConfig, IConfigurationRoot configuration)
        {
            return logConfig
                    .MinimumLevel.Is(LogEventLevel.Error)
                    .MinimumLevel.Override("Serilog.AspNetCore.RequestLoggingMiddleware", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(GetElasticSinkOptions(configuration))
                    ;
        }

        public static void UseCustomSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, logConfig) =>
                     logConfig.Config(builder.Configuration));
        }

        public static void UseCustomRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                //default: HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = async (diagnosticContext, httpContext) =>
                {

                    diagnosticContext.Set("RequestQueryString", httpContext.Request.QueryString.ToString());
                    diagnosticContext.Set("IdentityName", httpContext.User.Identity?.Name);
                    if (httpContext.Request.Body.Length < 20000 && httpContext.Request.Body.Length > 0)
                    {
                        httpContext.Request.Body.Position = 0;
                        diagnosticContext.Set("RequestBody", await new StreamReader(httpContext.Request.Body).ReadToEndAsync());
                    }
                };
            });
        }

        static ElasticsearchSinkOptions GetElasticSinkOptions(IConfigurationRoot configuration)
        {
            var nodes = configuration.GetSection("ElasticSearch:Nodes").Get<string[]>().Select(a => new Uri(a));
            var userName = configuration["ElasticSearch:Username"];
            var password = configuration["ElasticSearch:Password"];
            var indexPrefix = configuration["ElasticSearch:IndexPrefix"];

            return new(nodes)
            {
                AutoRegisterTemplate = true,
                TypeName = null,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat = $"{indexPrefix}-{DateTime.UtcNow:yyyy-MM-dd}",
                ModifyConnectionSettings = x => x
                .BasicAuthentication(userName, password)
                .ServerCertificateValidationCallback((o, s, d, f) => true)
            };
        }
    }
}
