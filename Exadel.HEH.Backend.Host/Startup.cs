using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Exadel.HEH.Backend.BusinessLogic.Extensions;
using Exadel.HEH.Backend.BusinessLogic.Options;
using Exadel.HEH.Backend.BusinessLogic.Services;
using Exadel.HEH.Backend.BusinessLogic.Validators;
using Exadel.HEH.Backend.Host.Extensions;
using Exadel.HEH.Backend.Host.Identity;
using Exadel.HEH.Backend.Host.Infrastructure;
using Exadel.HEH.Backend.Host.SwaggerFilters;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.MemoryStorage;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OData.Swagger.Services;
using Serilog;

namespace Exadel.HEH.Backend.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var authority = Configuration["Authority"];

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddFluentValidation(validation =>
                {
                    validation.RegisterValidatorsFromAssemblyContaining<FavoritesValidator>();
                });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddOData().EnableApiVersioning();
            services.AddODataApiExplorer(options => options.GroupNameFormat = "'v'VVV");

            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";
                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"Happy Exadel Hours API {groupName}",
                    Version = groupName
                });
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(authority + "/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "heh_api", "Full access to HEH Api" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.OperationFilter<DefaultValuesOperationFilter>();
                options.OperationFilter<HideApiVersionOperationFilter>();
                options.OperationFilter<CountParameterOperationFilter>();
            });

            services.AddOdataSwaggerSupport();

            services.AddHttpContextAccessor();
            services.AddUserProvider();
            services.AddMethodProvider();
            services.AddRepositories(Configuration);
            services.AddCrudServices();
            services.AddBusinessServices(Environment);
            services.AddValidators();
            services.AddValidationServices();
            services.AddSingleton(MapperExtensions.Mapper);

            services.AddIdentityServer()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddUsers()
                .AddDeveloperSigningCredential();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = authority;
                    options.ApiName = "heh_api";
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsForUI",
                    builder =>
                    {
                        builder.WithOrigins(Configuration["CorsOrigins"].Split(','))
                            .AllowCredentials()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            services.Configure<EmailOptions>(Configuration.GetSection(EmailOptions.EmailSettings));
        }

        public void Configure(IApplicationBuilder app, VersionedODataModelBuilder modelBuilder, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHangfireDashboard();
            }
            else
            {
                app.UseHttpStatusExceptionHandler();
            }

            app.UseHangfireServer();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseForwardedHeaders(ForwardedHeadersSettings.Options);

            app.UseODataRouting();
            app.UseRouting();

            app.UseCors("CorsForUI");

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Filter().Count().OrderBy().Select().Expand();
                endpoints.MapVersionedODataRoute("odata", "odata", modelBuilder.GetEdmModels());
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Happy Exadel Hours API V1");
                options.OAuthClientId("HEHApiClient");
                options.OAuthAppName("Happy Exadel Hours Api client");
                options.RoutePrefix = string.Empty;
            });

            SeedIdentityData.InitializeDatabaseAsync(app).Wait();
            app.ApplicationServices.GetService<SchedulerService>()?.Start();
        }
    }
}