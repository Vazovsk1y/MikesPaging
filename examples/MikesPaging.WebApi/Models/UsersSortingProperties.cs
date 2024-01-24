using MikesPaging.AspNetCore.Common;

namespace MikesPaging.WebApi.Models;

public sealed class UsersSortingProperties : SortingEnum
{
    public static readonly UsersSortingProperties ByFullName = new(nameof(User.FullName), [nameof(User.FullName), "user_fullname"]);

    public static readonly UsersSortingProperties ByAge = new(nameof(User.Age), [nameof(User.Age), "user_age"]);

    public static readonly UsersSortingProperties ByCreatedDate = new(nameof(User.Created), [nameof(User.Created), "created_date"]);

    public static readonly UsersSortingProperties ByAccountsCount = new("AccountsCount", ["AccountsCount", "accounts_count"]);
    private UsersSortingProperties(string propetyName, IReadOnlyCollection<string> allowedValues) : base(propetyName, allowedValues) { }
}