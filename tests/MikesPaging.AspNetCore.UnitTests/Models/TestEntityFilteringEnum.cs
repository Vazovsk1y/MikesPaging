using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.UnitTests.Models;

public class TestEntityFilteringEnum : FilteringEnum
{
    public static readonly TestEntityFilteringEnum ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly TestEntityFilteringEnum ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly TestEntityFilteringEnum ByCreated = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static readonly TestEntityFilteringEnum ByAge = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge, 
        inaplicableOperators: [FilteringOperators.StartsWith, FilteringOperators.Contains]);

    public static readonly TestEntityFilteringEnum ByIQ = new(nameof(TestEntity.IQ), AllowedTestEntityNames.AllowedNamesForIQ);

    public static readonly TestEntityFilteringEnum ByAgeButWithInvalidPropertyName = new("invalid", ["invalid"]);

    public static readonly TestEntityFilteringEnum ByComplexTypeTitle = new("Title", [ "Title" ]);

    public static readonly TestEntityFilteringEnum ByComplexTypeValue = new("Value", [ "Value" ]);

    public static readonly TestEntityFilteringEnum ByRelatedCollection = new(nameof(TestEntity.RelatedCollection), [nameof(TestEntity.RelatedCollection) ]);

    private TestEntityFilteringEnum(
        string propertyName, 
        IReadOnlyCollection<string> allowedNames, 
        IReadOnlyCollection<FilteringOperators> inaplicableOperators = null) : base(propertyName, allowedNames, inaplicableOperators)
    {
    }
}