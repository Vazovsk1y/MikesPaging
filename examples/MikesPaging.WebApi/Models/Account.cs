namespace MikesPaging.WebApi.Models;

public class Account
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public required int Followers { get; init; }
}

