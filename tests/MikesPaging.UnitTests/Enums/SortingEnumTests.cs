using FluentAssertions;
using MikesPaging.AspNetCore.Common;

namespace MikesPaging.UnitTests.Enums;

public class SortingEnumTests
{
    [Fact]
    public void TypeInitializationException_Should_Throw__when_invalid_enum_defined()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyStringPassedToPropertyName);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullPassedToPropertyName);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullPassedToAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.DuplicatesInAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyCollectionPassedToAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyStringContainsInAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullContainsInAllowedNames);
    }

    [Fact]
    public void Enumerate_Should_Return_Only_public_or_internal_static_readonly_fields_NOT_null_values()
    {
        var expected = new[] { TestSortingEnum.ByFirstName, TestSortingEnum.ByLastNameInternalField, TestSortingEnum.ByLastName, TestSortingEnum.ByAnyPropertyCaseSensitive };

        var result = MikesPagingEnum.Enumerate<TestSortingEnum>();

        result.OrderBy(e => e.PropertyName).Should().BeEquivalentTo(expected.OrderBy(e => e.PropertyName));
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_expected_value_when_case_ignored()
    {
        var expected = TestSortingEnum.ByLastName;

        string searchTerm = expected.AllowedNames.GetRandom();
        var defaultCaseResult = MikesPagingEnum.FindFirstOrDefault<TestSortingEnum>(searchTerm);
        var caseChangedResult = MikesPagingEnum.FindFirstOrDefault<TestSortingEnum>(new string(searchTerm
            .ToCharArray()
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        defaultCaseResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeEquivalentTo(expected);
        defaultCaseResult.Should().Be(caseChangedResult);
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_expected_value_when_case_sensitive()
    {
        var expected = TestSortingEnum.ByAnyPropertyCaseSensitive;

        string searchTerm = expected.AllowedNames.GetRandom();
        var caseSensitiveResult = MikesPagingEnum.FindFirstOrDefault<TestSortingEnum>(searchTerm);
        var caseChangedResult = MikesPagingEnum.FindFirstOrDefault<TestSortingEnum>(new string(searchTerm
            .ToCharArray()
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        caseSensitiveResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeNull();
    }
}