﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.Interfaces;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services;
using MikesPaging.AspNetCore.UnitTests.Models;
using NSubstitute;

namespace MikesPaging.AspNetCore.UnitTests.Filtering;

public class DefaultFilteringManagerTests
{
    private static readonly FilteringOperators[] ComparableOperators = [
        FilteringOperators.GreaterThan,
        FilteringOperators.LessThan,
        FilteringOperators.GreaterThanOrEqual,
        FilteringOperators.LessThanOrEqual,
    ];

    private static readonly FilteringOperators[] StringOperators = [
        FilteringOperators.Contains,
        FilteringOperators.StartsWith,
    ];

    public static TheoryData<FilteringOptions<TestEntityFilteringEnum>> InvalidFilteringOptionsData { get; } = new TheoryData<FilteringOptions<TestEntityFilteringEnum>>()
    {
        // invalid
        new(null, Extensions.PickRandom(Logic.And, Logic.Or)),
        new([ ], Extensions.PickRandom(Logic.And, Logic.Or)),
        new([null], Extensions.PickRandom(Logic.And, Logic.Or)),


        new([new (TestEntityFilteringEnum.ByAge, FilteringOperators.NotEqual, "1"), 
             new(TestEntityFilteringEnum.ByAge, FilteringOperators.NotEqual, "1")],        // duplicates in filters
            Extensions.PickRandom(Logic.And, Logic.Or)),                                    

        new([new(null, FilteringOperators.NotEqual, "1")], Extensions.PickRandom(Logic.And, Logic.Or)),

        // inapplicable filters
        new([new(TestEntityFilteringEnum.ByAge, FilteringOperators.NotEqual, "notIntNumber")], Extensions.PickRandom(Logic.And, Logic.Or)),
        new([new(TestEntityFilteringEnum.ByAgeButWithInvalidPropertyName, FilteringOperators.NotEqual, "anyValue")], Extensions.PickRandom(Logic.And, Logic.Or)),

        new([new(TestEntityFilteringEnum.ByAge, StringOperators.GetRandom(), "1")], Extensions.PickRandom(Logic.And, Logic.Or)),
        new([new(TestEntityFilteringEnum.ByFirstName, ComparableOperators.GetRandom(), "anySearchTerm")], Extensions.PickRandom(Logic.And, Logic.Or)),

        new([new(TestEntityFilteringEnum.ByAge, TestEntityFilteringEnum.ByAge.InapplicableOperators.GetRandom(), "1")], Extensions.PickRandom(Logic.And, Logic.Or)),
    };

    [Fact]
    public void ApplyFiltering_Should_Return_The_Same_Collection_WHEN_null_passed_to_filteringOptions()
    {
        // arrange
        var manager = CreateFilteringManager<TestEntity>();
        var expected = Data.TestEntities;
        FilteringOptions<TestEntityFilteringEnum>? nullFilteringOptions = null;

        // act
        var actual = manager.ApplyFiltering(expected.AsQueryable(), nullFilteringOptions);

        // assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_single_filter_passed()
    {
        // arrange
        var manager = CreateFilteringManager<TestEntity>();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum> []{ new(TestEntityFilteringEnum.ByAge, FilteringOperators.LessThanOrEqual, "2") };
        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Extensions.PickRandom(Logic.And, Logic.Or));
        var expected = new Guid[]
        {
            data[2].Id,
            data[0].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_more_than_one_filter_passed_with_OR_logic()
    {
        // arrange
        var manager = CreateFilteringManager<TestEntity>();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByAge, FilteringOperators.LessThanOrEqual, "1"),
            new(TestEntityFilteringEnum.ByIQ, FilteringOperators.NotEqual, null)
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.Or);
        var expected = new Guid[]
        {
            data[0].Id,
            data[2].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_more_than_one_filter_passed_with_AND_logic()
    {
        // arrange
        var manager = CreateFilteringManager<TestEntity>();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByAge, FilteringOperators.LessThanOrEqual, "3"),
            new(TestEntityFilteringEnum.ByIQ, FilteringOperators.Equal, null)
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.And);
        var expected = new Guid[]
        {
            data[1].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_single_userDefined_filter_passed_with_AND_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByComplexTypeTitle, FilteringOperators.Contains, "1"),
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.And);
        var expected = new Guid[]
        {
            data[0].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_single_userDefined_filter_passed_with_OR_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByComplexTypeTitle, FilteringOperators.Contains, "1"),
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.Or);
        var expected = new Guid[]
        {
            data[0].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_more_than_one_userDefined_filter_passed_with_AND_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByComplexTypeValue, FilteringOperators.StartsWith, "Value2"),
            new(TestEntityFilteringEnum.ByRelatedCollection, FilteringOperators.Contains, "Jack")
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.And);
        var expected = new Guid[]
        {
            data[1].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_more_than_one_userDefined_filter_passed_with_OR_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByComplexTypeValue, FilteringOperators.StartsWith, "Value2"),
            new(TestEntityFilteringEnum.ByRelatedCollection, FilteringOperators.Contains, "Jack")
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.Or);
        var expected = new Guid[]
        {
            data[0].Id,
            data[1].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_userDefined_and_built_filter_applied_with_AND_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByComplexTypeValue, FilteringOperators.StartsWith, "Value3"),
            new(TestEntityFilteringEnum.ByLastName, FilteringOperators.StartsWith, "For")
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.And);
        var expected = new Guid[]
        {
            data[2].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ApplyFiltering_Should_Return_Filtered_Collection_WHEN_userDefined_and_built_filter_applied_with_OR_logic()
    {
        // arrange
        var manager = CreateFilteringManagerWithFilteringConfigurationUsing();
        var data = Data.TestEntities;

        var filters = new Filter<TestEntityFilteringEnum>[]
        {
            new(TestEntityFilteringEnum.ByRelatedCollection, FilteringOperators.Contains, "Empty"),
            new(TestEntityFilteringEnum.ByFirstName, FilteringOperators.Contains, "Jo")
        };

        var filteringOptions = new FilteringOptions<TestEntityFilteringEnum>(filters, Logic.Or);
        var expected = new Guid[]
        {
            data[0].Id,
            data[2].Id,
        };

        // act 
        var result = manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidFilteringOptionsData))]
    public void ApplyFiltering_Should_Throw_FilteringException_WHEN_invalid_filtering_options_passed(FilteringOptions<TestEntityFilteringEnum> filteringOptions)
    {
        // arrange
        var manager = CreateFilteringManager<TestEntity>();
        var data = Data.TestEntities;

        // act 
        IQueryable<TestEntity> Result() => manager.ApplyFiltering(data.AsQueryable(), filteringOptions);

        // assert
        Assert.Throws<FilteringException>(Result);
    }

    private static DefaultFilteringManager<T> CreateFilteringManager<T>()
    {
        var scopeMock = Substitute.For<IServiceScope>();
        var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();

        scopeMock.ServiceProvider.GetService(Arg.Any<Type>()).Returns(null);
        scopeFactoryMock.CreateScope().Returns(scopeMock);

        return new DefaultFilteringManager<T>(scopeFactoryMock);
    }
    
    private static DefaultFilteringManager<TestEntity> CreateFilteringManagerWithFilteringConfigurationUsing()
    {
        var scopeMock = Substitute.For<IServiceScope>();
        var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();

        scopeFactoryMock.CreateScope().Returns(scopeMock);
        scopeMock.ServiceProvider.GetService(typeof(IFilteringConfiguration<TestEntity, TestEntityFilteringEnum>)).Returns(new TestEntityFilteringConfiguration());

        return new DefaultFilteringManager<TestEntity>(scopeFactoryMock);
    }
}
