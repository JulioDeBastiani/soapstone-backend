using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Soapstone.Data;
using Soapstone.Domain;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.Security;
using Soapstone.WebApi.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace Soapstone.WebApi
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(op => op
                .UseMySql(_config.GetConnectionString("Soapstone")));

            services.AddScoped<IRepository<User>, GenericRepository<User>>();
            services.AddScoped<IRepository<Post>, GenericRepository<Post>>();
            services.AddScoped<IRepository<Upvote>, GenericRepository<Upvote>>();
            services.AddScoped<IRepository<Downvote>, GenericRepository<Downvote>>();
            services.AddScoped<IRepository<SavedPost>, GenericRepository<SavedPost>>();
            services.AddScoped<IRepository<Report>, GenericRepository<Report>>();

            services.AddScoped<PostsService>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var tokenSettings = new TokenSettings();
            var tokenConfigurator = new ConfigureFromConfigurationOptions<TokenSettings>(_config.GetSection("TokenSettings"));
            tokenConfigurator.Configure(tokenSettings);

            var key = Encoding.ASCII.GetBytes(tokenSettings.SecretKey);

            services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    // o.TokenValidationParameters.IssuerSigningKey = rsaKeyManager.RestoreKey();
                    o.TokenValidationParameters.ValidAudience = tokenSettings.Audience;
                    o.TokenValidationParameters.ValidIssuer = tokenSettings.Issuer;
                    // o.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    // o.TokenValidationParameters.ValidateAudience = true;
                    // o.TokenValidationParameters.ValidateLifetime = true;
                    // o.TokenValidationParameters.ValidateIssuer = true;
                    // o.RequireHttpsMetadata = true;
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(o =>
                {
                    o.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        // .RequireClaim("user-id")
                        .Build();
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Soapstone-Backend", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // TODO tokens
            // TODO serilog
            // TODO mock mysql
            // TODO first migration
            // TODO post lifespan on map
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Soapstone Backend v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
            app.UseAuthentication();
        }
    }
}
