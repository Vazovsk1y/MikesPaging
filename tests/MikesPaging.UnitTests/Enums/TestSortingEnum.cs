using MikesPaging.AspNetCore.Common;
using MikesPaging.UnitTests.Models;

namespace MikesPaging.UnitTests.Enums;

internal class TestSortingEnum : SortingEnum
{
    // valid

    public static readonly TestSortingEnum ByFirstName = new(nameof(TestEntity.FirstName), AllowedTestEntityNames.AllowedNamesForFirstName);

    public static readonly TestSortingEnum ByLastName = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    internal static readonly TestSortingEnum ByLastNameInternalField = new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);

    public static readonly TestSortingEnum ByAnyPropertyCaseSensitive = new(nameof(ByAnyPropertyCaseSensitive), [nameof(ByAnyPropertyCaseSensitive)], false);

    //

    // invalid

    public static readonly TestSortingEnum ByLastNameNullValue = null;

    internal static readonly TestSortingEnum ByLastNameInternalFieldNullValue = null;

    public static TestSortingEnum ByCreatedStaticField = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    public static TestSortingEnum ByCreatedStaticGetOnlyProperty { get; } = new(nameof(TestEntity.Created), AllowedTestEntityNames.AllowedNamesForCreated);

    protected static readonly TestSortingEnum ByAgeProtectedField = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    private static readonly TestSortingEnum ByAgePrivateField = new(nameof(TestEntity.Age), AllowedTestEntityNames.AllowedNamesForAge);

    public static TestSortingEnum ByLastNameReadOnlyProperty => new(nameof(TestEntity.LastName), AllowedTestEntityNames.AllowedNamesForLastName);
   
    //
    private TestSortingEnum(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true) : base(propertyName, allowedNames, ignoreCase)
    {
    }
}