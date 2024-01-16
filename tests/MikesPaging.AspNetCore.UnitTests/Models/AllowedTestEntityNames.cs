namespace MikesPaging.AspNetCore.UnitTests.Models;

public static class AllowedTestEntityNames
{
    public static IEnumerable<string> All
    {
        get
        {
            return new List<IEnumerable<string>>
            {
                AllowedNamesForFirstName,
                AllowedNamesForLastName,
                AllowedNamesForCreated,
                AllowedNamesForAge
            }
            .SelectMany(e => e)
            .ToList();
        }
    }

    public static readonly IReadOnlyCollection<string> AllowedNamesForFirstName = [nameof(TestEntity.FirstName), "first_name"];
    public static readonly IReadOnlyCollection<string> AllowedNamesForLastName = [nameof(TestEntity.LastName), "last_name"];
    public static readonly IReadOnlyCollection<string> AllowedNamesForCreated = [nameof(TestEntity.Created), "created_date"];
    public static readonly IReadOnlyCollection<string> AllowedNamesForAge = [nameof(TestEntity.Age), "entity_age"];
}
