using System.Diagnostics.CodeAnalysis;

namespace MikesPaging.AspNetCore.Exceptions.Base;

public class MikesPagingException : Exception
{
    internal MikesPagingException(string? message) : base(message) { }
    internal MikesPagingException(string? message, Exception? inner) : base(message, inner) { }

    internal static void ThrowIf([DoesNotReturnIf(true)]bool condition, string? message)
    {
        if (condition)
        {
            throw new MikesPagingException(message);
        }
    }

    internal static void ThrowIf([DoesNotReturnIf(true)]bool condition, string? message, Exception? inner)
    {
        if (condition)
        {
            throw new MikesPagingException(message, inner);
        }
    }
}