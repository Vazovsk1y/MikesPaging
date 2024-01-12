using MikesPaging.AspNetCore.Exceptions.Base;

namespace MikesPaging.AspNetCore.Exceptions;

public class PagingException(string? message) : MikesPagingException(message)
{
    public static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new PagingException(message);
        }
    }
}