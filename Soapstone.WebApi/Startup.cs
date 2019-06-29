using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Soapstone.Data;
using Soapstone.Domain;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.Security;
using Soapstone.WebApi.Services;
using Soapstone.WebApi.Settings;
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
                .UseMySql(_config.GetConnectionString("Soapstone"), o => o.ServerVersion(new Version(5, 5), ServerType.MySql)));

            services.AddScoped<IRepository<User>, GenericRepository<User>>();
            services.AddScoped<IRepository<Post>, GenericRepository<Post>>();
            services.AddScoped<IRepository<Upvote>, GenericRepository<Upvote>>();
            services.AddScoped<IRepository<Downvote>, GenericRepository<Downvote>>();
            services.AddScoped<IRepository<SavedPost>, GenericRepository<SavedPost>>();
            services.AddScoped<IRepository<Report>, GenericRepository<Report>>();

            services.AddScoped<PostService>();
            services.AddScoped<ImageUploadService>();
            services.AddScoped<TokenService>();

            var imageUploadSettings = new ImageUploadSettings();
            var imageUploadConfigurator = new ConfigureFromConfigurationOptions<ImageUploadSettings>(_config.GetSection("ImageUploadSettings"));
            imageUploadConfigurator.Configure(imageUploadSettings);
            services.AddSingleton(imageUploadSettings);

            var tokenSettings = new TokenSettings();
            var tokenConfigurator = new ConfigureFromConfigurationOptions<TokenSettings>(_config.GetSection("TokenSettings"));
            tokenConfigurator.Configure(tokenSettings);
            services.AddSingleton<TokenSettings>(tokenSettings);

            var key = Convert.FromBase64String(tokenSettings.SecretKey);

            services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenSettings.Issuer,
                        ValidateIssuer = true,
                        ValidAudience = tokenSettings.Audience,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            services.AddAuthorization(o =>
                {
                    o.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireClaim(DefaultClaims.UserId)
                        .Build();
                });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Soapstone-Backend", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT", 
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() },
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // TODO serilog
            // TODO post lifespan on map
            // TODO exception message overhaul
            // TODO documentation overhaul
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

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
