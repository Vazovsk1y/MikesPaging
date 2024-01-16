using FluentAssertions;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.AspNetCore.UnitTests.Enums;

public class FilteringEnumTests
{
    public static TheoryData<TestFilteringEnum, TestFilteringEnum, bool> FilteringEnums { get; } = new TheoryData<TestFilteringEnum, TestFilteringEnum, bool>
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

        Assert.Throws<TypeInitializationException>(() => InvalidValuesFilteringEnum.DuplicatesInInapplicableOperators);
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

    [Theory]
    [MemberData(nameof(FilteringEnums))]
    public void Equals_Should_Return_Expected(TestFilteringEnum testFilteringEnum, TestFilteringEnum testFilteringEnum1, bool expected)
    {
        var result = testFilteringEnum.Equals(testFilteringEnum1);

        result.Should().Be(expected);
    }
}