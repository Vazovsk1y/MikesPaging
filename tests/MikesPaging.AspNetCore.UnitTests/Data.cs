using Bogus;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests;

internal static class Data
{
    private static readonly Faker<TestEntity> _testEntityFaker = new Faker<TestEntity>()
        .RuleFor(e => e.Id, e => Guid.NewGuid())
        .RuleFor(e => e.FirstName, e => e.Name.FirstName())
        .RuleFor(e => e.LastName, e => e.Name.LastName())
        .RuleFor(e => e.Age, e => (uint)e.Random.Int(0, 100))
        .RuleFor(e => e.Created, e => DateTimeOffset.UtcNow);

    public static IReadOnlyCollection<TestEntity> GenerateTestEntities(uint count)
    {
        return Enumerable.Range(0, (int)count).Select(e => _testEntityFaker.Generate()).ToList();
    }
}
