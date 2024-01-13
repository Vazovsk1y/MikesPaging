using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.ViewModels;

namespace MikesPaging.AspNetCore;

public static class Mapper
{
    public static MappingResult<PagingOptions> ToOptions(this PagingOptionsModel pagingOptionsModel)
    {
        if (pagingOptionsModel.PageIndex <= 0)
        {
            return MappingResult<PagingOptions>.Failure(Errors.Paging.PageIndexMustBeGreaterThanZero);
        }

        if (pagingOptionsModel.PageSize <= 0)
        {
            return MappingResult<PagingOptions>.Failure(Errors.Paging.PageSizeMustBeGreaterThanZero);
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

    public static MappingResult<FilteringOptions<TFilterBy>> ToOptions<TFilterBy>(this FilteringOptionsModel filteringOptionsModel)
             where TFilterBy : Enum
    {
        if (string.IsNullOrWhiteSpace(filteringOptionsModel.Logic))
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure("Logic cannot be null or empty.");
        }

        if (filteringOptionsModel.Filters.Count == 0)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure("Filters collection cannot be empty.");
        }

        var logicParsingRes = Enum.TryParse<Logic>(filteringOptionsModel.Logic, true, out var logic);
        if (!logicParsingRes)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure($"Invalid logic value: {filteringOptionsModel.Logic}.");
        }

        if (new HashSet<FilterModel>(filteringOptionsModel.Filters).Count != filteringOptionsModel.Filters.Count)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure($"Filters collection contain duplicates.");
        }

        var filters = new List<Filter<TFilterBy>>();
        foreach (var item in filteringOptionsModel.Filters)
        {
            if (item.Value is null)
            {
                return MappingResult<FilteringOptions<TFilterBy>>.Failure("Value cannot be null.");
            }

            var filterByParsingRes = Enum.TryParse(typeof(TFilterBy), item.FilterBy, true, out var filterBy);
            var operatorParsingRes = Enum.TryParse<FilteringOperators>(item.Operator, true, out var @operator);

            if (!filterByParsingRes)
            {
                return MappingResult<FilteringOptions<TFilterBy>>.Failure($"Invalid FilterBy value: {item.FilterBy}.");
            }

            if (!operatorParsingRes)
            {
                return MappingResult<FilteringOptions<TFilterBy>>.Failure($"Invalid Operator value: {item.Operator}.");
            }

            var filter = new Filter<TFilterBy>((TFilterBy)filterBy!, @operator, item.Value);
            filters.Add(filter);
        }

        return new FilteringOptions<TFilterBy>(filters, logic);
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

    internal static MappingResult<T> Success(T value) => new(true, value, string.Empty);

    internal static MappingResult<T> Failure(string errorMessage) => new(false, default, errorMessage);

    public static implicit  operator MappingResult<T>(T value) => Success(value);
}