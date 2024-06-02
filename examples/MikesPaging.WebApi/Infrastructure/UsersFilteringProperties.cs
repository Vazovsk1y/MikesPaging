using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Infrastructure;

public sealed class UsersFilteringProperties : FilteringEnum
{
    public static readonly UsersFilteringProperties ByFullName = new(nameof(User.FullName), [nameof(User.FullName), "user_fullname"],
            inapplicableOperators: [
                FilteringOperators.GreaterThanOrEqual,
                FilteringOperators.GreaterThan,
                FilteringOperators.LessThan,
                FilteringOperators.LessThanOrEqual,
            ]);

    public static readonly UsersFilteringProperties ByAge = new(nameof(User.Age), [nameof(User.Age), "user_age"],
        inapplicableOperators: [
            FilteringOperators.Contains,
            FilteringOperators.StartsWith
            ]);

    public static readonly UsersFilteringProperties ByCreatedDate = new(nameof(User.Created), [nameof(User.Created), "created_date"],
        inapplicableOperators: [
            FilteringOperators.Contains,
            FilteringOperators.StartsWith
            ]);

    public static readonly UsersFilteringProperties ByAccounts = new(nameof(User.Accounts), [nameof(User.Accounts), "user_accounts"],
        inapplicableOperators: [
            FilteringOperators.GreaterThanOrEqual,
            FilteringOperators.GreaterThan,
            FilteringOperators.LessThan,
            FilteringOperators.LessThanOrEqual,
            FilteringOperators.StartsWith,
            ]);

    private UsersFilteringProperties(string propertyName, IReadOnlyCollection<string> allowedNames, IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null)
        : base(propertyName, allowedNames, inapplicableOperators)
    {
    }
}