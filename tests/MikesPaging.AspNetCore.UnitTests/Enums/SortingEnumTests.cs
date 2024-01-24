using FluentAssertions;
using MikesPaging.AspNetCore.Common;

namespace MikesPaging.AspNetCore.UnitTests.Enums;

public class SortingEnumTests
{
    public static TheoryData<TestSortingEnum, TestSortingEnum, bool> SortingEnums { get; } = new TheoryData<TestSortingEnum, TestSortingEnum, bool>
    {
        { new("propertyName", ["propertyName"]), new("property", ["propertyName"]), false },
        { new("propertyName", ["propertyName"]), new("propertyName", ["propertyName", "another"]), false },
        { new("propertyName", ["another", "propertyName"]), new("propertyName", ["Another", "propertyName"]), false },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["Another", "propertyName"]), false },
        { new("propertyName", ["propertyName"], true), new("propertyName", ["propertyName"], false), false },

        { new("propertyName", ["propertyName"]), new("propertyName", ["propertyName"]), true },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["propertyName", "another"]), true },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["another", "propertyName"]), true },
        { new("propertyName", ["propertyName", "another"], false), new("propertyName", ["another", "propertyName"], false), true },
    };

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_empty_string_passed_to_property_name()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyStringPassedToPropertyName);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_null_passed_to_property_name()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullPassedToPropertyName);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_null_passed_to_allowed_names()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullPassedToAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_duplicates_in_allowed_names()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.DuplicatesInAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_empty_collection_passed_to_allowed_names()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyCollectionPassedToAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_empty_string_contains_in_allowed_names()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.EmptyStringContainsInAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_when_null_contains_in_allowed_names()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesSortingEnum.NullContainsInAllowedNames);
    }

    [Fact]
    public void Enumerate_Should_Return_Only_Public_Or_Internal_Static_Readonly_Fields_and_NOT_null_values()
    {
        var expected = new[] { TestSortingEnum.ByFirstName, TestSortingEnum.ByLastNameInternalField, TestSortingEnum.ByLastName, TestSortingEnum.ByAnyPropertyCaseSensitive };

        var result = SortingEnum.Enumerate<TestSortingEnum>();

        result.OrderBy(e => e.PropertyName).Should().BeEquivalentTo(expected.OrderBy(e => e.PropertyName));
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_The_Same_Value_regardless_of_which_case_searchTerm_was_passed_when_case_ignore_configured_to_true()
    {
        var expected = TestSortingEnum.ByLastName;
        string searchTerm = expected.AllowedNames.GetRandom();

        var defaultCaseResult = SortingEnum.FindFirstOrDefault<TestSortingEnum>(searchTerm);
        var caseChangedResult = SortingEnum.FindFirstOrDefault<TestSortingEnum>(new string(searchTerm
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        defaultCaseResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeEquivalentTo(expected);
        defaultCaseResult.Should().Be(caseChangedResult);
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_Null_when_searchTerm_case_was_changed_and_case_ignore_configured_to_false()
    {
        var expected = TestSortingEnum.ByAnyPropertyCaseSensitive;
        string searchTerm = expected.AllowedNames.GetRandom();

        var caseSensitiveResult = SortingEnum.FindFirstOrDefault<TestSortingEnum>(searchTerm);
        var caseChangedResult = SortingEnum.FindFirstOrDefault<TestSortingEnum>(new string(searchTerm
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        caseSensitiveResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(SortingEnums))]
    public void Equals_Should_Return_Expected(TestSortingEnum testSortingEnum, TestSortingEnum testSortingEnum1, bool expected)
    {
        var result = testSortingEnum.Equals(testSortingEnum1);

        result.Should().Be(expected);
    }
}