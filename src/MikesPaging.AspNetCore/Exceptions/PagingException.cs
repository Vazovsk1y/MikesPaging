using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Exceptions;

public class PagingException : MikesPagingException
{
    internal PagingException(string? message) : base(message)
    {
    }

    internal PagingException(string? message, Exception? inner) : base(message, inner)
    {

    }
    internal static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new PagingException(message);
        }
    }

    internal static void ThrowIf(bool condition, string? message, Exception? inner)
    {
        if (condition)
        {
            throw new PagingException(message, inner);
        }
    }
}