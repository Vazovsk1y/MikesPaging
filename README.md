# MikesPaging.AspNetCore
MikesPaging.AspNetCore is a simple library for implementing paging, sorting and filtering functionality to your ASP.NET applications.

## Install

Minimum Requirements: **.NET 8.0.x**

[Download from Nuget](https://www.nuget.org/packages/MikesPaging.AspNetCore/)

##### powershell

```powershell
NuGet\Install-Package MikesPaging.AspNetCore -Version *version_number*
```

##### cmd
```cmd
dotnet add package MikesPaging.AspNetCore --version *version_number*
```

## Usage

In this example, consider an app with a `User` entity. 
We'll use MakesPaging.AspNetCore to add sorting, filtering, and pagination capabilities when receiving all available users.

### 1. Paging

##### DI
```C#
builder.Services.AddPaging();
```
##### Code example
1. Inject IPagingManager<'T'> to use paging.
2. Apply paging by calling 'ApplyPaging' and obtain result collection.

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
- you can use built in 'PagingOptionsModel' class as api endpoint argument that supports mapping to 'PagingOptions' that you can pass to 'ApplyPaging' method.

### 2. Sorting

##### DI
```C#
builder.Services.AddSorting();
```

##### Code example
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
3. Apply sorting by calling 'ApplySorting' method and obtain result collection.
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
- you can use built in 'SortingOptionsModel' class as api endpoint argument that supports mapping to 'SortingOptions' that you can pass to 'ApplySorting' method.

### 3. Filtering

##### DI
```C#
builder.Services.AddFiltering();
```

##### Code example
1. Define filtering enum for entity.
```C# 
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
```
2. Inject IFilteringManager<'T'> to use filtering.
3. Apply filtering by calling 'ApplyFiltering' method and obtain result collection.
```C#
[HttpPost("filter")]
public IActionResult GetAsync(FilteringOptionsModel filteringOptionsModel)
{
    var filteringOptionsRes = filteringOptionsModel.ToOptions<UsersFilteringProperties>();
    if (filteringOptionsRes.IsFailure)
    {
        return BadRequest(filteringOptionsRes.ErrorMessage);
    }

    var filteringOptions = filteringOptionsRes.Value;
    int totalUsersCount = filteringManager.ApplyFiltering(dbContext.Users, filteringOptions).Count();

    var result = dbContext
        .Users
        .Include(e => e.Accounts)
        .OrderBy(e => e.FullName)
        .AsNoTracking();

    result = filteringManager.ApplyFiltering(result, filteringOptions);

    var users = result.Select(e => new UserDTO(
        e.Id,
        e.FullName,
        e.Age,
        e.Created,
        e.Accounts.Select(i => new AccountDTO(i.Id, i.Followers)).ToList()
        ))
        .ToList();

    return Ok(new UsersPage(users, totalUsersCount, null, filteringOptions, null));
}
```

##### Notes:
- only public/internal static readonly fields will be valid enum values.
- you must inherit your sorting enum from base 'FilteringEnum' class.
- use nameof construction instead of passing string raws directly.
- you can use built in 'FilteringOptionsModel' class as api endpoint argument that supports mapping to 'FilteringOptions' that you can pass to 'ApplyFiltering' method.

### Configurations

#### Sorting
If you need to define custom sorter for any property that you defined in your sorting enum you can configure it as shown in example below.

```C#
public class UsersSortingConfiguration : SortingConfiguration<User, UsersSortingProperties>
{
    public UsersSortingConfiguration()
    {
        RuleFor(UsersSortingProperties.ByAccountsCount, e => e.Accounts.Count);
    }
}
```

##### DI
```C#
builder.Services.AddSortingConfigurationsFromAssembly(typeof(UsersSortingConfiguration).Assembly);
```


#### Filtering
If you need to define custom filter for any property that you defined in your filtering enum you can configure it as shown in example below.

```C#
public class UsersFilteringConfiguration : FilteringConfiguration<User, UsersFilteringProperties>
{
    public UsersFilteringConfiguration()
    {
        RuleFor(UsersFilteringProperties.ByAccounts, FilteringOperators.Contains, filterValue =>
        {
            if (!Guid.TryParse(filterValue, out var accountId))
            {
                throw new InvalidCastException($"Unable cast {filterValue} to guid.");
            }

            return user => user.Accounts.Any(a => a.Id == accountId);
        });
    }
}
```

##### DI
```C#
builder.Services.AddFilteringConfigurationsFromAssembly(typeof(UsersFilteringConfiguration).Assembly);
```

### Notes
- you can return your paged/sorted/filtered collection directly or you can define page class that will be inherit one of possible pages and use it like return type.
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
```
- you can use all at once as shown in the example below.
```C#
[HttpPost]
public IActionResult GetAsync(ReceivingModel receivingModel)
{
    var pagingOptionsRes = receivingModel.PagingOptions.ToOptions();
    if (pagingOptionsRes.IsFailure)
    {
        return BadRequest(pagingOptionsRes.ErrorMessage);
    }

    var sortingOptionsRes = receivingModel.SortingOptions.ToOptions<UsersSortingProperties>();
    if (sortingOptionsRes.IsFailure)
    {
        return BadRequest(sortingOptionsRes.ErrorMessage);
    }

    var filteringOptionsRes = receivingModel.FilteringOptions.ToOptions<UsersFilteringProperties>();
    if (filteringOptionsRes.IsFailure)
    {
        return BadRequest(filteringOptionsRes.ErrorMessage);
    }

    var pagingOptions = pagingOptionsRes.Value;
    var sortingOptions = sortingOptionsRes.Value;
    var filteringOptions = filteringOptionsRes.Value;

    var totalUsersCount = filteringManager.ApplyFiltering(dbContext.Users, filteringOptions).Count();
    var result = dbContext
        .Users
        .Include(e => e.Accounts)
        .AsNoTracking();

    result = filteringManager.ApplyFiltering(result, filteringOptions);
    result = sortingManager.ApplySorting(result, sortingOptions);
    result = pagingManager.ApplyPaging(result, pagingOptions);

    var users = result.Select(e => new UserDTO(
        e.Id,
        e.FullName,
        e.Age,
        e.Created,
        e.Accounts.Select(i => new AccountDTO(i.Id, i.Followers)).ToList()
        ))
        .ToList();

    return Ok(new UsersPage(users, totalUsersCount, sortingOptions, filteringOptions, pagingOptions));
}
```
