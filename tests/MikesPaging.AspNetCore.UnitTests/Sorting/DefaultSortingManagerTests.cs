using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services;
using MikesPaging.AspNetCore.UnitTests.Models;
using NSubstitute;

namespace MikesPaging.AspNetCore.UnitTests.Sorting;

public class DefaultSortingManagerTests
{
    public static TheoryData<SortingOptions<TestEntitySortingEnum>> InvalidSortingOptions { get; } =
    [
        new (SortingDirections.Descending, null),
        new (SortingDirections.Ascending, TestEntitySortingEnum.ByAgeButWithInvalidPropertyName),
        new (SortingDirections.Descending, TestEntitySortingEnum.ByAgeButWithInvalidPropertyName),
    ];
    
    [Fact]
    public void ApplySorting_Should_Return_The_Same_Collection_WHEN_null_passed_to_sorting_options()
    {
        // arrange
        var manager = CreateSortingManager<TestEntity>();
        var expected = Data.TestEntities;
        SortingOptions<TestEntitySortingEnum>? nullSortingOptions = null;

        // act
        var actual = manager.ApplySorting(expected.AsQueryable(), nullSortingOptions);

        // assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplySorting_Should_Return_Sorted_In_Ascending_Order_Collection_WHEN_sorting_options_sort_direction_property_set_to_ASCENDING()
    {
        // arrange
        var manager = CreateSortingManager<TestEntity>();
        var data = Data.TestEntities;
        var expected = new Guid[] 
        { 
            data[2].Id, 
            data[0].Id, 
            data[1].Id 
        };

        var sortingOptions = new SortingOptions<TestEntitySortingEnum>(SortingDirections.Ascending, TestEntitySortingEnum.ByAge);

        // act 
        var result = manager.ApplySorting(data.AsQueryable(), sortingOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplySorting_Should_Return_Sorted_In_Descending_Order_Collection_WHEN_sorting_options_sort_direction_property_set_to_DESCENDING()
    {
        // arrange
        var manager = CreateSortingManager<TestEntity>();
        var data = Data.TestEntities;
        var expected = new Guid[]
        {
            data[1].Id,
            data[0].Id,
            data[2].Id
        };

        var sortingOptions = new SortingOptions<TestEntitySortingEnum>(SortingDirections.Descending, TestEntitySortingEnum.ByAge);

        // act 
        var result = manager.ApplySorting(data.AsQueryable(), sortingOptions);

        // assert
        var actual = result.Select(e => e.Id).ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidSortingOptions))]
    public void ApplySorting_Should_Throw_SortingException_WHEN_invalid_sorting_options_passed(SortingOptions<TestEntitySortingEnum> sortingOptions)
    {
        // arrange
        var manager = CreateSortingManager<TestEntity>();
        var data = Data.TestEntities;

        // act
        IQueryable<TestEntity> Result() => manager.ApplySorting(data.AsQueryable(), sortingOptions);

        // assert
        Assert.Throws<SortingException>(Result);
    }

    private static DefaultSortingManager<T> CreateSortingManager<T>()
    {
        var scopeMock = Substitute.For<IServiceScope>();
        var scopeFactoryMock = Substitute.For<IServiceScopeFactory>();

        scopeMock.ServiceProvider.GetService(Arg.Any<Type>()).Returns(null);
        scopeFactoryMock.CreateScope().Returns(scopeMock);

        return new DefaultSortingManager<T>(scopeFactoryMock);
    }
}
