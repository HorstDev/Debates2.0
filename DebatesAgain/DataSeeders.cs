using DebatesApp.Data;

namespace DebatesAgain
{
    public static class DataSeeder
    {
        public static void Seed(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<DebatesDataContext>();
            context.Database.EnsureCreated();
        }
    }
}
