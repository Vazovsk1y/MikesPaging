using MikesPaging.Core.Exceptions.Base;

namespace MikesPaging.Core.Exceptions;

public class FilteringException(string? message) : MikesPagingException(message)
{
    public static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new FilteringException(message);
        }
    }
}