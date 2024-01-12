using MikesPaging.Core.Exceptions.Base;

namespace MikesPaging.Core.Exceptions;

public class SortingException(string? message) : MikesPagingException(message)
{
    public static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new SortingException(message);
        }
    }
}