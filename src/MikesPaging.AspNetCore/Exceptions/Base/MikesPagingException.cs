namespace MikesPaging.AspNetCore.Exceptions.Base;

public abstract class MikesPagingException(string? message) : Exception(message)
{
}