using Microsoft.EntityFrameworkCore;
using MikesPaging.WebApi.Data;

namespace MikesPaging.WebApi;

public static class Extensions
{
    public static void MigrateDatabase(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WebApiExampleDbContext>();
        dbContext.Database.Migrate();
    }
}