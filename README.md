# MikesPaging.AspNetCore [DEPRECATED]

Use instead https://github.com/Vazovsk1y/MikesSifter

MikesPaging.AspNetCore is a simple library for implementing paging, sorting, and filtering functionality in your ASP.NET applications.

## Install

Minimum Requirements: **.NET 8.0.x**

[Download from NuGet](https://www.nuget.org/packages/MikesPaging.AspNetCore/)

##### PowerShell

```powershell
NuGet\Install-Package MikesPaging.AspNetCore -Version *version_number*
```

##### CMD

```cmd
dotnet add package MikesPaging.AspNetCore --version *version_number*
```

## Usage

In this example, consider an app with a `User` entity that can have many accounts.
We'll use MikesPaging.AspNetCore to add sorting, filtering, and pagination capabilities when retrieving all available users.

### 1. Paging

##### Dependency Injection (DI)

```csharp
builder.Services.AddPaging();
```

##### Code Example

1. Inject `IPagingManager<T>`.
2. Apply paging by calling `ApplyPaging` and obtain the result collection.

```csharp
[HttpPost("page")]
public IActionResult GetAsync(PagingOptionsModel pagingOptionsModel)
{
    var pagingOptionsRes = pagingOptionsModel.ToOptions();
    if (pagingOptionsRes.IsFailure)
    {
        return BadRequest(pagingOptionsRes.Errors);
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

##### Notes

- You can use the built-in `PagingOptionsModel` class as an API endpoint argument that supports mapping to `PagingOptions` that you must pass to the `ApplyPaging` method.
- It is recommended to sort the collection before applying pagination.

### 2. Sorting

##### Dependency Injection (DI)

```csharp
builder.Services.AddSorting();
```

##### Code Example

1. Define sorting enum for the entity.

```csharp
public sealed class UsersSortingProperties : SortingEnum
{
    public static readonly UsersSortingProperties ByFullName = new(nameof(User.FullName), new[] { nameof(User.FullName), "user_fullname" });

    public static readonly UsersSortingProperties ByAge = new(nameof(User.Age), new[] { nameof(User.Age), "user_age" });

    public static readonly UsersSortingProperties ByCreatedDate = new(nameof(User.Created), new[] { nameof(User.Created), "created_date" });

    public static readonly UsersSortingProperties ByAccountsCount = new("AccountsCount", new[] { "AccountsCount", "accounts_count" });

    private UsersSortingProperties(string propetyName, IReadOnlyCollection<string> allowedValues) : base(propetyName, allowedValues) { }
}
```

2. Inject `ISortingManager<T>`.
3. Apply sorting by calling `ApplySorting` method and obtain the result collection.

```csharp
[HttpPost("sort")]
public IActionResult GetAsync(SortingOptionsModel sortingOptionsModel)
{
    var sortingOptionsRes = sortingOptionsModel.ToOptions<UsersSortingProperties>();
    if (sortingOptionsRes.IsFailure)
    {
        return BadRequest(sortingOptionsRes.Errors);
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

##### Notes

- Only `public/internal static readonly` fields will be valid enum values.
- You must inherit your sorting enum from the base `SortingEnum` class.
- Use the `nameof` keyword instead of passing string values directly.
- You can use the built-in `SortingOptionsModel` class as an API endpoint argument that supports mapping to `SortingOptions` that you must pass to the `ApplySorting` method.

### 3. Filtering

##### Dependency Injection (DI)

```csharp
builder.Services.AddFiltering();
```

##### Code Example

1. Define filtering enum for the entity.

```csharp
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
            FilteringOperators.Equal,
            FilteringOperators.NotEqual,
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
```

2. Inject `IFilteringManager<T>`.
3. Apply filtering by calling the `ApplyFiltering` method and obtain the result collection.

```csharp
[HttpPost("filter")]
public IActionResult GetAsync(FilteringOptionsModel filteringOptionsModel)
{
    var filteringOptionsRes = filteringOptionsModel.ToOptions<UsersFilteringProperties>();
    if (filteringOptionsRes.IsFailure)
    {
        return BadRequest(filteringOptionsRes.Errors);
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

##### Notes

- Only `public/internal static readonly` fields will be valid enum values.
- You must inherit your filtering enum from the base `FilteringEnum` class.
- Use the `nameof` keyword instead of passing string values directly.
- You can use the built-in `FilteringOptionsModel` class as an API endpoint argument that supports mapping to `FilteringOptions` that you must pass to the `ApplyFiltering` method.

### Configurations

#### Sorting

If you need to define a custom sorter for any property that you defined in your sorting enum, you can configure it as shown in the example below.

```csharp
public class UsersSortingConfiguration : SortingConfiguration<User, UsersSortingProperties>
{
    public UsersSortingConfiguration()
    {
        SortFor(UsersSortingProperties.ByAccountsCount, e => e.Accounts.Count);
    }
}
```

##### Dependency Injection (DI)

```csharp
builder.Services.AddSortingConfigurationsFromAssembly(typeof(UsersSortingConfiguration).Assembly);
```

#### Filtering

If you need to define a custom filter for any property that you defined in your filtering enum, you can configure it as shown in the example below.

```csharp
public class UsersFilteringConfiguration : FilteringConfiguration<User, UsersFilteringProperties>
{
    public UsersFilteringConfiguration()
    {
        FilterFor(UsersFilteringProperties.ByAccounts, FilteringOperators.Contains, filterValue =>
        {
            if (!Guid.TryParse(filterValue, out var accountId))
            {
                throw new InvalidCastException($"Unable to cast {filterValue} to Guid.");
            }

            return user => user.Accounts.Any(a => a.Id == accountId);
        });
    }
}
```

##### Dependency Injection (DI)

```csharp
builder.Services.AddFilteringConfigurationsFromAssembly(typeof(UsersFilteringConfiguration).Assembly);
```

### Notes

- You can return your paged/sorted/filtered collection directly, or you can define a page class that will be inherited from one of the possible pages and use it as a return type.

```csharp
public abstract record Page<TItem> :

 IPage<TItem>;

public abstract record Page<TItem, TSorting, TFiltering> :
    Page<TItem>
    where TSorting : class, ISortingOptions
    where TFiltering : class, IFilteringOptions;

public abstract record FilteredPage<TItem, TFiltering> : Page<TItem>
    where TFiltering : class, IFilteringOptions;

public abstract record SortedPage<TItem, TSorting> : Page<TItem>
    where TSorting : class, ISortingOptions;

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

- You can use all at once with the `ReceivingModel` class as shown in the example below.

```csharp
[HttpPost("full")]
public IActionResult GetAsync(ReceivingModel receivingModel)
{
    var pagingOptionsRes = receivingModel.PagingOptions.ToOptions();
    if (pagingOptionsRes.IsFailure)
    {
        return BadRequest(pagingOptionsRes.Errors);
    }

    var sortingOptionsRes = receivingModel.SortingOptions.ToOptions<UsersSortingProperties>();
    if (sortingOptionsRes.IsFailure)
    {
        return BadRequest(sortingOptionsRes.Errors);
    }

    var filteringOptionsRes = receivingModel.FilteringOptions.ToOptions<UsersFilteringProperties>();
    if (filteringOptionsRes.IsFailure)
    {
        return BadRequest(filteringOptionsRes.Errors);
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