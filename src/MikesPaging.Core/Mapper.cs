using MikesPaging.Core.Common;
using MikesPaging.Core.Common.Enums;
using MikesPaging.Core.Common.ViewModels;

namespace MikesPaging.Core;

public static class Mapper
{
    public static MappingResult<PagingOptions> ToOptions(this PagingOptionsModel pagingOptionsModel)
    {
        if (pagingOptionsModel.PageIndex <= 0)
        {
            return MappingResult<PagingOptions>.Failure("PageIndex must be greater than zero.");
        }

        if (pagingOptionsModel.PageSize <= 0)
        {
            return MappingResult<PagingOptions>.Failure("PageSize must be greater than zero.");
        }

        return new PagingOptions(pagingOptionsModel.PageIndex, pagingOptionsModel.PageSize);
    }

    public static MappingResult<SortingOptions<TSortBy>> ToOptions<TSortBy>(this SortingOptionsModel sortingOptionsModel)
             where TSortBy : Enum
    {
        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortBy))
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure("SortBy cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortDirection))
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure("SortDirection cannot be null or empty.");
        }

        var sortDirectionParsingRes = Enum.TryParse<SortDirections>(sortingOptionsModel.SortDirection, true, out var sortDirection);
        var sortByParsingRes = Enum.TryParse(typeof(TSortBy), sortingOptionsModel.SortBy, true, out var sortBy);

        if (!sortDirectionParsingRes)
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure($"Invalid SortDirection value: {sortingOptionsModel.SortDirection}.");
        }

        if (!sortByParsingRes)
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure($"Invalid SortBy value: {sortingOptionsModel.SortBy}.");
        }

        return new SortingOptions<TSortBy>(sortDirection, (TSortBy)sortBy!);
    }
}

public record MappingResult<T>
{
    private readonly T? _value;
    public T Value 
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Value of failured result can't be accessed.");
            }

            return _value!;
        }
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string ErrorMessage { get; }

    protected MappingResult(bool isSuccess, T? value, string errorMessage)
    {
        if (isSuccess && value is null)
        {
            throw new InvalidOperationException("Unable to pass null when result is successed.");
        }

        if (!isSuccess && string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new InvalidOperationException("Error message is required.");
        }

        _value = value;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static MappingResult<T> Success(T value) => new(true, value, string.Empty);

    public static MappingResult<T> Failure(string errorMessage) => new(false, default, errorMessage);

    public static implicit  operator MappingResult<T>(T value) => Success(value);
}