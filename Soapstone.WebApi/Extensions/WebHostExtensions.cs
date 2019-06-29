using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Soapstone.Data;

namespace Soapstone.WebApi.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost Seed(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
                using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
                    context.Database.Migrate();

            return host;
        }
    }
}