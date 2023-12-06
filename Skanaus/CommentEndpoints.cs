using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;
using Skanaus.Data;
using Skanaus.Data.Dtos;
using Skanaus.Data.Entities;
using Skanaus.Helpers;

namespace Skanaus;

public static class CommentEndpoints
{
    public static void AddCommentApi(this WebApplication app)
    {
        var commentsGroup = app.MapGroup("/api/kitchens/{kitchenId}/recipes/{recipeId}").WithValidationFilter();

commentsGroup.MapGet("comments/{commentId}", async (int kitchenId, int recipeId, int commentId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();

    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId && p.Kitchen.Id == kitchenId);
    if (recipe == null)
        return Results.NotFound();
    
    var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId && c.Recipe.Id == recipeId);
    if (comment == null)
        return Results.NotFound();
    
    return Results.Ok(comment);
});

commentsGroup.MapGet("comments", async (int kitchenId, int recipeId, ForumDbContext  dbContext, CancellationToken cancellationToken) =>
{ 
    var recipeComments = await dbContext.Comments
        .Where(comment => comment.Recipe.Id == recipeId && comment.Recipe.Kitchen.Id == kitchenId)
        .ToListAsync(cancellationToken);
    
    return recipeComments
        .Select(comment => new CommentAllDto(comment.Id, comment.Content, comment.CreationDate));
});

commentsGroup.MapPost("comments", async ([Validate]CreateCommentDto createCommentDto, int kitchenId, int recipeId,HttpContext httpContext , ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();
    
    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId && p.Kitchen.Id == kitchenId);
    if (recipe == null)
        return Results.NotFound();
    
    var comment = new Comment()
    {
        Content = createCommentDto.Content,
        CreationDate = DateTime.UtcNow,
        Recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId),
        UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
    };
    dbContext.Comments.Add(comment);

    await dbContext.SaveChangesAsync();
    
    return Results.Created($"/api/kitchens/{kitchen.Id}/recipes/{recipe.Id}/comments/{comment.Id}",
        new CommentDto(comment.Id, comment.Content, comment.CreationDate, comment.Recipe));
});

commentsGroup.MapPut("comments/{commentId}", async (int kitchenId, int recipeId, int commentId, [Validate]UpdateCommentDto dto, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();
    
    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId);
    if (recipe == null)
        return Results.NotFound();
    
    var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
    if (comment == null)
        return Results.NotFound();

    comment.Content = dto.Content;

    dbContext.Update(comment);
    await dbContext.SaveChangesAsync();
    
    return Results.Ok(new CommentDto(comment.Id, comment.Content, comment.CreationDate, comment.Recipe));
});

commentsGroup.MapDelete("comments/{commentId}", async (int kitchenId,int recipeId, int commentId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null) 
        return Results.NotFound();
    
    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId);
    if (recipe == null)
        return Results.NotFound();
    
    var comment = await dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
    if (comment == null)
        return Results.NotFound();

    dbContext.Remove(comment);
    await dbContext.SaveChangesAsync();
    
    return Results.NoContent();
});
    }
}