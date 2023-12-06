using System.ComponentModel.DataAnnotations;
using Skanaus.Auth.Model;

namespace Skanaus.Data.Entities;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Body { get; set; }
    public DateTime CreationDate { get; set; }
    public required Kitchen Kitchen { get; set; }
    [Required]
    public required string UserId { get; set; }
    public ForumRestUser User { get; set; }
}
public record RecipeDto(int Id, string Name, string Body, DateTime CreationDate, Kitchen kitchen);
public record RecipeAllDto(int Id, string Name, string Body, DateTime CreationDate);