using Bunt.Core;
using Bunt.Core.Domain.Queries;
using Bunt.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Bunt.Core.Security;
using Microsoft.AspNetCore.Http;
using Bunt.Web.Security;
using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using System;
using Serilog.ExtensionMethods;

namespace Bunt.Web
{
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
            var logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(Configuration.GetConnectionString("BuntDb"), "Log")
                .WriteTo.ApplicationInsights("bfe8faf2-332f-4d95-8d51-b658caed73a3", ConvertLogEventsToCustomEventTelemetry)
                .Enrich.FromLogContext()
                .CreateLogger();

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            services.AddSingleton<ILogger>(logger);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AccessBehavior<,>));
            services.AddTransient<IConnectionFactory>(provider => new SqlConnectionFactory(Configuration.GetConnectionString("BuntDb")));
            services.AddDbContext<BuntDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BuntDb")));
            services.AddMediatR(typeof(ListaBuntladeStallen.Handler));
            services.AddTransient<IAuthorizationHandler, BuntWebLoginRequirementHandler>();
            services.AddTransient(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddTransient<ICurrentUser, ClaimsPrincipalCurrentUser>();
            services.AddTransient<IAccessClient>(provider => new HardcodedAccessClient(new AccessUser { UserId = 0 }));

            services.AddMvc(options =>
                        {
                            var builder = new AuthorizationPolicyBuilder()
                            .AddRequirements(new BuntWebLoginRequirement())
                            .RequireAuthenticatedUser();
                            options.Filters.Add(new AuthorizeFilter(builder.Build()));
                        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private static ITelemetry ConvertLogEventsToCustomEventTelemetry(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var telemetry = logEvent.ToDefaultEventTelemetry(
                formatProvider,
                includeLogLevelAsProperty: true,
                includeRenderedMessageAsProperty: true,
                includeMessageTemplateAsProperty: true);
            telemetry.Name = logEvent.RenderMessage();
            return telemetry;
        }
    }
}
