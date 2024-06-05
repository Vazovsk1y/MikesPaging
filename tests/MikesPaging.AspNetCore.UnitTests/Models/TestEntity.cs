namespace MikesPaging.AspNetCore.UnitTests.Models;

internal class TestEntity
{
    public required Guid Id { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required DateTimeOffset Created { get; init; }

    public required uint Age { get; init; }
    
    public required ComplexType ComplexType { get; init; }
    
    public required IEnumerable<ComplexType> RelatedCollection { get; init; }
    
    public uint? IQ { get; set; }
}

internal record ComplexType(string Title, string Value);
