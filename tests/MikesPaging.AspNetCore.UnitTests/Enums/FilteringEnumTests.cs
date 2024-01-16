using FluentAssertions;
using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.UnitTests.Enums;

public class FilteringEnumTests
{
    [Fact]
    public void TypeInitializationException_Should_Throw_when_invalid_enums_defined()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyStringPassedToPropertyName);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullPassedToPropertyName);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullPassedToAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.DuplicatesInAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyCollectionPassedToAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyStringContainsInAllowedNames);

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullContainsInAllowedNames);
    }

    [Fact]
    public void Enumerate_Should_Return_Only_public_or_internal_static_readonly_fields_NOT_null_values()
    {
        var expected = new[] { TestFilteringEnum.ByFirstName, TestFilteringEnum.ByLastNameInternalField, TestFilteringEnum.ByLastName, TestFilteringEnum.ByAnyPropertyCaseSensitive };

        var result = MikesPagingEnum.Enumerate<TestFilteringEnum>();

        result.OrderBy(e => e.PropertyName).Should().BeEquivalentTo(expected.OrderBy(e => e.PropertyName));
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_expected_value_when_case_ignored()
    {
        var expected = TestFilteringEnum.ByLastName;

        string searchTerm = expected.AllowedNames.GetRandom();
        var defaultCaseResult = MikesPagingEnum.FindFirstOrDefault<TestFilteringEnum>(searchTerm);
        var caseChangedResult = MikesPagingEnum.FindFirstOrDefault<TestFilteringEnum>(new string(searchTerm
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
        var expected = TestFilteringEnum.ByAnyPropertyCaseSensitive;

        string searchTerm = expected.AllowedNames.GetRandom();
        var caseSensitiveResult = MikesPagingEnum.FindFirstOrDefault<TestFilteringEnum>(searchTerm);
        var caseChangedResult = MikesPagingEnum.FindFirstOrDefault<TestFilteringEnum>(new string(searchTerm
            .ToCharArray()
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        caseSensitiveResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeNull();
    }
}