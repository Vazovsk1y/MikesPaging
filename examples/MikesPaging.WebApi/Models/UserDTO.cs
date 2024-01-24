namespace MikesPaging.WebApi.Models;

public record UserDTO(Guid Id, string FullName, int Age, DateTimeOffset Created, IReadOnlyCollection<AccountDTO> Accounts);