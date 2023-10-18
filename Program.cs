using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using O9d.AspNet.FluentValidation;
using Skanaus.Data;
using Skanaus.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
// Npgsql.EntityFrameworkCore.PostgreSQL
// Microsoft.EntityFrameworkCore.Tools

// FluentValidation
// FluentValidation.DependencyInjectionExtensions
// 09d.AspNet.FluentValidation

builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

var kitchensGroup = app.MapGroup("/api").WithValidationFilter();

kitchensGroup.MapGet("kitchens", async (ForumDbContext dbContext, CancellationToken cancellationToken) =>
{ 
    return (await dbContext.Kitchens.ToListAsync(cancellationToken))
        .Select(kitchen => new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
});

kitchensGroup.MapGet("kitchens/{kitchenId}", async (int kitchenId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
});

kitchensGroup.MapPost("kitchens", async ([Validate]CreateKitchenDto createKitchenDto, ForumDbContext dbContext) =>
{
    var kitchen = new Kitchen()
    {
        Name = createKitchenDto.Name,
        Description = createKitchenDto.Description,
        CreationDate = DateTime.UtcNow
    };
    dbContext.Kitchens.Add(kitchen);

    await dbContext.SaveChangesAsync();
    
    return Results.Created($"/api/kitchens/{kitchen.Id}",
        new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
});

kitchensGroup.MapPut("kitchens/{kitchenId}", async (int kitchenId, [Validate]UpdateKitchenDto dto, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
    {
        return Results.NotFound();
    }

    kitchen.Description = dto.Description;

    dbContext.Update(kitchen);
    await dbContext.SaveChangesAsync();
    
    return Results.Ok(new KitchenDto(kitchen.Id, kitchen.Name, kitchen.Description, kitchen.CreationDate));
});

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
});

//recipes

var recipeGroup = app.MapGroup("/api/kitchens/{kitchenId}").WithValidationFilter();

recipeGroup.MapGet("recipes", async (int kitchenId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
{ 
    var kitchenRecipes = await dbContext.Recipes
        .Where(recipe => recipe.Kitchen.Id == kitchenId)
        .ToListAsync(cancellationToken);
    
    return kitchenRecipes
        .Select(recipe => new RecipeAllDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate));
});

recipeGroup.MapGet("recipes/{recipeId}", async (int kitchenId, int recipeId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();

    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId && p.Kitchen.Id == kitchenId);
    if (recipe == null)
        return Results.NotFound();
    
    return Results.Ok(recipe);
});

recipeGroup.MapPost("recipes/{recipeId}", async ([Validate]CreateRecipeDto createRecipeDto,int kitchenId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();
    
    var recipe = new Recipe()
    {
        Name = createRecipeDto.Name,
        Body = createRecipeDto.Body,
        CreationDate = DateTime.UtcNow,
        Kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId)
    };
    dbContext.Recipes.Add(recipe);

    await dbContext.SaveChangesAsync();
    
    return Results.Created($"/api/kitchens/{kitchenId}/recipes/{recipe.Id}",
        new RecipeDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate, recipe.Kitchen));
});

recipeGroup.MapPut("recipes/{recipeId}", async (int kitchenId, int recipeId, [Validate]UpdateRecipeDto dto, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null)
        return Results.NotFound();
    
    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId);
    if (recipe == null)
        return Results.NotFound();

    recipe.Body = dto.Body;

    dbContext.Update(recipe);
    await dbContext.SaveChangesAsync();
    
    return Results.Ok(new RecipeDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate, recipe.Kitchen));
});

recipeGroup.MapDelete("recipes/{recipeId}", async (int kitchenId,int recipeId, ForumDbContext dbContext) =>
{
    var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
    if (kitchen == null) 
        return Results.NotFound();
    
    var recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId);
    if (recipe == null)
        return Results.NotFound();

    dbContext.Remove(recipe);
    await dbContext.SaveChangesAsync();
    
    return Results.NoContent();
});

//comments

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

commentsGroup.MapPost("comments/{commentId}", async ([Validate]CreateCommentDto createCommentDto, int kitchenId, int recipeId, ForumDbContext dbContext) =>
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
        Recipe = await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId)
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

app.Run();

//public record GetCommentParameters(int kitchenId, int recipeId, int commentId, ForumDbContext dbContext);
public record CreateKitchenDto(string Name, string Description);
public record UpdateKitchenDto(string Description);
public record CreateRecipeDto(string Name, string Body);
public record UpdateRecipeDto(string Body);

public record CreateCommentDto(string Content);
public record UpdateCommentDto(string Content);


public class CreateKitchenDtoValidator : AbstractValidator<CreateKitchenDto>
{
    public CreateKitchenDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateKitchenDtoValidator : AbstractValidator<CreateKitchenDto>
{
    public UpdateKitchenDtoValidator()
    {
        RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class CreateRecipeDtoValidator : AbstractValidator<CreateRecipeDto>
{
    public CreateRecipeDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
        RuleFor(dto => dto.Body).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateRecipeDtoValidator : AbstractValidator<CreateRecipeDto>
{
    public UpdateRecipeDtoValidator()
    {
        RuleFor(dto => dto.Body).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(dto => dto.Content).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}
public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
{
    public UpdateCommentDtoValidator()
    {
        RuleFor(dto => dto.Content).NotEmpty().NotNull().Length(min: 10, max: 300);
    }
}