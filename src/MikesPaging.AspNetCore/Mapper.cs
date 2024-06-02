using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.ViewModels;

namespace MikesPaging.AspNetCore;

public static class Mapper
{
    private const string FilteringBaseCode = "Filtering";
    private const string SortingBaseCode = "Sorting";
    private const string PagingBaseCode = "Paging";

    public static MappingResult<PagingOptions> ToOptions(this PagingOptionsModel pagingOptionsModel)
    {
        if (pagingOptionsModel.PageIndex <= 0)
        {
            return MappingResult.Failure<PagingOptions>(($"{PagingBaseCode}.InvalidPageIndex", Errors.Paging.PageIndexMustBeGreaterThanZero));
        }

        return pagingOptionsModel.PageSize <= 0
            ? MappingResult.Failure<PagingOptions>(($"{PagingBaseCode}.InvalidPageSize", Errors.Paging.PageSizeMustBeGreaterThanZero))
            : new PagingOptions(pagingOptionsModel.PageIndex, pagingOptionsModel.PageSize);
    }

    public static MappingResult<SortingOptions<TSortBy>> ToOptions<TSortBy>(this SortingOptionsModel sortingOptionsModel)
        where TSortBy : SortingEnum
    {
        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortBy))
        {
            return MappingResult.Failure<SortingOptions<TSortBy>>(($"{SortingBaseCode}.NullSortBy", Errors.ValueCannotBeNullOrEmpty(nameof(sortingOptionsModel.SortBy))));
        }

        if (string.IsNullOrWhiteSpace(sortingOptionsModel.SortDirection))
        {
            return MappingResult.Failure<SortingOptions<TSortBy>>(($"{SortingBaseCode}.NullSortDirection", Errors.ValueCannotBeNullOrEmpty(nameof(sortingOptionsModel.SortDirection))));
        }

        var sortDirectionParsingRes = Enum.TryParse<SortingDirections>(sortingOptionsModel.SortDirection, true, out var sortDirection);
        if (!sortDirectionParsingRes)
        {
            return MappingResult.Failure<SortingOptions<TSortBy>>(($"{SortingBaseCode}.InvalidSortDirection", Errors.InvalidStringValue(nameof(sortingOptionsModel.SortDirection), sortingOptionsModel.SortDirection)));
        }

        var sortBy = SortingEnum.FindFirstOrDefault<TSortBy>(sortingOptionsModel.SortBy);
        return sortBy is null
            ? MappingResult.Failure<SortingOptions<TSortBy>>(($"{SortingBaseCode}.InvalidSortBy", Errors.InvalidStringValue(nameof(sortingOptionsModel.SortBy), sortingOptionsModel.SortBy)))
            : new SortingOptions<TSortBy>(sortDirection, sortBy);
    }

    public static MappingResult<FilteringOptions<TFilterBy>> ToOptions<TFilterBy>(this FilteringOptionsModel filteringOptionsModel)
        where TFilterBy : FilteringEnum
    {
        if (string.IsNullOrWhiteSpace(filteringOptionsModel.Logic))
        {
            return MappingResult.Failure<FilteringOptions<TFilterBy>>(($"{FilteringBaseCode}.NullLogic", Errors.ValueCannotBeNullOrEmpty(nameof(filteringOptionsModel.Logic))));
        }

        if (filteringOptionsModel.Filters is null || filteringOptionsModel.Filters.Count == 0)
        {
            return MappingResult.Failure<FilteringOptions<TFilterBy>>(($"{FilteringBaseCode}.NullFilters", Errors.ValueCannotBeNullOrEmpty(nameof(filteringOptionsModel.Filters))));
        }

        if (filteringOptionsModel.Filters.Any(e => e is null))
        {
            return MappingResult.Failure<FilteringOptions<TFilterBy>>(($"{FilteringBaseCode}.NullFilter", Errors.ValueCannotBeNull("Filter")));
        }

        var logicParsingRes = Enum.TryParse<Logic>(filteringOptionsModel.Logic, true, out var logic);
        if (!logicParsingRes)
        {
            return MappingResult.Failure<FilteringOptions<TFilterBy>>(($"{FilteringBaseCode}.InvalidLogic", Errors.InvalidStringValue(nameof(filteringOptionsModel.Logic), filteringOptionsModel.Logic)));
        }

        if (new HashSet<FilterModel>(filteringOptionsModel.Filters).Count != filteringOptionsModel.Filters.Count)
        {
            return MappingResult.Failure<FilteringOptions<TFilterBy>>(($"{FilteringBaseCode}.DuplicateFilters", Errors.Filtering.FiltersCollectionCannotContainDuplicates));
        }

        var filtersRes = ToFilters<TFilterBy>(filteringOptionsModel.Filters);
        return filtersRes.IsFailure
            ? MappingResult.Failure<FilteringOptions<TFilterBy>>(filtersRes.Errors)
            : new FilteringOptions<TFilterBy>(filtersRes.Value, logic);
    }

    private static MappingResult<IReadOnlyCollection<Filter<T>>> ToFilters<T>(IEnumerable<FilterModel> filtersModels)
        where T : FilteringEnum
    {
        var result = new List<Filter<T>>();
        foreach (var item in filtersModels)
        {
            if (string.IsNullOrWhiteSpace(item.FilterBy))
            {
                return MappingResult.Failure<IReadOnlyCollection<Filter<T>>>(
                    ($"{FilteringBaseCode}.NullFilterByValue", Errors.ValueCannotBeNullOrEmpty(nameof(item.FilterBy))));
            }

            var filterBy = FilteringEnum.FindFirstOrDefault<T>(item.FilterBy);
            if (filterBy is null)
            {
                return MappingResult.Failure<IReadOnlyCollection<Filter<T>>>(
                    ($"{FilteringBaseCode}.InvalidFilterBy", Errors.InvalidStringValue(nameof(item.FilterBy), item.FilterBy)));
            }

            var operatorParsingRes = Enum.TryParse<FilteringOperators>(item.Operator, true, out var @operator);
            if (!operatorParsingRes)
            {
                return MappingResult.Failure<IReadOnlyCollection<Filter<T>>>(
                    ($"{FilteringBaseCode}.InvalidOperator", Errors.InvalidStringValue(nameof(item.Operator), item.Operator)));
            }

            if (!filterBy.IsOperatorApplicable(@operator))
            {
                return MappingResult.Failure<IReadOnlyCollection<Filter<T>>>(
                    ($"{FilteringBaseCode}.OperatorNotApplicable", Errors.Filtering.OperatorIsNotApplicableFor(filterBy, @operator)));
            }

            var filter = new Filter<T>(filterBy, @operator, item.Value);
            result.Add(filter);
        }

        return result;
    }
}
