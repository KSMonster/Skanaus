using System.ComponentModel.DataAnnotations;
using Skanaus.Auth.Model;

namespace Skanaus.Data.Entities;

public class Kitchen
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required DateTime CreationDate { get; set; }
    
    [Required]
    public required string UserId { get; set; }
    public ForumRestUser User { get; set; }
}

public record KitchenDto(int Id, string Name, string Description, DateTime CreationDate);