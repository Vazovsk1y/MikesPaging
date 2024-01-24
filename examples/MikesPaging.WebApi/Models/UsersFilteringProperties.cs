using MikesPaging.AspNetCore.Common;
using MikesPaging.AspNetCore.Common.Enums;

namespace MikesPaging.WebApi.Models;

public sealed class UsersFilteringProperties : FilteringEnum
{
    public static readonly UsersFilteringProperties ByFullName = 
        new(nameof(User.FullName), [nameof(User.FullName), "user_fullname" ], 
            inapplicableOperators: [ 
                FilteringOperators.GreaterThanOrEqual, 
                FilteringOperators.GreaterThan,
                FilteringOperators.LessThan,
                FilteringOperators.LessThanOrEqual,
            ]);

    public static readonly UsersFilteringProperties ByAge = new(nameof(User.Age), [nameof(User.Age), "user_age"]);

    public static readonly UsersFilteringProperties ByCreatedDate = new(nameof(User.Created), [nameof(User.Created), "created_date"]);

    public static readonly UsersFilteringProperties ByAccounts = new(nameof(User.Accounts), [nameof(User.Accounts), "user_accounts"]);

    private UsersFilteringProperties(string propertyName, IReadOnlyCollection<string> allowedNames, bool ignoreCase = true, IReadOnlyCollection<FilteringOperators>? inapplicableOperators = null) 
        : base(propertyName, allowedNames, ignoreCase, inapplicableOperators)
    {
    }
}