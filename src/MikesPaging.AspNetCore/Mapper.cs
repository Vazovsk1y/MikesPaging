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

        return pagingOptionsModel.PageSize <= 0 ? 
            MappingResult<PagingOptions>.Failure(Errors.Paging.PageSizeMustBeGreaterThanZero) 
            :
            new PagingOptions(pagingOptionsModel.PageIndex, pagingOptionsModel.PageSize);
    }

    public static MappingResult<SortingOptions<TSortBy>> ToOptions<TSortBy>(this SortingOptionsModel sortingOptionsModel)
             where TSortBy : SortingEnum
    {
        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortBy))
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.ValueCannotBeNullOrEmpty(nameof(sortingOptionsModel.SortBy)));
        }

        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortDirection))
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.ValueCannotBeNullOrEmpty(nameof(sortingOptionsModel.SortDirection)));
        }

        var sortDirectionParsingRes = Enum.TryParse<SortingDirections>(sortingOptionsModel.SortDirection, true, out var sortDirection);
        if (!sortDirectionParsingRes)
        {
            return MappingResult<SortingOptions<TSortBy>>.Failure(Errors.InvalidStringValue(nameof(sortingOptionsModel.SortDirection), sortingOptionsModel.SortDirection));
        }

        var sortBy = SortingEnum.FindFirstOrDefault<TSortBy>(sortingOptionsModel.SortBy);
        return sortBy is null ? 
            MappingResult<SortingOptions<TSortBy>>.Failure(Errors.InvalidStringValue(nameof(sortingOptionsModel.SortBy), sortingOptionsModel.SortBy)) 
            :
            new SortingOptions<TSortBy>(sortDirection, sortBy);
    }

    public static MappingResult<FilteringOptions<TFilterBy>> ToOptions<TFilterBy>(this FilteringOptionsModel filteringOptionsModel)
             where TFilterBy : FilteringEnum
    {
        if (string.IsNullOrWhiteSpace(filteringOptionsModel.Logic))
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.ValueCannotBeNullOrEmpty(nameof(filteringOptionsModel.Logic)));
        }

        if (filteringOptionsModel.Filters is null || filteringOptionsModel.Filters.Count == 0)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.ValueCannotBeNullOrEmpty(nameof(filteringOptionsModel.Filters)));
        }

        if (filteringOptionsModel.Filters.Any(e => e is null))
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.ValueCannotBeNull("Filter"));
        }

        var logicParsingRes = Enum.TryParse<Logic>(filteringOptionsModel.Logic, true, out var logic);
        if (!logicParsingRes)
        {
            return MappingResult<FilteringOptions<TFilterBy>>.Failure(Errors.InvalidStringValue(nameof(filteringOptionsModel.Logic), filteringOptionsModel.Logic));
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
        where T : FilteringEnum
    {
        var result = new List<Filter<T>>();
        foreach (var item in filtersModels)
        {
            if (string.IsNullOrWhiteSpace(item.FilterBy))
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.ValueCannotBeNullOrEmpty(nameof(item.FilterBy)));
            }

            var filterBy = FilteringEnum.FindFirstOrDefault<T>(item.FilterBy);
            if (filterBy is null)
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.InvalidStringValue(nameof(item.FilterBy), item.FilterBy));
            }

            var operatorParsingRes = Enum.TryParse<FilteringOperators>(item.Operator, true, out var @operator);
            if (!operatorParsingRes)
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.InvalidStringValue(nameof(item.Operator), item.Operator));
            }

            if (!filterBy.IsOperatorApplicable(@operator))
            {
                return MappingResult<IReadOnlyCollection<Filter<T>>>.Failure(Errors.Filtering.OperatorIsNotApplicableFor(filterBy, @operator));
            }

            var filter = new Filter<T>(filterBy, @operator, item.Value);
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
                throw new InvalidOperationException("Value of failed result can't be accessed.");
            }

            return _value!;
        }
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string ErrorMessage { get; }

    protected MappingResult(bool isSuccess, T? value, string errorMessage)
    {
        switch (isSuccess)
        {
            case true when value is null:
                throw new InvalidOperationException("Unable to pass null when result is succeed.");
            case false when string.IsNullOrWhiteSpace(errorMessage):
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