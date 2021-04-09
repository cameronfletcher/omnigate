namespace Omnibill.WebAPI
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        //ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "tc",
                        ValidAudience = "api",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["secret"]))
                    });

            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            services.AddControllers(
                options =>
                { });

            services.AddSwaggerGen(setup =>
            {
                var apiInfo = new OpenApiInfo
                {
                    Title = "Omnibill Web API",
                    Version = "v1",
                    Description = "Web API for the Omnibill web service",
                    Contact = new OpenApiContact
                    {
                        Name = "Trendcommerce",
                        Url = new Uri("https://trendcommerce.ch/"),
                    },
                };

                var secrityDefinition = new OpenApiSecurityScheme
                {
                    Name = "Authorization", // change?
                    Scheme = JwtBearerDefaults.AuthenticationScheme, // lowercase?
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { secrityDefinition, Array.Empty<string>() }
                };

                setup.SwaggerDoc("v1", apiInfo);
                setup.AddSecurityDefinition("Bearer", secrityDefinition);
                setup.AddSecurityRequirement(securityRequirement);
                setup.OperationFilter<RequiredHeadersFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OmnibillWebAPI v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private class RequiredHeadersFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (!context.ApiDescription.HttpMethod.Equals("POST") &&
                    !context.ApiDescription.HttpMethod.Equals("PUT") &&
                    !context.ApiDescription.HttpMethod.Equals("PATCH") &&
                    !context.ApiDescription.HttpMethod.Equals("DELETE"))
                {
                    return;
                }

                if (operation.Parameters == null)
                {
                    operation.Parameters = new List<OpenApiParameter>();
                }

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "Operation-ID",
                        In = ParameterLocation.Header,
                        Required = false,
                        Style = ParameterStyle.Simple,
                        Description = "An identifier that will be used to enforce idempotency.",
                    });
            }
        }
    }
}
