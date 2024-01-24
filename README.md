# MikesPaging.AspNetCore
MikesPaging.AspNetCore is a simple library for implementing paging, sorting and filtering functionality to your ASP.NET applications.

## Install

Minimum Requirements: **.NET 8.0.x**

[Download from Nuget](https://www.nuget.org/packages/MikesPaging.AspNetCore/)

##### powershell

```powershell
Install-Package MikesPaging.AspNetCore
```

##### cmd
```cmd
dotnet add package MikesPaging.AspNetCore
```

## Usage

In this example, consider an app with a `User` entity. 
We'll use MakesPaging.AspNetCore to add sorting, filtering, and pagination capabilities when receiving all available users.

### 1. Paging

##### DI
```C#
builder.Services.AddPaging();
```
##### Steps to apply paging
1. Inject IPagingManager<'T'> to use paging.
2. Define post api endpoint with 'PagingOptionsModel' argument.
3. Map to 'PagingOptions' by calling extension method.
4. Apply paging by calling 'ApplyPaging' method.

```C# 
[HttpPost("page")]
public IActionResult GetAsync(PagingOptionsModel pagingOptionsModel)
{
    var pagingOptionsRes = pagingOptionsModel.ToOptions();
    if (pagingOptionsRes.IsFailure)
    {
        return BadRequest(pagingOptionsRes.ErrorMessage);
    }

    var pagingOptions = pagingOptionsRes.Value;
    int totalUsersCount = dbContext.Users.Count();

    var result = dbContext
        .Users
        .Include(e => e.Accounts)
        .OrderBy(e => e.FullName)
        .AsNoTracking();

    result = pagingManager.ApplyPaging(result, pagingOptions);

    var users = result.Select(e => new UserDTO(
        e.Id,
        e.FullName,
        e.Age,
        e.Created,
        e.Accounts.Select(i => new AccountDTO(i.Id, i.Followers)).ToList()
        ))
        .ToList();

    return Ok(new UsersPage(users, totalUsersCount, null, null, pagingOptions));
}
```

##### Notes:
- you can return your paged collection directly or you can define page class that will be inherit one of possible pages and use it like return type.
```C#
public abstract record Page<TItem> : IPage<TItem>

public abstract record Page<TItem, TSorting, TFiltering> : 
    Page<TItem>
    where TSorting : class, ISortingOptions
    where TFiltering : class, IFilteringOptions

public abstract record FilteredPage<TItem, TFiltering> : Page<TItem>
    where TFiltering : class, IFilteringOptions

public abstract record SortedPage<TItem, TSorting> : Page<TItem>
    where TSorting : class, ISortingOptions
```

### 2. Sorting

##### DI
```C#
builder.Services.AddSorting();
```

##### Steps to apply sorting
1. Define sorting enum for entity.
```C# 
public sealed class UsersSortingProperties : SortingEnum
{
    public static readonly UsersSortingProperties ByFullName = new(nameof(User.FullName), [nameof(User.FullName), "user_fullname"]);

    public static readonly UsersSortingProperties ByAge = new(nameof(User.Age), [nameof(User.Age), "user_age"]);

    public static readonly UsersSortingProperties ByCreatedDate = new(nameof(User.Created), [nameof(User.Created), "created_date"]);

    public static readonly UsersSortingProperties ByAccountsCount = new("AccountsCount", ["AccountsCount", "accounts_count"]);
    private UsersSortingProperties(string propetyName, IReadOnlyCollection<string> allowedValues) : base(propetyName, allowedValues) { }
}
```
2. Inject ISortingManager<'T'> to use sorting.
3. Define post api endpoint with 'SortingOptionsModel' argument.
4. Map to 'SortingOptions' by calling extension method.
5. Apply sorting by calling 'ApplySorting' method.
```C#
[HttpPost("sort")]
public IActionResult GetAsync(SortingOptionsModel sortingOptionsModel)
{
    var sortingOptionsRes = sortingOptionsModel.ToOptions<UsersSortingProperties>();
    if (sortingOptionsRes.IsFailure)
    {
        return BadRequest(sortingOptionsRes.ErrorMessage);
    }

    var sortingOptions = sortingOptionsRes.Value;
    int totalUsersCount = dbContext.Users.Count();

    var result = dbContext
        .Users
        .Include(e => e.Accounts)
        .OrderBy(e => e.FullName)
        .AsNoTracking();

    result = sortingManager.ApplySorting(result, sortingOptions);

    var users = result.Select(e => new UserDTO(
        e.Id,
        e.FullName,
        e.Age,
        e.Created,
        e.Accounts.Select(i => new AccountDTO(i.Id, i.Followers)).ToList()
        ))
        .ToList();

    return Ok(new UsersPage(users, totalUsersCount, sortingOptions, null));
}
```

##### Notes:
- only public/internal static readonly fields will be valid enum values.
- you must inherit your sorting enum from base 'SortingEnum' class.
- use nameof construction instead of passing string raws directly.
