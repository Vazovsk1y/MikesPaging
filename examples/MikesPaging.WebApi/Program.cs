using Microsoft.EntityFrameworkCore;
using MikesPaging.AspNetCore;
using MikesPaging.WebApi;
using MikesPaging.WebApi.Data;
using MikesPaging.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<WebApiExampleDbContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddPaging();
builder.Services.AddSorting();
builder.Services.AddFiltering();
builder.Services.AddSortingConfigurationsFromAssembly(typeof(UsersSortingConfiguration).Assembly);
builder.Services.AddFilteringConfigurationsFromAssembly(typeof(UsersFilteringConfiguration).Assembly);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
}

app.Run();
