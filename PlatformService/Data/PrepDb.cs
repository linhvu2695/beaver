namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
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