using Infrastructure.Context;
using Infrastructure.Models;

namespace WebApi
{
    public static class ServiceCollectionExtensions
    {
        internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
        {
            using var ServicesScope = app.ApplicationServices.CreateScope();
            
            var seeders = ServicesScope.ServiceProvider.GetServices<ApplicationDbSeeder>();
            
            foreach (var seeder in seeders)
            {
                seeder.SeedDatabaseAsync().GetAwaiter().GetResult();
            }

            return app;
        }


        internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

    }
}
