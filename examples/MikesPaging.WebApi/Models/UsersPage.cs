using MikesPaging.AspNetCore.Common;
using MikesPaging.WebApi.Infrastructure;

namespace MikesPaging.WebApi.Models;

public record UsersPage : Page<UserDTO, SortingOptions<UsersSortingProperties>, FilteringOptions<UsersFilteringProperties>>
{
    public UsersPage(
        IReadOnlyCollection<UserDTO> users, 
        int totalUsersCount,
        SortingOptions<UsersSortingProperties>? sortingOptions,
        FilteringOptions<UsersFilteringProperties>? filteringOptions,
        PagingOptions? pagingOptions = null) : base(users, totalUsersCount, sortingOptions, filteringOptions, pagingOptions)
    {
    }
}
