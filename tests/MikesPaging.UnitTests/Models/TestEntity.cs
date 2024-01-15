namespace MikesPaging.UnitTests.Models;

internal class TestEntity
{
    public required Guid Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required DateTimeOffset Created { get; init; }

    public required uint Age { get; init; }
}
