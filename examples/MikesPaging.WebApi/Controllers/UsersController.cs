using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MikesPaging.AspNetCore;
using MikesPaging.AspNetCore.Common.ViewModels;
using MikesPaging.AspNetCore.Services.Interfaces;
using MikesPaging.WebApi.Data;
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
}
