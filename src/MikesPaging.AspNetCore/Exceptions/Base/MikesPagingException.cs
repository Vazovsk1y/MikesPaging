namespace MikesPaging.AspNetCore.Exceptions.Base;

public abstract class MikesPagingException : Exception
{
    protected MikesPagingException(string? message) : base(message) { }
    protected MikesPagingException(string? message, Exception? inner) : base(message, inner) { }
}