using MikesPaging.AspNetCore.Common;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Infrastructure;

public class UsersSortingConfiguration : SortingConfiguration<User, UsersSortingProperties>
{
    public UsersSortingConfiguration()
    {
        SortFor(UsersSortingProperties.ByAccountsCount, e => e.Accounts.Count);
    }
}