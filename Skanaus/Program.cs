using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using O9d.AspNet.FluentValidation;
using Skanaus;
using Skanaus.Data;
using Skanaus.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Skanaus.Auth;
using Skanaus.Auth.Model;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//
//
//

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
// Npgsql.EntityFrameworkCore.PostgreSQL
// Microsoft.EntityFrameworkCore.Tools

// FluentValidation
// FluentValidation.DependencyInjectionExtensions
// 09d.AspNet.FluentValidation

builder.Services.AddDbContext<ForumDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddIdentity<ForumRestUser, IdentityRole>()
    .AddEntityFrameworkStores<ForumDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy

                .WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});




var app = builder.Build();

var kitchensGroup = app.MapGroup("/api").WithValidationFilter();

KitchenEndpoints.AddKitchenAPI(kitchensGroup);
//posts
//comments
//auth

//recipes

//var recipeGroup = app.MapGroup("/api/kitchens/{kitchenId}").WithValidationFilter();

app.AddRecipeApi();


//comments

app.AddCommentApi();

app.AddAuthApi();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
dbContext.Database.Migrate();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();

//public record GetCommentParameters(int kitchenId, int recipeId, int commentId, ForumDbContext dbContext);
