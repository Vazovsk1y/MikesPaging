using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Infrastructure;

public class UsersFilteringConfiguration : FilteringConfiguration<User, UsersFilteringProperties>
{
    public UsersFilteringConfiguration()
    {
        FilterFor(UsersFilteringProperties.ByAccounts, FilteringOperators.Contains, filterValue =>
        {
            if (!Guid.TryParse(filterValue, out var accountId))
            {
                throw new InvalidCastException($"Unable cast {filterValue} to guid.");
            }

            return user => user.Accounts.Any(a => a.Id == accountId);
        });
    }
}