using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Electricity.Api.Extensions
{
    public static class SwaggerClientExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();

                c.MapType<decimal>(() => new OpenApiSchema { Type = "number", Format = "decimal" });

                c.CustomSchemaIds(obj => obj.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Electricity API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Email = ""
                    },
                    Description = ""
                });

                c.OperationFilter<AddOperationId>();

                c.CustomSchemaIds((type) => type.Name);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.DocumentTitle = "Electricity API";
                c.DocExpansion(DocExpansion.None);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.OAuthScopeSeparator(" ");
                c.RoutePrefix = string.Empty;
#if DEBUG
                c.DisplayRequestDuration();
#endif
            });

            return app;
        }
    }
    public class AddOperationId : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.OperationId = context.MethodInfo.Name;
        }
    }
}
