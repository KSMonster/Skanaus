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

public static class RecipeEndpoints
{
    public static void AddRecipeApi(this WebApplication app)
    {
        var recipeGroup = app.MapGroup("/api/kitchens/{kitchenId}").WithValidationFilter();

        recipeGroup.MapGet("recipes",
            async (int kitchenId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var kitchenRecipes = await dbContext.Recipes
                    .Where(recipe => recipe.Kitchen.Id == kitchenId)
                    .ToListAsync(cancellationToken);

                return kitchenRecipes
                    .Select(recipe => new RecipeAllDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate));
            });

        recipeGroup.MapGet("recipes/{recipeId}",
            async (int kitchenId, int recipeId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
                if (kitchen == null)
                    return Results.NotFound();

                var recipe =
                    await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId && p.Kitchen.Id == kitchenId);
                if (recipe == null)
                    return Results.NotFound();

                return Results.Ok(recipe);
            });

        recipeGroup.MapPost("recipes",
            async ([Validate] CreateRecipeDto createRecipeDto, int kitchenId, HttpContext httpContext,
                ForumDbContext dbContext) =>
            {
                var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
                if (kitchen == null)
                    return Results.NotFound();

                var recipe = new Recipe()
                {
                    Name = createRecipeDto.Name,
                    Body = createRecipeDto.Body,
                    CreationDate = DateTime.UtcNow,
                    Kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId),
                    UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                };
                dbContext.Recipes.Add(recipe);

                await dbContext.SaveChangesAsync();

                return Results.Created($"/api/kitchens/{kitchenId}/recipes/{recipe.Id}",
                    new RecipeDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate, recipe.Kitchen));
            });

        recipeGroup.MapPut("recipes/{recipeId}",
            async (int kitchenId, int recipeId, [Validate] UpdateRecipeDto dto, ForumDbContext dbContext) =>
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

                return Results.Ok(new RecipeDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate,
                    recipe.Kitchen));
            });

        recipeGroup.MapDelete("recipes/{recipeId}", async (int kitchenId, int recipeId, ForumDbContext dbContext) =>
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
    }
}

/*public static void AddRecipeAPI(RouteGroupBuilder recipeGroup)
{
    recipeGroup.MapGet("recipes",
        async (int kitchenId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var kitchenRecipes = await dbContext.Recipes
                .Where(recipe => recipe.Kitchen.Id == kitchenId)
                .ToListAsync(cancellationToken);

            return kitchenRecipes
                .Select(recipe => new RecipeAllDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate));
        });

    recipeGroup.MapGet("recipes/{recipeId}",
        async (int kitchenId, int recipeId, ForumDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var kitchen = await dbContext.Kitchens.FirstOrDefaultAsync(t => t.Id == kitchenId);
            if (kitchen == null)
                return Results.NotFound();

            var recipe =
                await dbContext.Recipes.FirstOrDefaultAsync(p => p.Id == recipeId && p.Kitchen.Id == kitchenId);
            if (recipe == null)
                return Results.NotFound();

            return Results.Ok(recipe);
        });

    recipeGroup.MapPost("recipes",
        async ([Validate] CreateRecipeDto createRecipeDto, int kitchenId, ForumDbContext dbContext) =>
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

    recipeGroup.MapPut("recipes/{recipeId}",
        async (int kitchenId, int recipeId, [Validate] UpdateRecipeDto dto, ForumDbContext dbContext) =>
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

            return Results.Ok(new RecipeDto(recipe.Id, recipe.Name, recipe.Body, recipe.CreationDate,
                recipe.Kitchen));
        });

    recipeGroup.MapDelete("recipes/{recipeId}", async (int kitchenId, int recipeId, ForumDbContext dbContext) =>
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
}
}*/