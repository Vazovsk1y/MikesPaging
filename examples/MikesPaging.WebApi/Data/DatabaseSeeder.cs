using Bogus;
using Microsoft.EntityFrameworkCore;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Data;

public static class DatabaseSeeder
{
    private static readonly Faker<User> _userFaker = new Faker<User>()
        .RuleFor(e => e.Id, e => Guid.NewGuid())
        .RuleFor(e => e.FullName, e => e.Name.FullName())
        .RuleFor(e => e.Age, e => e.Random.Int(0, 100))
        .RuleFor(e => e.Created, e => e.Date.BetweenOffset(DateTimeOffset.MinValue, DateTimeOffset.UtcNow));

    private const int UsersSeedCount = 50;

    public static void Seed(this ModelBuilder modelBuilder)
    {
        var users = Enumerable.Range(0, UsersSeedCount).Select(e => _userFaker.Generate()).ToList();
        var accounts = new List<Account>();

        foreach (var user in users)
        {
            var userAccounts = Enumerable.Range(0, Random.Shared.Next(1, 10)).Select(e => new Account
            {
                Followers = Random.Shared.Next(1, 1000),
                Id = Guid.NewGuid(),
                UserId = user.Id
            }).ToList();

            accounts.AddRange(userAccounts);
        }

        modelBuilder.Entity<Account>().HasData(accounts);
        modelBuilder.Entity<User>().HasData(users);
    }
}