using FluentAssertions;
using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.AspNetCore.Common.ViewModels;

namespace MikesPaging.AspNetCore.UnitTests.Mapper;

public class MapperTests
{
    private const string _ascending = "ascending";
    private const string _descending = "descending";

    private const string _or = "or";
    private const string _and = "and";

    private readonly static string[] _operators = Enum.GetNames<FilteringOperators>().Select(e => e.ToString()).ToArray();

    private static readonly string[] AllowedNamesForSorting = SortingEnum.Enumerate<SortingEnumForMapperTests>().SelectMany(e => e.AllowedNames).ToArray();

    private static readonly string[] AllowedNamesForFiltering = FilteringEnum.Enumerate<FilteringEnumForMapperTests>().SelectMany(e => e.AllowedNames).ToArray();

    private readonly static string?[] _values = 
    [
        DateTimeOffset.UtcNow.ToString(),
        3.ToString(),
        3.0.ToString(),
        3.0m.ToString(),
        "randomString",
        (-5).ToString(),
        DateTime.UtcNow.ToString(),
        true.ToString(),
        null,
        string.Empty,
        "              "
    ];

    public static TheoryData<PagingOptionsModel> ValidPagingOptionsModels { get; } = new()
    {
       { new (1, 1) },
       { new (100, 1) },
       { new (1, 100) },
    };

    public static TheoryData<PagingOptionsModel> InvalidPagingOptionsModels { get; } = new()
    {
       { new (-1, -1) },
       { new (-1, 1) },
       { new (1, -1) },
       { new (0, 1) },
       { new (1, 0) },
       { new (0, 0) },
    };

    public static TheoryData<SortingOptionsModel> InvalidSortingOptionsModels { get; } =
    [
        // (null or empty string passed)
        new(string.Empty, "age"),
        new(Extensions.PickRandom(_and, _or), string.Empty),
        new("       ", "age"),
        new(Extensions.PickRandom(_and, _or), "        "),
        new(null, null),
        new(string.Empty, string.Empty),
        new(Extensions.PickRandom(_and, _or), null),
        new(null, "age"),

        // (passed wrong sortDirection or filterBy)
        new("asc", AllowedNamesForSorting.GetRandom()),
        new("desc", AllowedNamesForSorting.GetRandom()),
        new("decending", AllowedNamesForSorting.GetRandom()),
        new("acending", AllowedNamesForSorting.GetRandom()),
        new(Extensions.PickRandom(_and, _or), "agy"),
        new(Extensions.PickRandom(_and, _or), "ceated"),
        new(Extensions.PickRandom(_and, _or), "fulName"),
        new(Extensions.PickRandom(_and, _or), "frstName"),
        new(Extensions.PickRandom(_and, _or), "lst_name"),
        new(Extensions.PickRandom(_and, _or), "created_ate")
    ];

    public static TheoryData<SortingOptionsModel> ValidSortingOptionsModels
    {
        get
        {
            var result = new TheoryData<SortingOptionsModel>();
            foreach (var allowedName in AllowedNamesForSorting)
            {
                result.Add(new SortingOptionsModel(Extensions.PickRandom(_ascending, _descending), allowedName));
            }

            return result;
        }
    }

    public static TheoryData<FilteringOptionsModel> InvalidFilteringOptionsModels { get; } =
    [
        // invalid (bad data passed)
        new([], Extensions.PickRandom(_and, _or)),
        new(null, Extensions.PickRandom(_and, _or)),
        new([null], Extensions.PickRandom(_or, _and)),

        new([ new (string.Empty, _values.GetRandom(), _operators.GetRandom()) ], Extensions.PickRandom(_and, _or)),
        new([new(null, _values.GetRandom(), _operators.GetRandom())], Extensions.PickRandom(_and, _or)),
        new([new("            ", _values.GetRandom(), _operators.GetRandom())], Extensions.PickRandom(_and, _or)),
        new([new("invalidFilterByString", _values.GetRandom(), _operators.GetRandom())], Extensions.PickRandom(_and, _or)),

        new([new(AllowedNamesForFiltering.GetRandom(), _values.GetRandom(), string.Empty)], Extensions.PickRandom(_or, _and)),
        new([new(AllowedNamesForFiltering.GetRandom(), _values.GetRandom(), null)], Extensions.PickRandom(_or, _and)),
        new([new(AllowedNamesForFiltering.GetRandom(), _values.GetRandom(), "          ")], Extensions.PickRandom(_or, _and)),


        new([new(FilteringEnumForMapperTests.ByAge.PropertyName, _values.GetRandom(), "notEqual")], null),
        new([new(FilteringEnumForMapperTests.ByAge.PropertyName, _values.GetRandom(), "notEqual")], string.Empty),
        new([new(FilteringEnumForMapperTests.ByAge.PropertyName, _values.GetRandom(), "notEqual")], "             "),

        // invalid (contains duplicates)
        new([new ("first_name", "value", "notEqual"), new("first_name", "value", "notEqual")], Extensions.PickRandom(_and, _or)),

        // inapplicable operator passed
        new([new (
            FilteringEnumForMapperTests.ByAnyPropertyWithInapplicableOperators.AllowedNames.GetRandom(), 
            _values.GetRandom(), 
            FilteringEnumForMapperTests.ByAnyPropertyWithInapplicableOperators.InapplicableOperators.GetRandom().ToString())], Extensions.PickRandom(_or, _and))
    ];

    public static TheoryData<FilteringOptionsModel> ValidFilteringOptionsModels
    {
        get
        {
            var result = new TheoryData<FilteringOptionsModel>();
            for (int i = 0; i < 100; i++)
            {
                var filters = new List<FilterModel>();
                for (int j = 0; j < Random.Shared.Next(1, 5); j++)
                {
                    var filterBy = AllowedNamesForFiltering.GetRandom();
                    var @operator = _operators
                        .Except(FilteringEnum.FindFirstOrDefault<FilteringEnumForMapperTests>(filterBy)!
                              .InapplicableOperators
                              .Select(e => e.ToString()), StringComparer.OrdinalIgnoreCase)
                        .GetRandom();

                    var filter = new FilterModel(filterBy, _values.GetRandom(), @operator);
                    if (!filters.Contains(filter))
                    {
                        filters.Add(filter);
                    }
                }

                var model = new FilteringOptionsModel(
                    filters,
                    Extensions.PickRandom(_or, _and));

                result.Add(model);
            }

            return result;
        }
    }

    [Theory]
    [MemberData(nameof(ValidPagingOptionsModels))]
    public void ToPagingOptions_Should_Return_Success_Result_when_valid_data_passed(PagingOptionsModel pagingOptionsModel)
    {
        var expected = true;
        var expectedValue = new PagingOptions(pagingOptionsModel.PageIndex, pagingOptionsModel.PageSize);

        var result = pagingOptionsModel.ToOptions();

        result.IsSuccess.Should().Be(expected);
        result.Value.Should().BeEquivalentTo(expectedValue);
    }

    [Theory]
    [MemberData(nameof(InvalidPagingOptionsModels))]
    public void ToPagingOptions_Should_Return_Failure_Result_when_invalid_data_passed(PagingOptionsModel pagingOptionsModel)
    {
        var expected = false;

        var result = pagingOptionsModel.ToOptions();

        result.IsSuccess.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(InvalidSortingOptionsModels))]
    public void ToSortingOptions_Should_Return_Failure_Result_when_invalid_data_passed(SortingOptionsModel sortingOptionsModel)
    {
        bool expected = false;
        var result = sortingOptionsModel.ToOptions<SortingEnumForMapperTests>();

        result.IsSuccess.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(ValidSortingOptionsModels))]
    public void ToSortingOptions_Should_Return_Success_Result_when_valid_data_passed(SortingOptionsModel sortingOptionsModel)
    {
        bool expected = true;
        var expectedValue = new SortingOptions<SortingEnumForMapperTests>(
            Enum.Parse<SortingDirections>(sortingOptionsModel.SortDirection, true),
            SortingEnum.FindFirstOrDefault<SortingEnumForMapperTests>(sortingOptionsModel.SortBy) ?? throw new InvalidOperationException("Sort by property not found."));

        var result = sortingOptionsModel.ToOptions<SortingEnumForMapperTests>();

        result.IsSuccess.Should().Be(expected);
        result.Value.Should().BeEquivalentTo(expectedValue);
    }

    [Theory]
    [MemberData(nameof(InvalidFilteringOptionsModels))]
    public void ToFilteringOptions_Should_Return_Failure_Result_when_invalid_data_passed(FilteringOptionsModel filteringOptionsModel)
    {
        var expected = false;

        var result = filteringOptionsModel.ToOptions<FilteringEnumForMapperTests>();

        result.IsSuccess.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(ValidFilteringOptionsModels))]
    public void ToFilteringOptions_Should_Return_Success_Result_when_valid_data_passed(FilteringOptionsModel model)
    {
        var expected = true;
        var expectedValue = new FilteringOptions<FilteringEnumForMapperTests>(
            model.Filters.Select(e =>
                       new Filter<FilteringEnumForMapperTests>(
                           FilteringEnum.FindFirstOrDefault<FilteringEnumForMapperTests>(e.FilterBy) ?? throw new InvalidOperationException("Filter by property not found."),
                           Enum.Parse<FilteringOperators>(e.Operator, true),
                           e.Value)).ToList(),
            Enum.Parse<Logic>(model.Logic, true));

        var result = model.ToOptions<FilteringEnumForMapperTests>();

        result.IsSuccess.Should().Be(expected);
        result.Value.Should().BeEquivalentTo(expectedValue);
    }
}