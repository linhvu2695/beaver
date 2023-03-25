#nullable disable

using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                System.Console.WriteLine("---> Attempting to apply Migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"---> Could not run migrations: {ex.Message}");
                }
            }

            if (!context.Platforms.Any()) {
                System.Console.WriteLine("---> Seeding data...");

                context.Platforms.AddRange(
                    new Models.Platform() {
                        Name=".NET", 
                        Publisher="Microsoft",
                        Cost="Free"
                    },
                    new Models.Platform() {
                        Name="SQL Server Express", 
                        Publisher="Microsoft",
                        Cost="Free"
                    },
                    new Models.Platform() {
                        Name="Kubernetes", 
                        Publisher="Cloud Native Computing Network",
                        Cost="Free"
                    }
                );

                context.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("---> Data already populated");
            }
        }
    }
}