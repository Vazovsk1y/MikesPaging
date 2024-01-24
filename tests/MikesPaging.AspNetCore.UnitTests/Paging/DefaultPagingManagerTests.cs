using FluentAssertions;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Exceptions;
using MikesPaging.AspNetCore.Services;
using MikesPaging.AspNetCore.UnitTests.Models;

namespace MikesPaging.AspNetCore.UnitTests.Paging;

public class DefaultPagingManagerTests
{
    [Fact]
    public void ApplyPaging_Should_Return_Paged_Collection_when_valid_paging_options_passed()
    {
        // Arrange
        const int pageIndex = 1;
        const int pageSize = 2;

        var pagingOptions = new PagingOptions(pageIndex, pageSize);
        var pagingManager = CreatePagingManager<TestEntity>();

        var data = Data.TestEntities;
        var expected = new Guid[]
        {
            data[0].Id,
            data[1].Id
        };

        // Act
        var result = pagingManager.ApplyPaging(data.AsQueryable(), pagingOptions);

        // Assert 
        var actual = result.Select(e => e.Id).ToArray();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ApplyPaging_Should_Return_The_Same_Collection_when_null_passed_to_paging_options()
    {
        // Arrange
        var pagingManager = CreatePagingManager<TestEntity>();
        var expected = Data.TestEntities;

        // Act
        var actual = pagingManager.ApplyPaging(expected.AsQueryable(), null);

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
    public void ApplyPaging_Should_Throw_PagingException_when_invalid_paging_options_passed(int pageIndex, int pageSize)
    {
        // Arrange
        var invalidPagingOptions = new PagingOptions(pageIndex, pageSize);
        var pagingManager = CreatePagingManager<TestEntity>();
        var data = Data.TestEntities;

        // Act
        IQueryable<TestEntity> Result() => pagingManager.ApplyPaging(data.AsQueryable(), invalidPagingOptions);

        // Assert 
        Assert.Throws<PagingException>(Result);
    }

    private static DefaultPagingManager<T> CreatePagingManager<T>()
    {
        return new DefaultPagingManager<T>();
    }
}
