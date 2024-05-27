using FluentAssertions;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.UnitTests.Enums.Models;

namespace MikesPaging.AspNetCore.UnitTests.Enums;

public class FilteringEnumTests
{
    public static TheoryData<TestFilteringEnum, TestFilteringEnum, bool> FilteringEnums { get; } = new ()
    {
        // not equal
        { new("propertyName", ["propertyName"], 
            inapplicableOperators: [ FilteringOperators.NotEqual ]), 
            new("property", ["propertyName"], 
                inapplicableOperators: [ FilteringOperators.GreaterThanOrEqual ]), false },

        { new("propertyName", ["propertyName"], ignoreCase: true,
            inapplicableOperators: [ FilteringOperators.NotEqual ]),
            new("propertyName", ["propertyName"], ignoreCase: false,
                inapplicableOperators: [ FilteringOperators.NotEqual ]), false },

        { new("propertyName", ["propertyName"]), new("propertyName", ["propertyName", "another"]), false },
        { new("propertyName", ["another", "propertyName"]), new("propertyName", ["Another", "propertyName"]), false },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["Another", "propertyName"]), false },
        { new("propertyName", ["propertyName"], true), new("propertyName", ["propertyName"], false), false },


        // equal
        { new("propertyName", ["propertyName"], 
            inapplicableOperators: [ FilteringOperators.NotEqual ]), 
            new("propertyName", ["propertyName"], 
                inapplicableOperators: [ FilteringOperators.NotEqual ]), true },

        { new("propertyName", ["propertyName"], 
            inapplicableOperators: [ FilteringOperators.NotEqual, FilteringOperators.GreaterThanOrEqual ]), 
          new("propertyName", ["propertyName"], 
            inapplicableOperators: [ FilteringOperators.GreaterThanOrEqual, FilteringOperators.NotEqual ]), true },

        { new("propertyName", ["propertyName"]), new("propertyName", ["propertyName"]), true },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["propertyName", "another"]), true },
        { new("propertyName", ["propertyName", "another"]), new("propertyName", ["another", "propertyName"]), true },
        { new("propertyName", ["propertyName", "another"], ignoreCase: false), new("propertyName", ["another", "propertyName"], ignoreCase: false), true },
    };

    public static TheoryData<TestFilteringEnum, FilteringOperators, bool> DataForIsOperatorApplicable = new()
    {
        { new ("property", ["property"], inapplicableOperators: [ FilteringOperators.NotEqual ]), FilteringOperators.NotEqual, false },
        { new ("property", ["property"], inapplicableOperators: [ FilteringOperators.NotEqual, FilteringOperators.Contains ]), FilteringOperators.Contains, false },

        { new ("property", ["property"], inapplicableOperators: [ FilteringOperators.StartsWith ]), FilteringOperators.NotEqual, true },
        { new ("property", ["property"], inapplicableOperators: [ FilteringOperators.NotEqual, FilteringOperators.Contains ]), FilteringOperators.StartsWith, true },
    };

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_empty_string_passed_to_propertyName()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyStringPassedToPropertyName);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_null_passed_to_propertyName()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullPassedToPropertyName);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_null_passed_to_allowedNames()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullPassedToAllowedNames);
    }
    
    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_empty_collection_passed_to_allowedNames()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyCollectionPassedToAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_allowedNames_contains_duplicates()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.DuplicatesInAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_allowedNames_contains_empty_string()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.EmptyStringContainsInAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_allowedNames_contains_null()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.NullContainsInAllowedNames);
    }

    [Fact]
    public void Ctor_Should_Throw_TypeInitializationException_WHEN_inapplicableOperators_contains_duplicates()
    {
        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.DuplicatesInInapplicableOperators);
    }

    [Fact]
    public void Enumerate_Should_Return_Only_Public_Or_Internal_Static_Readonly_Fields_and_NOT_null_values()
    {
        var expected = new[]
        {
            TestFilteringEnum.ByFirstName, 
            TestFilteringEnum.ByLastNameInternalField, 
            TestFilteringEnum.ByLastName,
            TestFilteringEnum.ByAnyPropertyCaseSensitive
        };

        var result = FilteringEnum.Enumerate<TestFilteringEnum>();

        result.OrderBy(e => e.PropertyName).Should().BeEquivalentTo(expected.OrderBy(e => e.PropertyName));
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_The_Same_Value_regardless_of_which_case_searchTerm_was_passed_when_case_ignore_configured_to_TRUE()
    {
        var expected = TestFilteringEnum.ByLastName;
        string searchTerm = expected.AllowedNames.GetRandom();

        var defaultCaseResult = FilteringEnum.FindFirstOrDefault<TestFilteringEnum>(searchTerm);
        var caseChangedResult = FilteringEnum.FindFirstOrDefault<TestFilteringEnum>(new string(searchTerm
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        defaultCaseResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeEquivalentTo(expected);
        defaultCaseResult.Should().Be(caseChangedResult);
    }

    [Fact]
    public void FindFirstOrDefault_Should_Return_Null_when_searchTerm_case_was_changed_and_case_ignore_configured_to_FALSE()
    {
        var expected = TestFilteringEnum.ByAnyPropertyCaseSensitive;
        string searchTerm = expected.AllowedNames.GetRandom();

        var caseSensitiveResult = FilteringEnum.FindFirstOrDefault<TestFilteringEnum>(searchTerm);
        var caseChangedResult = FilteringEnum.FindFirstOrDefault<TestFilteringEnum>(new string(searchTerm
            .Select(e => char.IsLower(e) ? char.ToUpper(e) : char.ToLower(e))
            .ToArray()));

        caseSensitiveResult.Should().BeEquivalentTo(expected);
        caseChangedResult.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(FilteringEnums))]
    public void Equals_Should_Return_Expected(TestFilteringEnum testFilteringEnum, TestFilteringEnum testFilteringEnum1, bool expected)
    {
        var result = testFilteringEnum.Equals(testFilteringEnum1);

        result.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(DataForIsOperatorApplicable))]
    public void IsOperatorApplicable_Should_Return_Expected(TestFilteringEnum testFilteringEnum, FilteringOperators @operator, bool expected)
    {
        var result = testFilteringEnum.IsOperatorApplicable(@operator);

        result.Should().Be(expected);
    }
}