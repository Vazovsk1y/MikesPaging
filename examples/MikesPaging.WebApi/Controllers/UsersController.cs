using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MikesPaging.AspNetCore;
using MikesPaging.AspNetCore.Common.ViewModels;
using MikesPaging.AspNetCore.Services.Interfaces;
using MikesPaging.WebApi.Data;
using MikesPaging.WebApi.Infrastructure;
using MikesPaging.WebApi.Models;

namespace MikesPaging.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    WebApiExampleDbContext dbContext,
    IPagingManager<User> pagingManager,
    ISortingManager<User> sortingManager,
    IFilteringManager<User> filteringManager
    ) : ControllerBase
{
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
}
