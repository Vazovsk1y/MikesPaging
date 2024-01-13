using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Exceptions;

public class SortingException : MikesPagingException
{
    internal SortingException(string? message) : base(message)
    {
    }

    internal SortingException(string? message, Exception? inner) : base(message, inner)
    {

    }
    internal static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new SortingException(message);
        }
    }

    internal static void ThrowIf(bool condition, string? message, Exception? inner)
    {
        if (condition)
        {
            throw new SortingException(message, inner);
        }
    }
}