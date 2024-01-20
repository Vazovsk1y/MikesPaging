using FluentAssertions;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests.Paging;

public class DefaultPagingManagerTests
{
    private const uint ENTITIES_COUNT = 100;

    [Fact]
    public void ApplyPaging_Should_Return_Paged_Collection_when_valid_pagingOptions_passed()
    {
        // Arrange
        const int pageIndex = 1;
        const int pageSize = 10;

        var pagingOptions = new PagingOptions(pageIndex, pageSize);
        var pagingManager = CreatePagingManager<TestEntity>();
        var data = Data.GenerateTestEntities(ENTITIES_COUNT).AsQueryable();
        var expected = data
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(e => e.Id)
            .Order()
            .ToList();

        // Act
        var result = pagingManager.ApplyPaging(data, pagingOptions);

        // Assert 
        var actual = result
            .Select(e => e.Id)
            .Order()
            .ToList();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplyPaging_Should_Return_The_Same_Collection_when_null_passed_instead_of_paging_options()
    {
        // Arrange
        var pagingManager = CreatePagingManager<TestEntity>();
        var expected = Data
            .GenerateTestEntities(ENTITIES_COUNT)
            .AsQueryable();

        // Act
        var actual = pagingManager.ApplyPaging(expected, null);

        // Assert 
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(-1, 10)]
    [InlineData(10, -1)]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    public void ApplyPaging_Should_Throw_Exception_when_invalid_pagingOptions_passed(int pageIndex, int pageSize)
    {
        // Arrange
        var invalidPagingOptions = new PagingOptions(pageIndex, pageSize);
        var pagingManager = CreatePagingManager<TestEntity>();
        var data = Data
            .GenerateTestEntities(ENTITIES_COUNT)
            .AsQueryable();

        // Act
        IQueryable<TestEntity> Result() => pagingManager.ApplyPaging(data, invalidPagingOptions);

        // Assert 
        Assert.Throws<PagingException>(Result);
    }

    private static DefaultPagingManager<T> CreatePagingManager<T>()
    {
        return new DefaultPagingManager<T>();
    }
}
