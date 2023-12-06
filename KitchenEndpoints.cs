using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;
using Skanaus.Auth.Model;
using Skanaus.Data;
using Skanaus.Data.Dtos;
using Skanaus.Data.Entities;
using Skanaus.Helpers;

namespace Skanaus;

public class KitchenEndpoints
{
    public static void AddKitchenAPI(RouteGroupBuilder kitchensGroup)
    {
        // /posts>?page
        kitchensGroup.MapGet("kitchens", async ([AsParameters] SearchParameters searchParams, ForumDbContext dbContext,LinkGenerator linkGenerator, HttpContext httpContext) =>
        {
            var queryable = dbContext.Kitchens.AsQueryable().OrderBy(o => o.CreationDate);
            var pagedList = await PagedList<Kitchen>.CreateAsync(queryable, searchParams.PageNumber.Value,
                searchParams.PageSize.Value);

            var previousPageLink = pagedList.HasPrevious
                ? linkGenerator.GetUriByName(httpContext, "GetKitchens",
                    new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                : null;
                var nextPageLink = pagedList.HasNext
                    ? linkGenerator.GetUriByName(httpContext, "GetKitchens",
                        new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                    : null;

                var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize,
                    pagedList.CurrentPage, pagedList.TotalPages, previousPageLink, nextPageLink);
                
                httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
            
            return pagedList.Select(kitchen => new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
            
            /*var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id >= 0);
            if (kitchen == null)
            {
                return Results.NotFound();
            }

    return (await dbContext.Kitchens.ToListAsync(cancellationToken))
        .Select(kitchen => new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));*/
    

}).WithName("GetKitchens");

kitchensGroup.MapGet("kitchens/{kitchenId}", async (int kitchenId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
}).WithName("GetKitchen");

kitchensGroup.MapPost("kitchens", [Authorize(Roles = ForumRoles.ForumUser)] async ([Validate]CreateKitchenDto createKitchenDto,HttpContext httpContext, LinkGenerator linkGenerator, ForumDbContext dbContext) =>
{
    var kitchen = new Kitchen()
    {
        Name = createKitchenDto.Name,
        Description = createKitchenDto.Description,
        CreationDate = DateTime.UtcNow,
        UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
    };
    dbContext.Kitchens.Add(kitchen);

    await dbContext.SaveChangesAsync();

    var links = CreateLinks(kitchen.Id, httpContext, linkGenerator);
    var kitchenDto = new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate);

    var resource = new ResourceDto<KitchenDto>(kitchenDto, links.ToArray());
    
    return Results.Created($"/api/kitchens/{kitchen.Id}", resource);
}).WithName("CreateKitchen");

kitchensGroup.MapPut("kitchens/{kitchenId}", [Authorize(Roles = ForumRoles.ForumUser)] async (int kitchenId, [Validate]UpdateKitchenDto dto, HttpContext httpContext, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
    {
        return Results.NotFound();
    }

    if (!httpContext.User.IsInRole(ForumRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != kitchen.UserId)
    {
        return Results.Forbid();
    }
    
    kitchen.Description = dto.Description;

    dbContext.Update(kitchen);
    await dbContext.SaveChangesAsync();
    
    return Results.Ok(new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
}).WithName("EditKitchen");

kitchensGroup.MapDelete("kitchens/{kitchenId}", async (int kitchenId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
    {
        return Results.NotFound();
    }

    dbContext.Remove(kitchen);
    await dbContext.SaveChangesAsync();
    
    return Results.NoContent();
}).WithName("RemoveKitchen");
    }


    static IEnumerable<LinkDto> CreateLinks(int kitchenId, HttpContext httpContext, LinkGenerator linkGenerator)
    {
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetKitchen", new { kitchenId }), "self",
            "GET");
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditKitchen", new { kitchenId }), "edit",
            "PUT");
        yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveKitchen", new { kitchenId }), "delete",
            "DELETE");
    }
}