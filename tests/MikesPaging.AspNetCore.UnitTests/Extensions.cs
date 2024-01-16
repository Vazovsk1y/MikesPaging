namespace MikesPaging.AspNetCore.UnitTests;

internal static class Extensions
{
    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        return collection.ElementAt(Random.Shared.Next(0, collection.Count()));
    }

    public static T PickRandom<T>(params T[] values)
    {
        return values.GetRandom();
    }
}
