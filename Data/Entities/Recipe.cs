namespace Skanaus.Data.Entities;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Ingredients { get; set; }
    public required string Body { get; set; }
    public DateTime CreationDate { get; set; }
    
    public required Kitchen Kitchen { get; set; }
}
public record RecipeDto(int Id, string Name,string Ingredients, string Body, DateTime CreationDate, Kitchen kitchen);
public record RecipeAllDto(int Id, string Name,string Ingredients, string Body, DateTime CreationDate);