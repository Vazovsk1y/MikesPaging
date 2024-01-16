using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests.Enums;

public class TestFilteringEnum : FilteringEnum
{
    // valid

    public static readonly TestFilteringEnum ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly TestFilteringEnum ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    internal static readonly TestFilteringEnum ByLastNameInternalField = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly TestFilteringEnum ByAnyPropertyCaseSensitive = new(nameof(ByAnyPropertyCaseSensitive), [nameof(ByAnyPropertyCaseSensitive)], false);

    //

    // invalid

    public static readonly TestFilteringEnum ByLastNameNullValue = null;

    internal static readonly TestFilteringEnum ByLastNameInternalFieldNullValue = null;

    public static TestFilteringEnum ByCreatedStaticField = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static TestFilteringEnum ByCreatedStaticGetOnlyProperty { get; } = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    protected static readonly TestFilteringEnum ByAgeProtectedField = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    private static readonly TestFilteringEnum ByAgePrivateField = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    public static TestFilteringEnum ByLastNameReadOnlyProperty => new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    //
    internal TestFilteringEnum(
        string propertyName, 
        IReadOnlyCollection<string> allowedNames, 
        bool ignoreCase = true, 
        IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null) : base(propertyName, allowedNames, ignoreCase, inapplicableOperators)
    {
    }
}