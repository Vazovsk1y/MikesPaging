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
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.ValueCannotBeNullOrEmpty("Sort by property"));
        }

        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortDirection))
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.ValueCannotBeNullOrEmpty("Sort direction"));
        }

        var sortDirectionParsingRes = Enum.TryParse<SortDirections>(sortingOptionsModel.SortDirection, true, out var sortDirection);
        if (!sortDirectionParsingRes)
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.InvalidStringValue("sort direction", sortingOptionsModel.SortDirection));
        }

        var sortByParsingRes = Enum.TryParse(typeof(TSortBy), sortingOptionsModel.SortBy, true, out var sortBy);
        if (!sortByParsingRes)
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.InvalidStringValue("sort by", sortingOptionsModel.SortBy));
        }

        return new SortingOptions<TSortBy>(sortDirection, (TSortBy)sortBy!);
    }

    public static MappingResult<FilteringOptions<TFilterBy>> ToOptions<TFilterBy>(this FilteringOptionsModel filteringOptionsModel)
             where TFilterBy : Enum
    {
        if (string.IsNullOrWhiteSpace(filteringOptionsModel.Logic))
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.ValueCannotBeNullOrEmpty("Logic property"));
        }

        if (filteringOptionsModel.Filters is null || filteringOptionsModel.Filters.Count == 0)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.ValueCannotBeNullOrEmpty("Filters collection"));
        }

        var logicParsingRes = Enum.TryParse<Logic>(filteringOptionsModel.Logic, true, out var logic);
        if (!logicParsingRes)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.InvalidStringValue("logic", filteringOptionsModel.Logic));
        }

        if (new HashSet<FilterModel>(filteringOptionsModel.Filters).Count != filteringOptionsModel.Filters.Count)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.Filtering.FiltersCollectionCannotContainDuplicates);
        }

        var filtersRes = ToFilters<TFilterBy>(filteringOptionsModel.Filters);
        return filtersRes.IsFailure ?
            MappingResult<FilteringOptions<TFilterBy>>.Failure(filtersRes.ErrorMessage)
            :
            new FilteringOptions<TFilterBy>(filtersRes.Value, logic);
    }

    private static MappingResult<IReadOnlyCollection<Filter<T>>> ToFilters<T>(IEnumerable<FilterModel> filtersModels)
        where T : Enum
    {
        var result = new List<Filter<T>>();
        foreach (var item in filtersModels)
        {
            if (item.Value is null)
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.ValueCannotBeNull("Filter value"));
            }

            var filterByParsingRes = Enum.TryParse(typeof(T), item.FilterBy, true, out var filterBy);
            if (!filterByParsingRes)
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.InvalidStringValue("filter by", item.FilterBy));
            }

            var operatorParsingRes = Enum.TryParse<FilteringOperators>(item.Operator, true, out var @operator);
            if (!operatorParsingRes)
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.InvalidStringValue("filtering operator", item.Operator));
            }

            var filter = new Filter<T>((T)filterBy!, @operator, item.Value);
            result.Add(filter);
        }

        return result;
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