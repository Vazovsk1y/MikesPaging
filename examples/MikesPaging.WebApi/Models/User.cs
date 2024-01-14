namespace MikesPaging.WebApi.Models;

public class User
{
    public required Guid Id { get; init; }

    public required string FullName { get; init; }

    public required int Age { get; init; }

    public required DateTimeOffset Created { get; init; }

    public ICollection<Account> Accounts { get; init; } = new HashSet<Account>();
}