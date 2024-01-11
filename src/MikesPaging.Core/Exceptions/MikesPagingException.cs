namespace MikesPaging.Core.Exceptions;

public class MikesPagingException(string? message) : Exception(message)
{
    public static void ThrowIf(bool condition, string? message)
    {
        if (condition)
        {
            throw new MikesPagingException(message);
        }
    }
}
