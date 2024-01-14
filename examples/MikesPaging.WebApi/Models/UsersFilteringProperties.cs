using MikesPaging.AspNetCore.Common;

namespace MikesPaging.WebApi.Models;

public sealed class UsersFilteringProperties : MikesPagingEnum
{
    public static readonly UsersFilteringProperties ByFullName = new(nameof(User.FullName), [nameof(User.FullName), "user_fullname" ]);

    public static readonly UsersFilteringProperties ByAge = new(nameof(User.Age), [nameof(User.Age), "user_age"]);

    public static readonly UsersFilteringProperties ByCreatedDate = new(nameof(User.Created), [nameof(User.Created), "created_date"]);

    public static readonly UsersFilteringProperties ByAccounts = new(nameof(User.Accounts), [nameof(User.Accounts), "user_accounts"]);

    private UsersFilteringProperties(string propetyName, IReadOnlyCollection<string> allowedValues) : base(propetyName, allowedValues) { }
}